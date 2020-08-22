from abc import ABC


class IVideosProvider(ABC):
    def get_videos(self, user_id):
        pass
