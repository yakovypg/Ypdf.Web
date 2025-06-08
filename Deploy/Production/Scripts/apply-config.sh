#!/bin/bash

apply_all_resources() {
    kubectl apply -f ../namespace.yml

    kubectl apply -f ../tls-secret.yml
    #kubectl apply -f ../prometheus-adapter-tls.yml
    #kubectl apply -f ../prometheus-adapter-serviceaccount.yml
    #kubectl apply -f ../extension-apiserver-authentication-rbac.yml

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

    #kubectl apply -f ../prometheus-deployment.yml
    #kubectl apply -f ../prometheus-service.yml
    kubectl apply -f ../rabbitmq-exporter-deployment.yml
    #kubectl apply -f ../prometheus-adapter-deployment.yml
    #kubectl apply -f ../prometheus-adapter-service.yml

    #kubectl apply -f ../external-metrics.yml

    helm upgrade --install metrics-server metrics-server/metrics-server --namespace kube-system --create-namespace --set args[0]=--kubelet-insecure-tls --set args[1]="--kubelet-preferred-address-types=InternalIP\,Hostname\,ExternalIP"
    helm upgrade --install kube-prom-stack prometheus-community/kube-prometheus-stack --namespace monitoring --create-namespace --set grafana.enabled=true --set prometheusOperator.createCustomResource=true --set prometheusOperator.admissionWebhooks.enabled=false --set prometheusOperator.admissionWebhooks.patch.enabled=false --set prometheusOperator.tls.enabled=false --set prometheus.prometheusSpec.serviceMonitorSelector.matchLabels.release=kube-prom-stack --set prometheus.prometheusSpec.serviceMonitorSelectorNilUsesHelmValues=false -f ../values.yml
    kubectl apply -f ../rabbitmq-exporter-servicemonitor.yml
    helm upgrade --install prometheus-adapter prometheus-community/prometheus-adapter --namespace monitoring -f ../adapter-values.yaml

    kubectl apply -f ../pdf-processing-api-hpa.yml

    kubectl run curlpod -n ypdf --image=curlimages/curl --restart=Never --command -- sleep 3600
    kubectl run curlpod -n monitoring --image=curlimages/curl --restart=Never --command -- sleep 3600
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
