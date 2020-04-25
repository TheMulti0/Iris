#!/bin/bash

cp -a ../../src src

docker image build -t iris-image .

echo "Done building iris image"