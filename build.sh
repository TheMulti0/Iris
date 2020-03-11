#!/bin/bash

docker image build -t iris-image .

echo "Done building image"

# docker image save iris-image > iris-image.tar

echo "Saved image to tar"