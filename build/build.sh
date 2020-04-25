#!/bin/bash

cp -a ../src src
echo "Copied 'src'"

docker image build -t iris-image .

echo "Done building iris image"