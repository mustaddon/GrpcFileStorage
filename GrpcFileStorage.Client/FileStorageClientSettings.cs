using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;

namespace GrpcFileStorage.Client
{
    public class FileStorageClientSettings
    {
        public GrpcChannelOptions GrpcChannel { get; set; } = new();

        public Metadata DefaultRequestHeaders { get; set; } = new();

        public JsonSerializerSettings JsonSerializer { get; set; } = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
        };

    }
}
