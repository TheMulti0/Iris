#!/bin/bash

echo "Stopping facebook-scraper"

docker container rm -f facebook-scraper
docker system prune -f