#!/bin/bash

./stop.sh

echo "Stopped twitterscraper"

echo "Running the docker image for twitterscraper-image ..."

set -x

docker container run \
 --name twitterscraper \
 --publish 5001:5000 \
 twitterscraper-image

set +x