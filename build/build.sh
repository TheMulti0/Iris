#!/bin/bash

rm -rf src
echo "Removed 'src'"

cp -a ../src src
echo "Copied 'src'"

docker image build -t iris-image .

echo "Done building iris image"