#!/bin/bash

set -euo pipefail

name=$1
namespace=$2

if [[ -z $name ]]; then
    echo 'error: deployment name not specified'
    exit 1
fi

if [[ -z $namespace ]]; then
    namespace='ypdf'
fi

kubectl rollout restart deployment $name -n $namespace
