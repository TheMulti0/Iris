#!/bin/bash

cp -a ../../src/facebookscraper/. facebookscraper

docker image build -t facebookscraper-image .

echo "Done building facebookscraper image"