using PCLStorage;
using RetriX.Shared.StreamProviders;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.StreamProviders
{
    public abstract class StreamProviderTestBase
    {
        protected abstract Task<IStreamProvider> GetTargetAsync();

        protected async Task ListingEntriesWorksInternal(int numExpectedEntries)
        {
            var target = await GetTargetAsync();
            var entries = await target.ListEntriesAsync();
            Assert.Equal(numExpectedEntries, entries.Count());
        }

        protected async Task OpeningFileWorksInternal(string path, bool expectedSuccess)
        {
            var target = await GetTargetAsync();
            var stream = await target.GetFileStreamAsync(path, FileAccess.Read);
            if (expectedSuccess)
            {
                Assert.NotNull(stream);
            }
            else
            {
                Assert.Null(stream);
            }

            stream?.Dispose();
        }

        protected Task<IFolder> GetTestFilesFolderAsync()
        {
            return FileSystem.Current.GetFolderFromPathAsync("TestFiles");
        }
    }
}
