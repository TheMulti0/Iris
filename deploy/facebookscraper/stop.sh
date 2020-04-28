#!/bin/bash

echo "Stopping facebookscraper container ..."

docker container rm -f facebookscraper
docker system prune -f