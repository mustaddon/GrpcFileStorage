using GrpcFileStorage.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Test.Client
{
    internal class App
    {
        public static Lazy<IHost> Instance = new Lazy<IHost>(static () =>
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped(x => new FileStorageClient<TestMetadata>("https://localhost:7272"));
                });

            return builder.Build();
        });
    }

}
