from abc import ABC


class UpdatesProvider(ABC):
    def get_updates(self, user_id: str):
        pass
