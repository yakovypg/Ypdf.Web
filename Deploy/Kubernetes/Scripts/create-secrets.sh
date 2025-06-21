#!/bin/bash

set -euo pipefail

set -a
source ../.env
set +a

kubectl create secret tls secret-tls \
    -n ypdf \
    --cert ../../../Certificates/localhost.crt \
    --key ../../../Certificates/localhost.key

kubectl create secret generic secret-rabbitmq \
    -n ypdf \
    --from-literal=RABBITMQ_USER="$RABBITMQ_USER" \
    --from-literal=RABBITMQ_PASSWORD="$RABBITMQ_PASSWORD"

kubectl create secret generic secret-accounts-database \
    -n ypdf \
    --from-literal=ACCOUNTS_DATABASE_USER="$ACCOUNTS_DATABASE_USER" \
    --from-literal=ACCOUNTS_DATABASE_PASSWORD="$ACCOUNTS_DATABASE_PASSWORD" \
    --from-literal=ACCOUNTS_DATABASE_CONNECTION_STRING="Host=accounts-database;Port=5432;Database=AccountsDB;Username=${ACCOUNTS_DATABASE_USER};Password=${ACCOUNTS_DATABASE_PASSWORD}"

kubectl create secret generic secret-jwt \
    -n ypdf \
    --from-literal=JWT_KEY="$JWT_KEY"

kubectl create secret generic secret-initial-users \
    -n ypdf \
    --from-literal=ADMIN_USER_USERNAME="$ADMIN_USER_USERNAME" \
    --from-literal=ADMIN_USER_NICKNAME="$ADMIN_USER_NICKNAME" \
    --from-literal=ADMIN_USER_EMAIL="$ADMIN_USER_EMAIL" \
    --from-literal=ADMIN_USER_PASSWORD="$ADMIN_USER_PASSWORD" \
    --from-literal=TEST_USER_USERNAME="$TEST_USER_USERNAME" \
    --from-literal=TEST_USER_NICKNAME="$TEST_USER_NICKNAME" \
    --from-literal=TEST_USER_EMAIL="$TEST_USER_EMAIL" \
    --from-literal=TEST_USER_PASSWORD="$TEST_USER_PASSWORD"
