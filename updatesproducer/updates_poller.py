import asyncio
import logging

from updatesproducer.db.iupdates_repository import IUpdatesRepository
from updatesproducer.iupdates_provider import IUpdatesProvider
from updatesproducer.iupdatesproducer import IUpdatesProducer
from updatesproducer.updateapi.video import Video
from updatesproducer.updateapi.video_downloader import VideoDownloader


class UpdatesPoller:
    __config: dict

    def __init__(
            self,
            get_config,
            producer: IUpdatesProducer,
            repository: IUpdatesRepository,
            updates_provider: IUpdatesProvider,
            video_downloader: VideoDownloader):
        self.__get_config = get_config
        self.__producer = producer
        self.__repository = repository
        self.__updates_provider = updates_provider
        self.__video_downloader = video_downloader
        self.__logger = logging.getLogger(UpdatesPoller.__name__)

    async def start(self):
        while True:
            self.__config = self.__get_config()

            self.__logger.info('Polling all users')
            self.poll()

            if self.__cancellation_token.cancelled:
                return

            interval_seconds = self.__config['update_interval_seconds']
            self.__logger.info('Done polling updates')
            self.__logger.info('Sleeping for %s seconds', interval_seconds)

            await asyncio.sleep(interval_seconds)

    def poll(self):
        for user in self.__config['watched_users']:
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

            self._download_lowres_videos(update)

            self.__producer.send(update)

            self.__repository.set_user_latest_update_time(
                user_id,
                update.creation_date)

            if self.__config.get('store_sent_updates'):
                self.__repository.update_sent(update.url)

        if updates_count == 0:
            self.__logger.info('No new updates found')

    def _download_lowres_videos(self, update):
        if update.should_redownload_video:
            # Find all of the old lowres videos that this update has (if any)
            lowres_videos = list(filter(
                lambda m: isinstance(m, Video),
                update.media))

            if len(lowres_videos) != 0:
                for lowres_video in lowres_videos:
                    self.__video_downloader.download_video(update, lowres_video)

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

            if self.__config.get('store_sent_updates'):
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
