using DistributedFileStorage;
using GrpcFileStorage.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Client
{
    [TestClass]
    public partial class Tests
    {
        public Tests()
        {
            _dfs = App.Instance.Value.Services.GetRequiredService<FileStorageClient<TestMetadata>>();
        }

        readonly IDistributedFileStorage<TestMetadata> _dfs;
    }
}