#!/bin/bash

echo "Build facebookscraper"
cd facebookscraper
./build-facebookscraper.sh
cd ..

echo "Build Iris"
cd iris
./build-iris.sh
cd ..