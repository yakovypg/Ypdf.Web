#!/bin/bash

kubectl delete deployments --all --namespace=ypdf
kubectl delete statefulsets --all --namespace=ypdf
kubectl delete replicasets --all --namespace=ypdf
kubectl delete pods --all --namespace=ypdf
kubectl delete services --all --namespace=ypdf
kubectl delete hpa --all --namespace=ypdf
kubectl delete configmap --all --namespace=ypdf
