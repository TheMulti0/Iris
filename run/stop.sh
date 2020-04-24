#!/bin/bash

echo "Stopping facebookscraper"
cd facebookscraper
./stop-facebookscraper.sh
cd ..

echo "Stopping Iris"
cd iris
./stop-iris.sh
cd ..