dotnet build -c Release 
dotnet pack .\GrpcFileStorage\ -c Release -o ..\_publish
dotnet pack .\GrpcFileStorage.Client\ -c Release -o ..\_publish
