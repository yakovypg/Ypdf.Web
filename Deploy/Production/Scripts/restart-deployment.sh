#!/bin/bash

restart_all_deployments() {
    kubectl rollout restart deployment accounts-database -n ypdf
    kubectl rollout restart deployment rabbitmq -n ypdf

    kubectl rollout restart deployment account-api -n ypdf
    kubectl rollout restart deployment files-api -n ypdf
    kubectl rollout restart deployment pdf-processing-api -n ypdf
    kubectl rollout restart deployment pdf-operations-history-api -n ypdf
    kubectl rollout restart deployment web-app -n ypdf
}

name=$1
all_deployments="all"

if [[ -z $name ]]; then
    name=$all_deployments
fi

if [[ $name == $all_deployments ]]; then
    restart_all_deployments
else
    kubectl rollout restart deployment $name -n ypdf
fi
