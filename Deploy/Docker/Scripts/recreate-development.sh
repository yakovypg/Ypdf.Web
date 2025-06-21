#!/bin/bash

set -euo pipefail
docker compose -f ../docker-compose-development.yml --env-file ../.env up --build --force-recreate --no-deps -d
