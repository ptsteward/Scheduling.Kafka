#!/usr/bin/env bash
# set -x

echo -e "\nAdding Pre-Reqs"
apk add curl

echo -e "\nStarting Schema Init... Waiting for Schema Registry"
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
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/requirement.proto" -s "protos%2Fscheduling%2Frequirement.proto" -r "/data/scripts/json/requirement-refs.json"
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/capability.proto" -s "protos%2Fscheduling%2Fcapability.proto" -r "/data/scripts/json/capability-refs.json"
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/resource.proto" -s "protos%2Fscheduling%2Fresource.proto" -r "/data/scripts/json/resource-refs.json"

echo -e "\nTopic Schemas"
bash /data/scripts/post-schema.sh -p "/data/protos/scheduling/resource.proto" -s "resources-value" -r "/data/scripts/json/resource-refs.json"

echo -e "\nAll Subjects"
curl -silent -X GET "http://schema-registry:8081/subjects"