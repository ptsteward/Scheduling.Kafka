#!/usr/bin/env bash
# set -x

echo "Adding Pre-Reqs"
apk add curl

echo "Starting Schema Init... Waiting for Schema Registry"
while [ $(curl -s -o /dev/null -w %{http_code} http://schema-registry:8081/subjects) -ne 200 ]
do 
    echo -e $(date) "Waiting for Schema Registry 200"
    sleep 5
done

echo -e "\nWell Known"
bash /data/scripts/post-schema.sh -p "/data/protos/well-known/duration.proto" -s "google%2Fprotobuf%2Fduration.proto"
bash /data/scripts/post-schema.sh -p "/data/protos/well-known/timestamp.proto" -s "google%2Fprotobuf%2Ftimestamp.proto"
bash /data/scripts/post-schema.sh -p "/data/protos/well-known/wrappers.proto" -s "google%2Fprotobuf%2Fwrappers.proto"

echo -e "\nScheduling"
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/core.proto" -s "protos%2Fscheduling%2Fcore.proto" -r "/data/scripts/json/core-refs.json"
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/fundamental_units.proto" -s "protos%2Fscheduling%2Ffundamental_units.proto" -r "/data/scripts/json/fundamental_units-refs.json"

echo -e "\nTopics"
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/fundamental_units.proto" -s "capability_topic-value" -r "/data/scripts/json/fundamental_units-refs.json"

echo -e "\nAll Subjects"
curl -silent -X GET "http://schema-registry:8081/subjects"