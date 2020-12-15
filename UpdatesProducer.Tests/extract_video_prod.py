import json
import sys

from youtube_dl import YoutubeDL


def main(args):
    url = args[0]

    ytdl_options = {
        'format': args[1],
        'quiet': True
    }

    if len(args) > 2:
        ytdl_options = ytdl_options.update({
            'username': args[2],
            'password': args[3]
        })
        
    print('[facebook] 210491343885193: Downloading webpage')

    with YoutubeDL(ytdl_options) as ytdl:
        video_info = ytdl.extract_info(url, download=False)
        print(json.dumps(video_info, indent=2))


if __name__ == "__main__":
    main(sys.argv[1:])
