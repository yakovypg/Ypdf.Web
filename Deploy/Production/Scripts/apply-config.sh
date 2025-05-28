#!/bin/bash

apply_all_resources() {
    kubectl apply -f ../namespace.yml

    kubectl apply -f ../tls-secret.yml
    kubectl apply -f ../accounts-database-pvc.yml
    kubectl apply -f ../db-data-pvc.yml
    kubectl apply -f ../intput-files-pvc.yml
    kubectl apply -f ../output-files-pvc.yml
    kubectl apply -f ../web-app-keys-pvc.yml

    kubectl apply -f ../accounts-database-deployment.yml
    kubectl apply -f ../accounts-database-service.yml
    kubectl apply -f ../rabbitmq-deployment.yml
    kubectl apply -f ../rabbitmq-service.yml

    kubectl apply -f ../account-api-deployment.yml
    kubectl apply -f ../files-api-deployment.yml
    kubectl apply -f ../pdf-processing-api-deployment.yml
    kubectl apply -f ../pdf-operations-history-api-deployment.yml
    kubectl apply -f ../web-app-deployment.yml

    kubectl apply -f ../account-api-service.yml
    kubectl apply -f ../files-api-service.yml
    kubectl apply -f ../pdf-processing-api-service.yml
    kubectl apply -f ../pdf-operations-history-api-service.yml
    kubectl apply -f ../web-app-service.yml

    kubectl apply -f ../pdf-processing-api-hpa.yml
}

name=$1
all_configs="all"

if [[ -z $name ]]; then
    name=$all_configs
fi

if [[ $name == $all_configs ]]; then
    apply_all_resources
else
    kubectl apply -f "../${name}"
fi
