from typing import Union, List

from pyrogram import types


class AttrDict(dict):
    def __init__(self, *args, **kwargs):
        super(AttrDict, self).__init__(*args, **kwargs)
        self.__dict__ = self


class SendMediaRequest:
    chat_id: Union[int, str]
    photos: List[dict] = []
    videos: List[dict] = []
    media: List[Union[
        "types.InputMediaPhoto",
        "types.InputMediaVideo",
        "types.InputMediaAudio",
        "types.InputMediaDocument"
    ]]
    disable_notification: bool
    reply_to_message_id: int
    schedule_date: int

    def __init__(self, new_dict):
        self.__dict__ = new_dict

        self.media = list(self.create_media())

        delattr(self, 'photos')
        delattr(self, 'videos')

    def create_media(self):
        if self.photos is not None:
            for p in self.photos:
                yield types.InputMediaPhoto(**p)
        if self.videos is not None:
            for v in self.videos:
                yield AttrDict(v.items())
