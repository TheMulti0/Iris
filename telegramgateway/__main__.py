import asyncio
import json
import jsonpickle

from pyrogram import Client
from quart import Quart, request, Response

from send_media_request import SendMediaRequest


class MyResponse(Response):
    default_mimetype = 'application/json'


app = Quart(__name__)
app.response_class  = MyResponse

pyro = Client(__name__)


@jsonpickle.handlers.register(Client, base=True)
class ClientHandler(jsonpickle.handlers.BaseHandler):
    def restore(self, obj):
        pass

    def flatten(self, obj, data):
        return ''


@app.route('/send/media')
async def send_media_group():
    async with pyro:
        r = SendMediaRequest(
            jsonpickle.decode(
                request.args.get('request')))

        messages = await pyro.send_media_group(**r.__dict__)

        return jsonpickle.encode(messages, unpicklable=False)


loop = asyncio.get_event_loop()
loop.run_until_complete(app.run_task())
