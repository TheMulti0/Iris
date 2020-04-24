#!/bin/bash

echo "Stopping iris container ..."

docker container rm -f iris
docker system prune -f