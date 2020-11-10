from typing import List


class UpdatesPollerConfig:
    update_interval_seconds: float
    watched_users: List[str]
    store_sent_updates: bool = False

    def __init__(self, original_dict):
        self.__dict__.update(original_dict)