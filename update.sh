#!/bin/bash

git checkout production

git pull

./build.sh

./run.sh