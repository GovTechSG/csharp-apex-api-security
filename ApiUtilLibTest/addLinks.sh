#!/bin/bash

# dependency: must clone test-suites-apex-api-security
#             at the save level as csharp-apex2-net5-skd

CREATE_LINK=true

cd bin/Debug
mkdir -p linkFolder
cd linkFolder

FOLDER=../../../../../test-suites-apex-api-security

FOLDER_NAME=testData
[ -L "${FOLDER_NAME}" ] && unlink ${FOLDER_NAME} && echo "Unlink folder - ${FOLDER_NAME}"
if [ ${CREATE_LINK} = "true" ]; then ln -s ${FOLDER}/${FOLDER_NAME} ${FOLDER_NAME} && echo ">>>Add link folder - ${FOLDER_NAME}"; fi

FOLDER_NAME=cert
[ -L "${FOLDER_NAME}" ] && unlink ${FOLDER_NAME} && echo "Unlink folder - ${FOLDER_NAME}"
if [ ${CREATE_LINK} = "true" ]; then ln -s ${FOLDER}/${FOLDER_NAME} ${FOLDER_NAME} && echo ">>>Add link folder - ${FOLDER_NAME}"; fi

cd ../../../