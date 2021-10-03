using ConsoleClient;
using GrpcFileStorage.Client;
using Newtonsoft.Json;
using System.Text;


// create client
using var client = new FileStorageClient<MyCustomMetadata>("https://localhost:7272");

// generate file
using var uploadFile = new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", Enumerable.Range(0, 100).Select(i => $"{i} - text text text"))));

// upload
var fileId = await client.Add(uploadFile, "example_file.txt", new MyCustomMetadata { Author = "Test01" });

// read saved info
var fileInfo = await client.GetInfo(fileId);
Console.WriteLine(JsonConvert.SerializeObject(fileInfo, Formatting.Indented));

// download 
using var downloadFile = File.Create("download_example.txt");
await foreach (var chunk in client.GetContent(fileId))
    await downloadFile.WriteAsync(chunk);
