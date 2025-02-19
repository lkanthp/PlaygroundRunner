#!/usr/bin/env bash
scriptdir=`dirname "$0"`

bash "${scriptdir}/download_binary.sh"

plugin="${scriptdir}/contract_csharp_plugin"

solutiondir=`dirname ${scriptdir}`

# The first argument to the script is the directory of the .proto file
proto_file_dir=$1

# Get the parent directory of the .proto file
proto_files_dir=`dirname ${proto_file_dir}`

protoc --proto_path=${proto_files_dir} \
--csharp_out="$2":./Protobuf/Generated \
--csharp_opt=file_extension=.g.cs \
--contract_opt="$2",nocontract \
--contract_out=./Protobuf/Generated \
--plugin=protoc-gen-contract=${plugin} \
$3