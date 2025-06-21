#!/bin/bash

set -euo pipefail

docker compose -f ../docker-compose-development.yml --env-file ../.env build
docker compose -f ../docker-compose-development.yml --env-file ../.env up -d
