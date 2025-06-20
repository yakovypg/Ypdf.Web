#!/bin/bash

set -euo pipefail
kubectl get --raw "/apis/external.metrics.k8s.io/v1beta1/namespaces/ypdf/rabbitmq_queue_messages"
