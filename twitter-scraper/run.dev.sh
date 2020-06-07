#!/bin/bash

./stop.dev.sh

echo "Running twitter-scraper"

set -x

docker container run -dit --restart always \
 --name twitter-scraper \
 --publish 5000:80 \
 themulti0/twitter-scraper

set +x