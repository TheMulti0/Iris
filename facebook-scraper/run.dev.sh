#!/bin/bash

./stop.dev.sh

echo "Running facebook-scraper"

set -x

docker container run -dit --restart always \
 --name facebook-scraper \
 --publish 5001:80 \
 themulti0/facebook-scraper

set +x