from flask import Flask, request, jsonify
from facebook_scraper import get_posts

app = Flask(__name__)


@app.route('/facebook', methods=['GET'])
def get_facebook_posts():
    name = request.args['name']
    pages = int(request.args['pages'])

    posts = list(
        get_posts(
            name,
            pages=pages))

    return jsonify(posts)


if __name__ == '__main__':
    app.run()
