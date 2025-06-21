#!/bin/bash

set -euo pipefail

kubectl delete all --all -n monitoring
kubectl delete all --all -n ypdf
kubectl delete pvc --all -n ypdf
