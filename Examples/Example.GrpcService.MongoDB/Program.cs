var builder = WebApplication.CreateBuilder(args);

// add services to the container
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);
builder.Services.AddDfsMongo((services, options) =>
{
    // add database settings 
    options.Database.ConnectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("dfs");

    // add path construction algorithm
    var rnd = new Random();
    options.FileStorage.PathBuilder = (fileId) =>
    {
        var nodes = services.GetRequiredService<IConfiguration>().GetSection("FileStorageNodes").Get<string[]>();
        var randomNode = nodes[rnd.Next(0, nodes.Length)];
        return Path.GetFullPath($@"{randomNode}\{DateTime.Now:yyyy\\MM\\dd}\{fileId}");
    };
});

var app = builder.Build();

// map gRPC service to the endpoint
app.MapGrpcFileStorage();

app.Run();