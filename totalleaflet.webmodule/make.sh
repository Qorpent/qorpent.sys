#!/bin/sh
TASK=$1
if [[ "$TASK" == "" ]]; then
    TASK="build"
fi
./build/${TASK} ${*:2}