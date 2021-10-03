using ConsoleClient;
using GrpcFileStorage.Client;
using Newtonsoft.Json;


// create client
using var client = new FileStorageClient<MyCustomMetadata>("https://localhost:7272");

// upload
using var uploadFile = Example.GenerateTextFile("text text text", 5000);
var fileId = await client.Add(uploadFile, "example_file.txt", new MyCustomMetadata { Author = "Test01" });

// read saved info
var fileInfo = await client.GetInfo(fileId);
Console.WriteLine(JsonConvert.SerializeObject(fileInfo, Formatting.Indented));

// download 
using var downloadFile = File.Create("download_example.txt");
await foreach (var chunk in client.GetContent(fileId))
    await downloadFile.WriteAsync(chunk);
