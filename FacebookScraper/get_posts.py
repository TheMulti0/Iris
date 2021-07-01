import json
import sys
from datetime import datetime

from facebook_scraper import get_posts, set_proxy


def main(args):
    user_id = args[0]
    pages = int(args[1])
    proxy = args[2]

    if proxy != '':
        set_proxy(proxy)

    posts = get_facebook_posts(user_id, pages)

    print(json.dumps(posts, indent=2, default=json_converter))


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()


def get_facebook_posts(user_id, pages):
    return list(get_posts(
        user_id,
        pages=pages
    ))


if __name__ == "__main__":
    main(sys.argv[1:])
