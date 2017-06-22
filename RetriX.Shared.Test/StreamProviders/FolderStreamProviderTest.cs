using RetriX.Shared.StreamProviders;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.StreamProviders
{
    public class FolderStreamProviderTest : StreamProviderTestBase
    {
        const string HandledScheme = "scheme:\\";

        protected override async Task<IStreamProvider> GetTargetAsync()
        {
            var folder = await GetTestFilesFolderAsync();
            return new FolderStreamProvider(HandledScheme, folder);
        }

        [Fact]
        public Task ListingEntriesWorks()
        {
            return ListingEntriesWorksInternal(3);
        }

        [Theory]
        [InlineData("scheme:\\TestFile.txt", true)]
        [InlineData("scheme:\\Archive.zip", true)]
        [InlineData("scheme:\\Afolder\\File.zzz", true)]
        [InlineData("ascheme:\\Archive.zip", false)]
        [InlineData("ascheme:\\SomeFi.ext", false)]
        [InlineData("ascheme:\\Dir\\file.ext", false)]
        public Task OpeningFileWorks(string path, bool expectedSuccess)
        {
            return OpeningFileWorksInternal(path, expectedSuccess);
        }
    }
}
