from abc import ABC


class IUpdatesRepository(ABC):
    def get_user_latest_update_time(self, user_id):
        pass

    def set_user_latest_update_time(self, user_id, latest_update_time):
        pass

    def was_update_sent(self, url):
        pass

    def update_sent(self, url):
        pass
