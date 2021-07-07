import json
import sys
from datetime import datetime
from typing import Optional

from youtube_dl import YoutubeDL


class ExtractVideoRequest:
    url: str
    format: str
    username: Optional[str]
    password: Optional[str]

    def __init__(self, new_dict):
        self.__dict__.update(new_dict)


class ExtractVideoResponse:
    video_info: object
    error: str
    error_description: str


def json_converter(obj):
    if isinstance(obj, datetime):
        return obj.__str__()
    if isinstance(obj, ExtractVideoResponse):
        return obj.__dict__


def main(args):
    request = ExtractVideoRequest(json.loads(args[0]))
    response = ExtractVideoResponse()

    try:
        response.video_info = extract_video(request)
    except Exception as e:
        response.error = type(e).__name__
        response.error_description = str(e)

    print(
        json.dumps(response, indent=2, default=json_converter))


def extract_video(request: ExtractVideoRequest):
    ytdl_options = {
        'format': request.format,
        'quiet': True
    }
    if request.username is not None:
        ytdl_options.update({
            'username': request.username
        })
    if request.password is not None:
        ytdl_options.update({
            'password': request.password
        })

    with YoutubeDL(ytdl_options) as ytdl:
        return ytdl.extract_info(
            url=request.url,
            download=False)


if __name__ == "__main__":
    main(sys.argv[1:])
