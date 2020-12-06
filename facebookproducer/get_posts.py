import sys, json
from datetime import datetime

from facebook_scraper import get_posts


def main(args):
    user_id = args[0]
    pages = int(args[1])

    if len(args) > 2:
        username = args[2]
        password = args[3]
        posts = get_facebook_posts(user_id, pages, (username, password))
    else:
        posts = get_facebook_posts(user_id, pages)

    print(json.dumps(posts, indent=2, default=json_converter))


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()


def get_facebook_posts(user_id, pages, credentials=None):
    return list(get_posts(
        user_id,
        pages=pages,
        credentials=credentials
    ))


if __name__ == "__main__":
    main(sys.argv[1:])
