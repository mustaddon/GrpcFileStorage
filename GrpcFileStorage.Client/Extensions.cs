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
            Stream content, string name, TMetadata? metadata = default, CancellationToken cancellationToken = default)
        {
            return DfsExtensions.Add(dfs, content, name, metadata, cancellationToken);
        }

        public static Task<string> Add<TMetadata>(this FileStorageClient<TMetadata> dfs,
            byte[] content, string name, TMetadata? metadata = default, CancellationToken cancellationToken = default)
        {
            return DfsExtensions.Add(dfs, content, name, metadata, cancellationToken);
        }

        public static Task<string> Add<TMetadata>(this FileStorageClient<TMetadata> dfs,
            IAsyncEnumerable<byte[]> content, string name, TMetadata? metadata = default, CancellationToken cancellationToken = default)
        {
            return DfsExtensions.Add(dfs, content, name, metadata, cancellationToken);
        }

        public static IAsyncEnumerable<IDfsFileInfo<TMetadata>> GetInfos<TMetadata>(this FileStorageClient<TMetadata> dfs,
            IAsyncEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return DfsExtensions.GetInfos(dfs, ids, cancellationToken);
        }

        public static IAsyncEnumerable<IDfsFileInfo<TMetadata>> GetInfos<TMetadata>(this FileStorageClient<TMetadata> dfs,
            IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return DfsExtensions.GetInfos(dfs, ids, cancellationToken);
        }

        public static Task<IDfsFileInfo<TMetadata>> GetInfo<TMetadata>(this FileStorageClient<TMetadata> dfs,
            string id, CancellationToken cancellationToken = default)
        {
            return DfsExtensions.GetInfo(dfs, id, cancellationToken);
        }
    }
}
