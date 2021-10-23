using DistributedFileStorage;
using Google.Protobuf;
using Grpc.Core;

namespace GrpcFileStorage
{
    public class FileStorageService : Endpoint.EndpointBase
    {
        public FileStorageService(IDistributedFileStorage<string> fileStorage)
        {
            _fileStorage = fileStorage;
        }

        private readonly IDistributedFileStorage<string> _fileStorage;

        public override async Task<FileKey> Upload(IAsyncStreamReader<FileUploadChunk> requestStream, ServerCallContext context)
        {
            await requestStream.MoveNext();

            var id = await _fileStorage.Add(
                content: GetContent(requestStream, context.CancellationToken),
                name: requestStream.Current.Name,
                metadata: requestStream.Current.Metadata,
                cancellationToken: context.CancellationToken);

            return new FileKey { Id = id };
        }

        public override async Task Download(FileKey request, IServerStreamWriter<FileChunk> responseStream, ServerCallContext context)
        {
            await foreach (var chunk in _fileStorage.GetContent(request.Id, context.CancellationToken))
                await responseStream.WriteAsync(new FileChunk
                {
                    Content = ByteString.CopyFrom(chunk, 0, chunk.Length),
                });
        }

        public override async Task GetInfo(IAsyncStreamReader<FileKey> requestStream, IServerStreamWriter<FileInfo> responseStream, ServerCallContext context)
        {
            var ids = MapAsyncEnumerator(requestStream, x => x.Id, context.CancellationToken);
            var infos = _fileStorage.GetInfos(ids, context.CancellationToken);

            await foreach (var info in infos)
                await responseStream.WriteAsync(new FileInfo
                {
                    Id = info.Id,
                    Name = info.Name,
                    Metadata = info.Metadata ?? string.Empty,
                    Length = info.Length,
                });
        }

        public override async Task<Empty> Update(FileInfo request, ServerCallContext context)
        {
            await _fileStorage.Update(request.Id, request.Name, request.Metadata, context.CancellationToken);
            return new Empty();
        }

        public override async Task<Empty> Delete(FileKey request, ServerCallContext context)
        {
            await _fileStorage.Delete(request.Id, context.CancellationToken);
            return new Empty();
        }

        private static async IAsyncEnumerator<byte[]> GetContent(IAsyncStreamReader<FileUploadChunk> requestStream, CancellationToken cancellationToken)
        {
            do
            {
                if (requestStream.Current.Content.Length > 0)
                    yield return requestStream.Current.Content.ToByteArray();
            } while (await requestStream.MoveNext(cancellationToken));
        }

        private static async IAsyncEnumerator<TRes> MapAsyncEnumerator<T, TRes>(IAsyncStreamReader<T> requestStream, Func<T, TRes> mapper, CancellationToken cancellationToken)
        {
            while (await requestStream.MoveNext(cancellationToken))
                yield return mapper(requestStream.Current);
        }
    }
}