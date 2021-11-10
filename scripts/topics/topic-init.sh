#!/bin/bash
# set -x

echo -e "\nStarting Topic Init... Waiting for Rest Proxy"
while [ $(curl -s -o /dev/null -w %{http_code} http://rest-proxy:8082/topics) -ne 200 ]
do 
    echo -e $(date) "Waiting for Rest Proxy 200"
    sleep 5
done  

echo -e "\nCreating topics"

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic test
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic test --replication-factor 3 --partitions 2

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic capability_topic
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic capability_topic --replication-factor 3 --partitions 10

kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --delete --if-exists --topic resources
kafka-topics --bootstrap-server broker1:29092,broker2:29093,broker3:29094 --create --if-not-exists --topic resources --replication-factor 3 --partitions 10

echo -e "\nAll Topics"
kafka-topics --bootstrap-server broker1:29092 --list