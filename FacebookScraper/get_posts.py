import sys, json
from datetime import datetime

from facebook_scraper import get_posts


def main(args):
    user_id = args[0]
    pages = int(args[1])
    
    posts = get_facebook_posts(user_id, pages)

    print(json.dumps(posts, indent=2, default=json_converter))


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()


def get_facebook_posts(user_id, pages):
    return list(get_posts(
        user_id,
        #cookies='cookies.txt',
        pages=pages
    ))


if __name__ == "__main__":
    main(sys.argv[1:])