#!/bin/bash

echo "Starting ksqlDB Init... Waiting for Topics Init"
while [ $(curl -s -o /dev/null -w %{http_code} http://rest-proxy:8082/topics/capability_topic) -ne 200 ]
do 
    echo -e $(date) "Waiting for last Topic to exist"
    sleep 5
done

echo "Starting ksqlDB Init... Waiting for Schema Init"
while [ $(curl -s -o /dev/null -w %{http_code} http://schema-registry:8081/subjects/capability_topic-value/versions/latest/schema) -ne 200 ]
do 
    echo -e $(date) "Waiting for last Schema to exist"
    sleep 5
done

echo "Running KSQL"
cat /data/scripts/ksqldb-init.ksql <(echo "DONE")| ksql http://ksqldb-server:8088