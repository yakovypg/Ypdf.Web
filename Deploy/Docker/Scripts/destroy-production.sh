#!/bin/bash

set -euo pipefail
docker compose -f ../docker-compose-production.yml down
