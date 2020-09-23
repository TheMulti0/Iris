from youtubeproducer.updateapi.update_factory import UpdateFactory
from updatesproducer.iupdates_provider import IUpdatesProvider
from youtubeproducer.videos.ivideos_provider import IVideosProvider


class YouTubeUpdatesProvider(IUpdatesProvider):
    def __init__(self, videos_provider: IVideosProvider):
        self.__videos_provider = videos_provider

    def get_updates(self, user_id: str):
        return [
            UpdateFactory.to_update(video)
            for video in self.__videos_provider.get_videos(user_id)
        ]
