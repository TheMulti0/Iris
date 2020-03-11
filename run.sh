#!/bin/bash

./stop.sh

# docker load < iris-image.tar

HOST_DATA_DIR=`pwd`/Iris
CONTAINER_DATA_DIR=/app/data

echo "Running the docker image for iris-image ..."

set -x

docker container run -dit --restart always \
 -v $HOST_DATA_DIR:$CONTAINER_DATA_DIR \
 --name iris \
 iris-image

set +x