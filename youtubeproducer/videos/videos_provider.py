from googleapiclient.discovery import build

from youtubeproducer.videos.ivideos_provider import IVideosProvider
from youtubeproducer.videos.video import Video


class VideosProvider(IVideosProvider):
    def __init__(self, config):
        self.__youtube = build(
            'youtube',
            'v3',
            developerKey=config['api_key'])

    def get_videos(self, channel_id):
        search_response = self.__youtube.search().list(
            channelId=channel_id,
            part='id,snippet',
            maxResults=1,
            order='date',
            type='video'
        ).execute()

        return [
            Video(video['snippet'], video['id']['videoId'])
            for video in search_response.get('items', [])
            if video['id']['kind'] == 'youtube#video'
        ]
