#!/bin/bash

./stop.sh

echo "Stopped facebookscraper"

echo "Running the docker image for facebookscraper-image ..."

set -x

docker container run \
 --name facebookscraper \
 --publish 5000:5000 \
 facebookscraper-image

set +x