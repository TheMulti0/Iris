from flask import Flask, request, jsonify
from facebook_scraper import get_posts

app = Flask(__name__)


@app.route('/facebook', methods=['GET'])
def get_facebook_posts():
    name = request.args['name']
    pageCount = int(request.args['pageCount'])

    posts = list(
        get_posts(
            name,
            pages=pageCount))

    return jsonify(posts)


if __name__ == '__main__':
    app.run()
