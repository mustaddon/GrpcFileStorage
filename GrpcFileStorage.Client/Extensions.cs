using DistributedFileStorage.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcFileStorage.Client
{
    public static class Extensions
    {
        public static Task<string> Add<TMetadata>(this FileStorageClient<TMetadata> dfs,
            Stream stream, string name, TMetadata? metadata, CancellationToken cancellationToken = default)
        {
            return dfs.Add(stream.GetEnumerator(), name, metadata, cancellationToken);
        }

        public static Task<string> Add<TMetadata>(this FileStorageClient<TMetadata> dfs,
            IAsyncEnumerable<byte[]> content, string name, TMetadata? metadata, CancellationToken cancellationToken = default)
        {
            return dfs.Add(content.GetAsyncEnumerator(cancellationToken), name, metadata, cancellationToken);
        }
    }
}
