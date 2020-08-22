from datetime import datetime

from youtubeproducer.videos.ivideos_provider import IVideosProvider
from youtubeproducer.videos.video import Video


class MockVideosProvider(IVideosProvider):
    def get_videos(self, user_id):
        return [
            Video({
                'channelId': 'Mock channel id',
                'title': 'Mock title',
                'description': 'Mock description',
                'thumbnails': {},
                'publishedAt': datetime.now(),
                'publishTime': datetime.now(),
                'channelTitle': 'Mock channel title',
                'liveBroadcastContent': 'sample-photo-url'
            }, 'video_id')]
