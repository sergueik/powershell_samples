cd /d %~dp0
..\..\packages\Google.Protobuf.3.0.0-alpha4\tools\protoc.exe -I./ --csharp_out ..\ServiceDefinition --grpc_out ..\ServiceDefinition --plugin=protoc-gen-grpc=..\..\packages\Grpc.Tools.0.7.1\tools\grpc_csharp_plugin.exe playground_service.proto