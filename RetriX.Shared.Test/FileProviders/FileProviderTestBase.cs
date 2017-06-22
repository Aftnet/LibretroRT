using RetriX.Shared.FileProviders;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.FileProviders
{
    public abstract class FileProviderTestBase<T> : TestBase<T> where T : class, IFileProvider
    {
        protected async Task ListingEntriesWorks(int numExpectedEntries)
        {
            var entries = await Target.ListEntriesAsync();
            Assert.Equal(numExpectedEntries, entries.Count());
        }

        protected async Task OpeningFileWorksInternal(string path, bool expectedSuccess)
        {
            var stream = await Target.GetFileStreamAsync(path, FileAccess.Read);
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
    }
}
