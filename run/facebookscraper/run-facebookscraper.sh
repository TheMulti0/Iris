#!/bin/bash

./stop-facebookscraper.sh

echo "Stopped facebookscraper"

echo "Running the docker image for facebookscraper-image ..."

set -x

docker container run --restart always \
 --name facebookscraper \
 --publish 5000:5000 \
 facebookscraper-image

set +x