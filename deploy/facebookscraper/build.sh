#!/bin/bash

./clean.sh

cp -a ../../src/facebookscraper facebookscraper
echo "Copied 'facebookscraper'"

docker image build -t facebookscraper-image .

echo "Done building facebookscraper image"