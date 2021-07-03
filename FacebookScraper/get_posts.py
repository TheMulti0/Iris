﻿import json
import sys
from datetime import datetime
from typing import Optional

from facebook_scraper import get_posts, set_proxy
from requests.exceptions import ProxyError


class GetPostsRequest:
    user_id: str
    pages: int
    proxy: Optional[str]
    cookies_filename: Optional[str]

    def __init__(self, new_dict):
        self.__dict__.update(new_dict)


class GetPostsResponse:
    posts: []
    error: str
    error_description: str


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()
    if isinstance(obj, GetPostsResponse):
        return obj.__dict__


def main(args):
    request = GetPostsRequest(json.loads(args[0]))
    response = GetPostsResponse()

    try:
        response.posts = get_facebook_posts(request)
    except ProxyError as e:
        response.error = 'ProxyError'
        response.error_description = str(e) 
    except Exception as e:
        exc_type, exc_obj, exc_tb = sys.exc_info()
        response.error = exc_type
        response.error_description = str(e)
        #raise e

    print(json.dumps(response, indent=2, default=json_converter))


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
