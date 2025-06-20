#!/bin/bash

set -euo pipefail

days='365'
name='localhost'
subject='localhost'

key_file_name="${name}.key"
crt_file_name="${name}.crt"
base64_key_file_name="${key_file_name}.base64"
base64_crt_file_name="${crt_file_name}.base64"

openssl req \
    -x509 \
    -nodes \
    -days $days \
    -newkey rsa:2048 \
    -keyout $key_file_name \
    -out $crt_file_name \
    -subj "/CN=${subject}"

base64 -w 0 $key_file_name > $base64_key_file_name
base64 -w 0 $crt_file_name > $base64_crt_file_name
