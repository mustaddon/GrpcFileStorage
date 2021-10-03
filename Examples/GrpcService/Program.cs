using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDfsEfc((s, o) =>
{
    var connectionString = s.GetRequiredService<IConfiguration>().GetConnectionString("dfs");
    o.DbContextConfigurator = x => x.UseSqlServer(connectionString);

    var rnd = new Random();
    o.FileStorage.PathGenerator = (fileId) => Path.GetFullPath($@"_tmp\fake_disk_{rnd.Next(1, 3)}\{DateTime.Now.ToString(@"yyyy\\MM\\dd")}\{fileId}");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGrpcFileStorage();

app.Run();