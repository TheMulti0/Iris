from flask import Flask, request, jsonify
from twitter_scraper import get_tweets

app = Flask(__name__)


@app.route('/tweets', methods=['GET'])
def get_twitter_tweets():
    name = request.args['name']
    page_count = int(request.args['page_count'])

    posts = list(
        get_tweets(
            name,
            pages=page_count))

    return jsonify(posts)


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)
