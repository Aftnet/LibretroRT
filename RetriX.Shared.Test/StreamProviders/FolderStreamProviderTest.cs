using RetriX.Shared.StreamProviders;
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
            return ListingEntriesWorksInternal(4);
        }

        [Theory]
        [InlineData("scheme:\\TestFile.txt", true)]
        [InlineData("scheme:\\Archive.zip", true)]
        [InlineData("scheme:\\A\\B\\AnotherFile.cds", true)]
        [InlineData("scheme:\\A\\C\\AnotherFile.cds", false)]
        [InlineData("ascheme:\\Archive.zip", false)]
        [InlineData("ascheme:\\SomeFi.ext", false)]
        [InlineData("ascheme:\\Dir\\file.ext", false)]
        public Task OpeningFileWorks(string path, bool expectedSuccess)
        {
            return OpeningFileWorksInternal(path, expectedSuccess);
        }
    }
}
