#!/bin/bash

set -euo pipefail

echo Prometheus will be available at http://localhost:9090
kubectl -n monitoring port-forward svc/kube-prom-stack-kube-prome-prometheus 9090:9090
