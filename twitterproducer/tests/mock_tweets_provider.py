import ast
import json
from datetime import datetime

from tweepy import Status

from twitterproducer.tweets.itweets_provider import ITweetsProvider

status_json = "{'created_at': 'Wed Aug 26 03:22:48 +0000 2020', 'id': 1298460861900652545, 'id_str': " \
              "'1298460861900652545', 'full_text': 'https://t.co/WjbNIvW96r', 'truncated': False, " \
              "'display_text_range': [0, 23], 'entities': {'hashtags': [], 'symbols': [], 'user_mentions': [], " \
              "'urls': [], 'media': [{'id': 1298458167211425795, 'id_str': '1298458167211425795', 'indices': [0, 23], " \
              "'media_url': 'http://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH.jpg', " \
              "'media_url_https': 'https://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH" \
              ".jpg', 'url': 'https://t.co/WjbNIvW96r', 'display_url': 'pic.twitter.com/WjbNIvW96r', 'expanded_url': " \
              "'https://twitter.com/TeamTrump/status/1298458366306660353/video/1', 'type': 'photo', 'sizes': {" \
              "'thumb': {'w': 150, 'h': 150, 'resize': 'crop'}, 'medium': {'w': 1200, 'h': 675, 'resize': 'fit'}, " \
              "'small': {'w': 680, 'h': 383, 'resize': 'fit'}, 'large': {'w': 1280, 'h': 720, 'resize': 'fit'}}, " \
              "'source_status_id': 1298458366306660353, 'source_status_id_str': '1298458366306660353', " \
              "'source_user_id': 729676086632656900, 'source_user_id_str': '729676086632656900'}]}, " \
              "'extended_entities': {'media': [{'id': 1298458167211425795, 'id_str': '1298458167211425795', " \
              "'indices': [0, 23], 'media_url': " \
              "'http://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH.jpg', " \
              "'media_url_https': 'https://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH" \
              ".jpg', 'url': 'https://t.co/WjbNIvW96r', 'display_url': 'pic.twitter.com/WjbNIvW96r', 'expanded_url': " \
              "'https://twitter.com/TeamTrump/status/1298458366306660353/video/1', 'type': 'video', 'sizes': {" \
              "'thumb': {'w': 150, 'h': 150, 'resize': 'crop'}, 'medium': {'w': 1200, 'h': 675, 'resize': 'fit'}, " \
              "'small': {'w': 680, 'h': 383, 'resize': 'fit'}, 'large': {'w': 1280, 'h': 720, 'resize': 'fit'}}, " \
              "'source_status_id': 1298458366306660353, 'source_status_id_str': '1298458366306660353', " \
              "'source_user_id': 729676086632656900, 'source_user_id_str': '729676086632656900', 'video_info': {" \
              "'aspect_ratio': [16, 9], 'duration_millis': 54922, 'variants': [{'bitrate': 288000, 'content_type': " \
              "'video/mp4', 'url': 'https://video.twimg.com/amplify_video/1298458167211425795/vid/480x270" \
              "/sXwPBww7AzzE0l9Q.mp4?tag=13'}, {'bitrate': 2176000, 'content_type': 'video/mp4', " \
              "'url': 'https://video.twimg.com/amplify_video/1298458167211425795/vid/1280x720/uNjTWvnP00FREqPM.mp4" \
              "?tag=13'}, {'content_type': 'application/x-mpegURL', 'url': " \
              "'https://video.twimg.com/amplify_video/1298458167211425795/pl/Zjm-_QsgnU-brl_G.m3u8?tag=13'}, " \
              "{'bitrate': 832000, 'content_type': 'video/mp4', 'url': " \
              "'https://video.twimg.com/amplify_video/1298458167211425795/vid/640x360/BBMmBNzsb75nsyC4.mp4?tag=13" \
              "'}]}, 'additional_media_info': {'title': '', 'description': '', 'embeddable': True, 'monetizable': " \
              "False, 'source_user': {'id': 729676086632656900, 'id_str': '729676086632656900', 'name': 'Team Trump (" \
              "Text VOTE to 88022)', 'screen_name': 'TeamTrump', 'location': 'USA', 'description': 'The official " \
              "Twitter account for the Trump Campaign. Together, we will KEEP AMERICA GREAT! 🇺🇸', " \
              "'url': 'https://t.co/mZB2hymxC9', 'entities': {'url': {'urls': [{'url': 'https://t.co/mZB2hymxC9', " \
              "'expanded_url': 'http://www.DonaldJTrump.com', 'display_url': 'DonaldJTrump.com', 'indices': [0, " \
              "23]}]}, 'description': {'urls': []}}, 'protected': False, 'followers_count': 2123084, 'friends_count': " \
              "127, 'listed_count': 4055, 'created_at': 'Mon May 09 14:15:10 +0000 2016', 'favourites_count': 3479, " \
              "'utc_offset': None, 'time_zone': None, 'geo_enabled': True, 'verified': True, 'statuses_count': 25663, " \
              "'lang': None, 'contributors_enabled': False, 'is_translator': False, 'is_translation_enabled': False, " \
              "'profile_background_color': '000000', 'profile_background_image_url': " \
              "'http://abs.twimg.com/images/themes/theme1/bg.png', 'profile_background_image_url_https': " \
              "'https://abs.twimg.com/images/themes/theme1/bg.png', 'profile_background_tile': False, " \
              "'profile_image_url': 'http://pbs.twimg.com/profile_images/745768799849308160/KrZhjkpH_normal.jpg', " \
              "'profile_image_url_https': 'https://pbs.twimg.com/profile_images/745768799849308160/KrZhjkpH_normal" \
              ".jpg', 'profile_banner_url': 'https://pbs.twimg.com/profile_banners/729676086632656900/1588979102', " \
              "'profile_link_color': 'CB0606', 'profile_sidebar_border_color': '000000', " \
              "'profile_sidebar_fill_color': '000000', 'profile_text_color': '000000', " \
              "'profile_use_background_image': False, 'has_extended_profile': False, 'default_profile': False, " \
              "'default_profile_image': False, 'following': False, 'follow_request_sent': False, 'notifications': " \
              "False, 'translator_type': 'none'}}}]}, 'source': '<a href=\"http://twitter.com/download/iphone\" " \
              "rel=\"nofollow\">Twitter for iPhone</a>', 'in_reply_to_status_id': None, 'in_reply_to_status_id_str': " \
              "None, 'in_reply_to_user_id': None, 'in_reply_to_user_id_str': None, 'in_reply_to_screen_name': None, " \
              "'user': {'id': 25073877, 'id_str': '25073877', 'name': 'Donald J. Trump', 'screen_name': " \
              "'realDonaldTrump', 'location': 'Washington, DC', 'description': '45th President of the United States " \
              "of America🇺🇸', 'url': 'https://t.co/OMxB0x7xC5', 'entities': {'url': {'urls': [{'url': " \
              "'https://t.co/OMxB0x7xC5', 'expanded_url': 'http://www.Instagram.com/realDonaldTrump', 'display_url': " \
              "'Instagram.com/realDonaldTrump', 'indices': [0, 23]}]}, 'description': {'urls': []}}, 'protected': " \
              "False, 'followers_count': 85580226, 'friends_count': 50, 'listed_count': 118643, 'created_at': 'Wed " \
              "Mar 18 13:46:38 +0000 2009', 'favourites_count': 4, 'utc_offset': None, 'time_zone': None, " \
              "'geo_enabled': True, 'verified': True, 'statuses_count': 54945, 'lang': None, 'contributors_enabled': " \
              "False, 'is_translator': False, 'is_translation_enabled': True, 'profile_background_color': '6D5C18', " \
              "'profile_background_image_url': 'http://abs.twimg.com/images/themes/theme1/bg.png', " \
              "'profile_background_image_url_https': 'https://abs.twimg.com/images/themes/theme1/bg.png', " \
              "'profile_background_tile': True, 'profile_image_url': " \
              "'http://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal.jpg', " \
              "'profile_image_url_https': 'https://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal" \
              ".jpg', 'profile_banner_url': 'https://pbs.twimg.com/profile_banners/25073877/1595058372', " \
              "'profile_link_color': '1B95E0', 'profile_sidebar_border_color': 'BDDCAD', " \
              "'profile_sidebar_fill_color': 'C5CEC0', 'profile_text_color': '333333', " \
              "'profile_use_background_image': True, 'has_extended_profile': False, 'default_profile': False, " \
              "'default_profile_image': False, 'following': True, 'follow_request_sent': False, 'notifications': " \
              "False, 'translator_type': 'regular'}, 'geo': None, 'coordinates': None, 'place': None, 'contributors': " \
              "None, 'is_quote_status': False, 'retweet_count': 8901, 'favorite_count': 39148, 'favorited': False, " \
              "'retweeted': False, 'possibly_sensitive': False, 'lang': 'und'}, created_at=datetime.datetime(2020, 8, " \
              "26, 3, 22, 48), id=1298460861900652545, id_str='1298460861900652545', " \
              "full_text='https://t.co/WjbNIvW96r', truncated=False, display_text_range=[0, 23], entities={" \
              "'hashtags': [], 'symbols': [], 'user_mentions': [], 'urls': [], 'media': [{'id': 1298458167211425795, " \
              "'id_str': '1298458167211425795', 'indices': [0, 23], 'media_url': " \
              "'http://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH.jpg', " \
              "'media_url_https': 'https://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH" \
              ".jpg', 'url': 'https://t.co/WjbNIvW96r', 'display_url': 'pic.twitter.com/WjbNIvW96r', 'expanded_url': " \
              "'https://twitter.com/TeamTrump/status/1298458366306660353/video/1', 'type': 'photo', 'sizes': {" \
              "'thumb': {'w': 150, 'h': 150, 'resize': 'crop'}, 'medium': {'w': 1200, 'h': 675, 'resize': 'fit'}, " \
              "'small': {'w': 680, 'h': 383, 'resize': 'fit'}, 'large': {'w': 1280, 'h': 720, 'resize': 'fit'}}, " \
              "'source_status_id': 1298458366306660353, 'source_status_id_str': '1298458366306660353', " \
              "'source_user_id': 729676086632656900, 'source_user_id_str': '729676086632656900'}]}, " \
              "extended_entities={'media': [{'id': 1298458167211425795, 'id_str': '1298458167211425795', 'indices': [" \
              "0, 23], 'media_url': 'http://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img" \
              "/Yf5i8sL2TJ9eOurH.jpg', 'media_url_https': " \
              "'https://pbs.twimg.com/amplify_video_thumb/1298458167211425795/img/Yf5i8sL2TJ9eOurH.jpg', " \
              "'url': 'https://t.co/WjbNIvW96r', 'display_url': 'pic.twitter.com/WjbNIvW96r', 'expanded_url': " \
              "'https://twitter.com/TeamTrump/status/1298458366306660353/video/1', 'type': 'video', 'sizes': {" \
              "'thumb': {'w': 150, 'h': 150, 'resize': 'crop'}, 'medium': {'w': 1200, 'h': 675, 'resize': 'fit'}, " \
              "'small': {'w': 680, 'h': 383, 'resize': 'fit'}, 'large': {'w': 1280, 'h': 720, 'resize': 'fit'}}, " \
              "'source_status_id': 1298458366306660353, 'source_status_id_str': '1298458366306660353', " \
              "'source_user_id': 729676086632656900, 'source_user_id_str': '729676086632656900', 'video_info': {" \
              "'aspect_ratio': [16, 9], 'duration_millis': 54922, 'variants': [{'bitrate': 288000, 'content_type': " \
              "'video/mp4', 'url': 'https://video.twimg.com/amplify_video/1298458167211425795/vid/480x270" \
              "/sXwPBww7AzzE0l9Q.mp4?tag=13'}, {'bitrate': 2176000, 'content_type': 'video/mp4', " \
              "'url': 'https://video.twimg.com/amplify_video/1298458167211425795/vid/1280x720/uNjTWvnP00FREqPM.mp4" \
              "?tag=13'}, {'content_type': 'application/x-mpegURL', 'url': " \
              "'https://video.twimg.com/amplify_video/1298458167211425795/pl/Zjm-_QsgnU-brl_G.m3u8?tag=13'}, " \
              "{'bitrate': 832000, 'content_type': 'video/mp4', 'url': " \
              "'https://video.twimg.com/amplify_video/1298458167211425795/vid/640x360/BBMmBNzsb75nsyC4.mp4?tag=13" \
              "'}]}, 'additional_media_info': {'title': '', 'description': '', 'embeddable': True, 'monetizable': " \
              "False, 'source_user': {'id': 729676086632656900, 'id_str': '729676086632656900', 'name': 'Team Trump (" \
              "Text VOTE to 88022)', 'screen_name': 'TeamTrump', 'location': 'USA', 'description': 'The official " \
              "Twitter account for the Trump Campaign. Together, we will KEEP AMERICA GREAT! 🇺🇸', " \
              "'url': 'https://t.co/mZB2hymxC9', 'entities': {'url': {'urls': [{'url': 'https://t.co/mZB2hymxC9', " \
              "'expanded_url': 'http://www.DonaldJTrump.com', 'display_url': 'DonaldJTrump.com', 'indices': [0, " \
              "23]}]}, 'description': {'urls': []}}, 'protected': False, 'followers_count': 2123084, 'friends_count': " \
              "127, 'listed_count': 4055, 'created_at': 'Mon May 09 14:15:10 +0000 2016', 'favourites_count': 3479, " \
              "'utc_offset': None, 'time_zone': None, 'geo_enabled': True, 'verified': True, 'statuses_count': 25663, " \
              "'lang': None, 'contributors_enabled': False, 'is_translator': False, 'is_translation_enabled': False, " \
              "'profile_background_color': '000000', 'profile_background_image_url': " \
              "'http://abs.twimg.com/images/themes/theme1/bg.png', 'profile_background_image_url_https': " \
              "'https://abs.twimg.com/images/themes/theme1/bg.png', 'profile_background_tile': False, " \
              "'profile_image_url': 'http://pbs.twimg.com/profile_images/745768799849308160/KrZhjkpH_normal.jpg', " \
              "'profile_image_url_https': 'https://pbs.twimg.com/profile_images/745768799849308160/KrZhjkpH_normal" \
              ".jpg', 'profile_banner_url': 'https://pbs.twimg.com/profile_banners/729676086632656900/1588979102', " \
              "'profile_link_color': 'CB0606', 'profile_sidebar_border_color': '000000', " \
              "'profile_sidebar_fill_color': '000000', 'profile_text_color': '000000', " \
              "'profile_use_background_image': False, 'has_extended_profile': False, 'default_profile': False, " \
              "'default_profile_image': False, 'following': False, 'follow_request_sent': False, 'notifications': " \
              "False, 'translator_type': 'none'}}}]}, source='Twitter for iPhone', " \
              "source_url='http://twitter.com/download/iphone', in_reply_to_status_id=None, " \
              "in_reply_to_status_id_str=None, in_reply_to_user_id=None, in_reply_to_user_id_str=None, " \
              "in_reply_to_screen_name=None, author=User(_api=<tweepy.api.API object at 0x0000023F470BA0B8>, " \
              "_json={'id': 25073877, 'id_str': '25073877', 'name': 'Donald J. Trump', 'screen_name': " \
              "'realDonaldTrump', 'location': 'Washington, DC', 'description': '45th President of the United States " \
              "of America🇺🇸', 'url': 'https://t.co/OMxB0x7xC5', 'entities': {'url': {'urls': [{'url': " \
              "'https://t.co/OMxB0x7xC5', 'expanded_url': 'http://www.Instagram.com/realDonaldTrump', 'display_url': " \
              "'Instagram.com/realDonaldTrump', 'indices': [0, 23]}]}, 'description': {'urls': []}}, 'protected': " \
              "False, 'followers_count': 85580226, 'friends_count': 50, 'listed_count': 118643, 'created_at': 'Wed " \
              "Mar 18 13:46:38 +0000 2009', 'favourites_count': 4, 'utc_offset': None, 'time_zone': None, " \
              "'geo_enabled': True, 'verified': True, 'statuses_count': 54945, 'lang': None, 'contributors_enabled': " \
              "False, 'is_translator': False, 'is_translation_enabled': True, 'profile_background_color': '6D5C18', " \
              "'profile_background_image_url': 'http://abs.twimg.com/images/themes/theme1/bg.png', " \
              "'profile_background_image_url_https': 'https://abs.twimg.com/images/themes/theme1/bg.png', " \
              "'profile_background_tile': True, 'profile_image_url': " \
              "'http://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal.jpg', " \
              "'profile_image_url_https': 'https://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal" \
              ".jpg', 'profile_banner_url': 'https://pbs.twimg.com/profile_banners/25073877/1595058372', " \
              "'profile_link_color': '1B95E0', 'profile_sidebar_border_color': 'BDDCAD', " \
              "'profile_sidebar_fill_color': 'C5CEC0', 'profile_text_color': '333333', " \
              "'profile_use_background_image': True, 'has_extended_profile': False, 'default_profile': False, " \
              "'default_profile_image': False, 'following': True, 'follow_request_sent': False, 'notifications': " \
              "False, 'translator_type': 'regular'}, id=25073877, id_str='25073877', name='Donald J. Trump', " \
              "screen_name='realDonaldTrump', location='Washington, DC', description='45th President of the United " \
              "States of America🇺🇸', url='https://t.co/OMxB0x7xC5', entities={'url': {'urls': [{'url': " \
              "'https://t.co/OMxB0x7xC5', 'expanded_url': 'http://www.Instagram.com/realDonaldTrump', 'display_url': " \
              "'Instagram.com/realDonaldTrump', 'indices': [0, 23]}]}, 'description': {'urls': []}}, protected=False, " \
              "followers_count=85580226, friends_count=50, listed_count=118643, created_at=datetime.datetime(2009, 3, " \
              "18, 13, 46, 38), favourites_count=4, utc_offset=None, time_zone=None, geo_enabled=True, verified=True, " \
              "statuses_count=54945, lang=None, contributors_enabled=False, is_translator=False, " \
              "is_translation_enabled=True, profile_background_color='6D5C18', " \
              "profile_background_image_url='http://abs.twimg.com/images/themes/theme1/bg.png', " \
              "profile_background_image_url_https='https://abs.twimg.com/images/themes/theme1/bg.png', " \
              "profile_background_tile=True, " \
              "profile_image_url='http://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal.jpg', " \
              "profile_image_url_https='https://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal.jpg', " \
              "profile_banner_url='https://pbs.twimg.com/profile_banners/25073877/1595058372', " \
              "profile_link_color='1B95E0', profile_sidebar_border_color='BDDCAD', " \
              "profile_sidebar_fill_color='C5CEC0', profile_text_color='333333', profile_use_background_image=True, " \
              "has_extended_profile=False, default_profile=False, default_profile_image=False, following=True, " \
              "follow_request_sent=False, notifications=False, translator_type='regular'), user=User(" \
              "_api=<tweepy.api.API object at 0x0000023F470BA0B8>, _json={'id': 25073877, 'id_str': '25073877', " \
              "'name': 'Donald J. Trump', 'screen_name': 'realDonaldTrump', 'location': 'Washington, DC', " \
              "'description': '45th President of the United States of America🇺🇸', 'url': 'https://t.co/OMxB0x7xC5', " \
              "'entities': {'url': {'urls': [{'url': 'https://t.co/OMxB0x7xC5', 'expanded_url': " \
              "'http://www.Instagram.com/realDonaldTrump', 'display_url': 'Instagram.com/realDonaldTrump', " \
              "'indices': [0, 23]}]}, 'description': {'urls': []}}, 'protected': False, 'followers_count': 85580226, " \
              "'friends_count': 50, 'listed_count': 118643, 'created_at': 'Wed Mar 18 13:46:38 +0000 2009', " \
              "'favourites_count': 4, 'utc_offset': None, 'time_zone': None, 'geo_enabled': True, 'verified': True, " \
              "'statuses_count': 54945, 'lang': None, 'contributors_enabled': False, 'is_translator': False, " \
              "'is_translation_enabled': True, 'profile_background_color': '6D5C18', 'profile_background_image_url': " \
              "'http://abs.twimg.com/images/themes/theme1/bg.png', 'profile_background_image_url_https': " \
              "'https://abs.twimg.com/images/themes/theme1/bg.png', 'profile_background_tile': True, " \
              "'profile_image_url': 'http://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal.jpg', " \
              "'profile_image_url_https': 'https://pbs.twimg.com/profile_images/874276197357596672/kUuht00m_normal" \
              ".jpg', 'profile_banner_url': 'https://pbs.twimg.com/profile_banners/25073877/1595058372', " \
              "'profile_link_color': '1B95E0', 'profile_sidebar_border_color': 'BDDCAD', " \
              "'profile_sidebar_fill_color': 'C5CEC0', 'profile_text_color': '333333', " \
              "'profile_use_background_image': True, 'has_extended_profile': False, 'default_profile': False, " \
              "'default_profile_image': False, 'following': True, 'follow_request_sent': False, 'notifications': " \
              "False, 'translator_type': 'regular'}, id=25073877, id_str='25073877', name='Donald J. Trump', " \
              "screen_name='realDonaldTrump', location='Washington, DC', description='45th President of the United " \
              "States of America🇺🇸', url='https://t.co/OMxB0x7xC5', entities={'url': {'urls': [{'url': " \
              "'https://t.co/OMxB0x7xC5', 'expanded_url': 'http://www.Instagram.com/realDonaldTrump', 'display_url': " \
              "'Instagram.com/realDonaldTrump', 'indices': [0, 23]}]}, 'description': {'urls': []}} "

class obj(object):
    def __init__(self, d):
        for a, b in d.items():
            if type(b) == dict:
                if b.get('ignore'):
                    setattr(self, a, b)
                    continue
            if isinstance(b, (list, tuple)):
               setattr(self, a, [obj(x) if isinstance(x, dict) else x for x in b])
            else:
               setattr(self, a, obj(b) if isinstance(b, dict) else b)


class MockTweetsProvider(ITweetsProvider):
    def get_tweets(self, user_id):
        return [obj({
            'user': {
                'screen_name': 'mockuser'
            },
            'full_text': 'Mock tweet',
            'created_at': datetime.now(),
            'id_str': '121212',
            'entities': {'urls': [], 'ignore': True},
            'retweeted': False
        })]
