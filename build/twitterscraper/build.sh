#!/bin/bash

./clean.sh

cp -a ../../src/twitterscraper twitterscraper
echo "Copied 'twitterscraper'"

docker image build -t twitterscraper-image .

echo "Done building twitterscraper image"