from abc import ABC


class IUpdatesProvider(ABC):
    def get_updates(self, user_id: str):
        pass
