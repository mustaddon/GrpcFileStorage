# GrpcFileStorage [![NuGet version](https://badge.fury.io/nu/GrpcFileStorage.svg)](http://badge.fury.io/nu/GrpcFileStorage)
gRPC file storage


## Features
* Storing metadata in the database (SQL/NoSQL)
* Storing files in the file system
* Deduplication of files by content
* Distributed storage (multiple disks)


![](https://raw.githubusercontent.com/mustaddon/GrpcFileStorage/main/Examples/diagram.png)


## Example #1: Simple gRPC microservice
```
dotnet new web --name "GrpcService"
cd GrpcService
dotnet add package GrpcFileStorage
dotnet add package DistributedFileStorage.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

program.cs:
```C#
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// add services to the container
builder.Services.AddGrpc();
builder.Services.AddDfsEfc(options =>
{
    // add database provider 
    options.Database.ContextConfigurator = (db) => db.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DfsDatabase;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=False;TrustServerCertificate=False");

    // add path construction algorithm 
    var rnd = new Random();
    options.FileStorage.PathBuilder = (fileId) => Path.GetFullPath($@"_dfs\fake_disk_{rnd.Next(1, 3)}\{DateTime.Now:yyyy\\MM\\dd}\{fileId}");
});

var app = builder.Build();

// map gRPC service to the endpoint
app.MapGrpcFileStorage();

app.Run();
```



## Example #2: Simple console client
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
