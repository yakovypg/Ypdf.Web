#!/bin/bash

set -euo pipefail
kubectl get hpa hpa-pdf-processing-api -n ypdf
