#!/bin/bash

set -euo pipefail

kubectl apply -f ../namespace-ypdf.yml
kubectl apply -f ../namespace-monitoring.yml

kubectl apply -f ../pvc-accounts-database.yml
kubectl apply -f ../pvc-db-data.yml
kubectl apply -f ../pvc-intput-files.yml
kubectl apply -f ../pvc-output-file.yml
kubectl apply -f ../pvc-web-app-keys.yml

kubectl apply -f ../secret-tls.yml

kubectl apply -f ../deployment-accounts-database.yml
kubectl apply -f ../deployment-rabbitmq.yml
kubectl apply -f ../deployment-rabbitmq-exporter.yml

kubectl apply -f ../service-accounts-database.yml
kubectl apply -f ../service-rabbitmq.yml
kubectl apply -f ../service-rabbitmq-exporter.yml

kubectl apply -f ../deployment-account-api.yml
kubectl apply -f ../deployment-files-api.yml
kubectl apply -f ../deployment-pdf-processing-api.yml
kubectl apply -f ../deployment-pdf-operations-history-api.yml
kubectl apply -f ../deployment-web-app.yml

kubectl apply -f ../service-account-api.yml
kubectl apply -f ../service-files-api.yml
kubectl apply -f ../service-pdf-processing-api.yml
kubectl apply -f ../service-pdf-operations-history-api.yml
kubectl apply -f ../service-web-app.yml

helm upgrade \
    --install metrics-server metrics-server/metrics-server \
    --namespace kube-system \
    --create-namespace \
    --set args[0]=--kubelet-insecure-tls \
    --set args[1]="--kubelet-preferred-address-types=InternalIP\,Hostname\,ExternalIP"

helm upgrade \
    --install kube-prom-stack prometheus-community/kube-prometheus-stack \
    --namespace monitoring \
    --set grafana.enabled=true \
    --set prometheusOperator.createCustomResource=true \
    --set prometheusOperator.admissionWebhooks.enabled=false \
    --set prometheusOperator.admissionWebhooks.patch.enabled=false \
    --set prometheusOperator.tls.enabled=false \
    --set prometheus.prometheusSpec.serviceMonitorSelector.matchLabels.release=kube-prom-stack \
    --set prometheus.prometheusSpec.serviceMonitorSelectorNilUsesHelmValues=false \
    -f ../values.yml

kubectl apply -f ../servicemonitor-rabbitmq-exporter.yml

helm upgrade \
    --install prometheus-adapter prometheus-community/prometheus-adapter \
    --namespace monitoring \
    -f ../adapter-values.yaml

kubectl apply -f ../hpa-pdf-processing-api.yml
