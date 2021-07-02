import json
import sys
from datetime import datetime
from typing import Optional

from facebook_scraper import get_posts, set_proxy


class GetPostsRequest:
    user_id: str
    pages: int
    proxy: Optional[str]
    cookies_filename: Optional[str]

    def __init__(self, new_dict):
        self.__dict__.update(new_dict)


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()


def main(args):
    request = GetPostsRequest(json.loads(args[0]))

    posts = get_facebook_posts(request)

    print(json.dumps(posts, indent=2, default=json_converter))


def get_facebook_posts(request: GetPostsRequest):
    if request.proxy is not None:
        set_proxy(request.proxy)

    posts = get_posts(
        request.user_id,
        pages=request.pages,
        cookies=request.cookies_filename)

    return list(posts)


if __name__ == "__main__":
    main(sys.argv[1:])
