using DistributedFileStorage;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcFileStorage.Client
{
    public class FileStorageClient<TMetadata> : IDisposable, IDistributedFileStorage<TMetadata>
    {
        public FileStorageClient(string address, FileStorageClientSettings? settings = null)
        {
            _settings = settings ?? new();
            _channel = new Lazy<GrpcChannel>(() => GrpcChannel.ForAddress(address, _settings.GrpcChannel));
        }

        private readonly FileStorageClientSettings _settings;
        private readonly Lazy<GrpcChannel> _channel;

        public Metadata DefaultRequestHeaders => _settings.DefaultRequestHeaders;

        public void Dispose()
        {
            if (_channel.IsValueCreated) _channel.Value.Dispose();
        }

        public async Task<string> Add(IAsyncEnumerator<byte[]> content, string name, TMetadata? metadata = default, CancellationToken cancellationToken = default)
        {
            using var upload = CreateClient().Upload(
                   headers: DefaultRequestHeaders,
                   cancellationToken: cancellationToken);

            // sending file metadata
            await upload.RequestStream.WriteAsync(new FileUploadChunk
            {
                Name = name,
                Metadata = SerializeMetadata(metadata),
            });

            // sending file content
            while (await content.MoveNextAsync())
                await upload.RequestStream.WriteAsync(new FileUploadChunk
                {
                    Content = ByteString.CopyFrom(content.Current)
                });

            await upload.RequestStream.CompleteAsync();

            return (await upload.ResponseAsync).Id;
        }

        public async IAsyncEnumerable<byte[]> GetContent(string id, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var download = CreateClient().Download(
                request: new() { Id = id },
                headers: DefaultRequestHeaders,
                cancellationToken: cancellationToken);

            while (await download.ResponseStream.MoveNext(cancellationToken))
                yield return download.ResponseStream.Current.Content.ToByteArray();
        }

        public async IAsyncEnumerable<IDfsFileInfo<TMetadata>> GetInfos(IAsyncEnumerator<string> ids, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var infos = CreateClient().GetInfo(
                   headers: DefaultRequestHeaders,
                   cancellationToken: cancellationToken);

            _ = Task.Run(async () => {
                while (await ids.MoveNextAsync())
                    await infos.RequestStream.WriteAsync(new FileKey { Id = ids.Current });

                await infos.RequestStream.CompleteAsync();
            }, cancellationToken);

            while (await infos.ResponseStream.MoveNext(cancellationToken))
                yield return Map(infos.ResponseStream.Current);
        }

        public async Task Update(string id, string name, TMetadata? metadata = default, CancellationToken cancellationToken = default)
        {
            await CreateClient().UpdateAsync(
                request: new() { Id = id, Name = name, Metadata = SerializeMetadata(metadata) },
                headers: DefaultRequestHeaders,
                cancellationToken: cancellationToken);
        }

        public async Task Delete(string id, CancellationToken cancellationToken = default)
        {
            await CreateClient().DeleteAsync(
                request: new() { Id = id },
                headers: DefaultRequestHeaders,
                cancellationToken: cancellationToken);
        }



        private Endpoint.EndpointClient CreateClient()
        {
            return new Endpoint.EndpointClient(_channel.Value);
        }

        private string SerializeMetadata(TMetadata? metadata)
        {
            if (metadata == null || metadata is string)
                return (metadata as string) ?? string.Empty;

            return JsonConvert.SerializeObject(metadata, _settings.JsonSerializer) ?? string.Empty;
        }

        private TMetadata? DeserializeMetadata(string? metadata)
        {
            if (string.IsNullOrEmpty(metadata))
                return default;

            if (typeof(TMetadata) == typeof(string))
                return (TMetadata?)(metadata as object);

            try
            {
                return JsonConvert.DeserializeObject<TMetadata?>(metadata, _settings.JsonSerializer);
            }
            catch
            {
                return default;
            }
        }

        private DfsFileInfo<TMetadata> Map(FileInfo info)
        {
            return new(info.Id, info.Name, info.Length, DeserializeMetadata(info.Metadata));
        }
    }
}
