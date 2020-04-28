#!/bin/bash

echo "Stopping twitterscraper container ..."

docker container rm -f twitterscraper
docker system prune -f