#!/bin/bash

kubectl get apiservices | grep metrics
kubectl get --raw "/apis/external.metrics.k8s.io/v1beta1/namespaces/ypdf/rabbitmq_queue_messages"
kubectl get hpa hpa-pdf-processing-api -n ypdf

# kubectl -n monitoring port-forward svc/kube-prom-stack-kube-prome-prometheus 9090:9090
# minikube tunnel
