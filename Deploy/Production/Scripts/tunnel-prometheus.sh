#!/bin/bash

set -euo pipefail
kubectl -n monitoring port-forward svc/kube-prom-stack-kube-prome-prometheus 9090:9090
