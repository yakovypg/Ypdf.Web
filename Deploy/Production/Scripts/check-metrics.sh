#!/bin/bash

set -euo pipefail
kubectl get apiservices | grep metrics
