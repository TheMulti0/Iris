#!/bin/bash

echo "Running facebookscraper"
cd facebookscraper
./run-facebookscraper.sh
cd ..

echo "Running Iris"
cd iris
./run-iris.sh
cd ..