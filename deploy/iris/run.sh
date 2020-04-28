#!/bin/bash

./stop.sh

# docker load < iris-image.tar

HOST_CONFIG_DIR=`pwd`/config
CONTAINER_CONFIG_DIR=/app/config

HOST_LOGS_DIR=`pwd`/logs
CONTAINER_LOGS_DIR=/app/logs

echo "Running the docker image for iris-image ..."

set -x

docker container run -dit --restart always \
 --network="host" \
 -v $HOST_CONFIG_DIR:$CONTAINER_CONFIG_DIR \
 -v $HOST_LOGS_DIR:$CONTAINER_LOGS_DIR \
 --name iris \
 iris-image

set +x
