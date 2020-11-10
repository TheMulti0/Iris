import asyncio
from logging import Logger

from updatesproducer.cancellation_token import CancellationToken
from updatesproducer.db.iupdates_repository import IUpdatesRepository
from updatesproducer.iupdates_provider import IUpdatesProvider
from updatesproducer.iupdatespipe import IUpdatesPipe
from updatesproducer.updates_poller_config import UpdatesPollerConfig
from updatesproducer.updates_producer import UpdatesProducer


class UpdatesPoller(IUpdatesPipe):
    def __init__(
            self,
            config: UpdatesPollerConfig,
            producer: UpdatesProducer,
            repository: IUpdatesRepository,
            updates_provider: IUpdatesProvider,
            cancellation_token: CancellationToken,
            logger: Logger):
        self.__config = config
        self.__producer = producer
        self.__repository = repository
        self.__updates_provider = updates_provider
        self.__cancellation_token = cancellation_token
        self.__logger = logger

    async def start(self):
        while True:
            self.__logger.info('Polling all users')
            self.poll()

            if self.__cancellation_token.cancelled:
                return

            interval_seconds = self.__config.update_interval_seconds
            self.__logger.info('Done polling updates')
            self.__logger.info('Sleeping for %s seconds', interval_seconds)

            await asyncio.sleep(interval_seconds)

    def poll(self):
        for user in self.__config.watched_users:
            try:
                self._poll_user(user)
            except:
                self.__logger.exception(f'Failed to poll updates of user %s', user)

    def _poll_user(self, user_id):
        self.__logger.info('Polling updates of user %s', user_id)

        new_updates = self._get_new_updates(user_id)
        self.__logger.debug('Got new updates')

        updates_count = 0
        for update in new_updates:
            updates_count += 1

            self.__producer.send(update)

            self.__repository.set_user_latest_update_time(
                user_id,
                update.creation_date)

            if self.__config.store_sent_updates:
                self.__repository.update_sent(update.url)

        if updates_count == 0:
            self.__logger.info('No new updates found')

    def _get_new_updates(self, user_id):
        updates = filter(
            lambda u: u.creation_date is not None,
            self.__updates_provider.get_updates(user_id))

        sorted_updates = sorted(
            updates,
            key=lambda u: u.creation_date)

        user_latest_update_time = self.__repository.get_user_latest_update_time(user_id)

        def should_send_update(u):
            is_new = u.creation_date > user_latest_update_time['latest_update_time']

            if self.__config.store_sent_updates:
                sent = self.__repository.was_update_sent(u.url)
            else:
                sent = False

            # Both the user's latest post time and the updates are stored because only
            # updates that are less than 24 hour old are stored (because of Facebook's relative time specification
            # that can change between request to request)
            return is_new and not sent

        return filter(
            should_send_update,
            sorted_updates
        )
