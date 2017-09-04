#!/bin/bash
docker run --rm \
  --interactive \
  --tty \
  --volume $(pwd)/OAuthTest:/var/www/default \
  --volume $(pwd)/default:/usr/jexus/siteconf/default:ro \
  --publish 8088:80 \
  --name jexus-test \
  beginor/jexus:5.8.2.21
