#!/bin/bash

echo "Stopping twitter-scraper"

docker container rm -f twitter-scraper
docker system prune -f