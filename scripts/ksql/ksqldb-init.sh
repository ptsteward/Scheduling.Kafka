#!/bin/bash

echo -e "\nStarting ksqlDB Init... Waiting for Topics Init"
while [ $(curl -s -o /dev/null -w %{http_code} http://rest-proxy:8082/topics/resources) -ne 200 ]
do 
    echo -e $(date) "Waiting for last Topic to exist"
    sleep 5
done

echo -e "\nStarting ksqlDB Init... Waiting for Schema Init"
while [ $(curl -s -o /dev/null -w %{http_code} http://schema-registry:8081/subjects/resources-value/versions/latest/schema) -ne 200 ]
do 
    echo -e $(date) "Waiting for last Schema to exist"
    sleep 5
done

echo -e "\nRunning KSQL"
#cat /data/scripts/types.ksql <(echo "DONE")| ksql http://ksqldb-server:8088
#cat /data/scripts/resources.ksql <(echo "DONE")| ksql http://ksqldb-server:8088