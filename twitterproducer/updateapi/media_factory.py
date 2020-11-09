from tweepy import Status

from updatesproducer.updateapi.photo import Photo
from updatesproducer.updateapi.video import Video


class MediaFactory:
    @staticmethod
    def to_media(tweet: Status):
        tweet_media = MediaFactory.get_tweet_media(tweet)

        media = []
        for m in tweet_media:
            media_url = None if m.get('media_url_https') is None else m.get('media_url')

            if m.get('type') == 'photo':
                media.append(Photo(
                    url=media_url
                ))

            else:
                video_info = m.get('video_info')
                variants = video_info.get('variants')

                video_with_highest_bitrate = sorted(
                    filter(
                        lambda variant: variant.get('bitrate') is not None,
                        variants),
                    key=lambda v: v.get('bitrate'),
                    reverse=True)[0]

                sizes = m.get('sizes')

                try:
                    best_video_size = list(filter(
                        lambda s: s is not None,
                        [
                            sizes.get('large'),
                            sizes.get('medium'),
                            sizes.get('small')
                        ]))[0]

                    media.append(Video(
                        url=video_with_highest_bitrate.get('url'),
                        thumbnail_url=media_url,
                        duration_seconds=video_info.get('duration_millis') / 1000,
                        width=best_video_size.get('w'),
                        height=best_video_size.get('h')
                    ))
                except:
                    media.append(Video(
                        url=video_with_highest_bitrate.get('url')
                    ))

        return media

    @staticmethod
    def get_tweet_media(tweet: Status):
        try:
            return tweet.extended_entities['media']
        except AttributeError:
            # If no media is then return an empty list
            return []
