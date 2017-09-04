#!/bin/bash
docker run --rm \
  --interactive \
  --tty \
  --volume $(pwd)/OAuthTest:/app \
  --publish 8088:80 \
  --name mono-test \
  beginor/mono-runtime:5.2.0
