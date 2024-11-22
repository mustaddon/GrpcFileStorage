using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// add services to the container
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);
builder.Services.AddDfsEfc(options =>
{
    // add database provider 
    options.Database.ContextConfigurator = (db) => db.UseSqlServer(builder.Configuration.GetConnectionString("dfs"));

    // add path construction algorithm
    var rnd = new Random();
    options.FileStorage.PathBuilder = (fileId) =>
    {
        var nodes = builder.Configuration.GetSection("FileStorageNodes").Get<string[]>();
        var randomNode = nodes?.Length > 0 ? nodes[rnd.Next(0, nodes.Length)] : ".";
        return Path.GetFullPath($@"{randomNode}\{DateTime.Now:yyyy\\MM\\dd}\{fileId}");
    };
});

var app = builder.Build();

// map gRPC service to the endpoint
app.MapGrpcFileStorage();

app.Run();