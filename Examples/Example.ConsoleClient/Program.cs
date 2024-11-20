using ConsoleClient;
using GrpcFileStorage.Client;
using Newtonsoft.Json;

// create client
using var client = new FileStorageClient<MyCustomMetadata>("https://localhost:7272");

// upload
using var uploadFile = Example.GenerateTextFileStream();
var fileId = await client.Add(uploadFile, $"example.txt", new MyCustomMetadata { Author = "User" });

// get info
var fileInfo = await client.GetInfo(fileId);
Console.WriteLine(JsonConvert.SerializeObject(fileInfo, Formatting.Indented));

// download 
using var downloadFile = File.Create("example.txt");
await foreach (var chunk in client.GetContent(fileId))
    await downloadFile.WriteAsync(chunk);
