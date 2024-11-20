# GrpcFileStorage.Client [![NuGet version](https://badge.fury.io/nu/GrpcFileStorage.Client.svg)](http://badge.fury.io/nu/GrpcFileStorage.Client)
gRPC file storage client



## Example
```
dotnet new console --name "ConsoleClient"
cd ConsoleClient
dotnet add package GrpcFileStorage.Client
```

program.cs:
```C#
using GrpcFileStorage.Client;


// create client
using var client = new FileStorageClient<string>("https://localhost:7272");

// upload
using var uploadFile = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test text"));
var fileId = await client.Add(uploadFile, $"example.txt", "my metadata");

// get info
var fileInfo = await client.GetInfo(fileId);
Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(fileInfo));

// download 
using var downloadFile = File.Create("example.txt");
await foreach (var chunk in client.GetContent(fileId))
    await downloadFile.WriteAsync(chunk);
```


[More examples...](https://github.com/mustaddon/GrpcFileStorage/tree/main/Examples/)
