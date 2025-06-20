#!/bin/bash

set -euo pipefail

docker-compose -f docker-compose-production.yml --env-file .env build
docker-compose -f docker-compose-production.yml --env-file .env up -d
