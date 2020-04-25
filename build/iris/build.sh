#!/bin/bash

./clean.sh

cp -a ../../src src
echo "Copied 'src'"

docker image build -t iris-image .

echo "Done building iris image"