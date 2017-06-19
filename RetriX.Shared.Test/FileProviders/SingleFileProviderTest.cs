using RetriX.Shared.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.FileProviders
{
    public class SingleFileProviderTest : TestBase<SingleFileProvider>
    {
        private const string FileUri = "scheme://SomeFile.ext";

        protected override SingleFileProvider InstantiateTarget()
        {
            var folder = GetTestFilesFolderAsync().Result;
            var file = folder.GetFileAsync("TestFile.txt").Result;
            return new SingleFileProvider(new Uri(FileUri), file);
        }

        [Theory]
        [InlineData(FileUri, true)]
        [InlineData("scheme2://SomeFile.ext", false)]
        [InlineData("scheme://SomeFi.ext", false)]
        [InlineData("scheme://Dir/file.ext", false)]
        public async Task OpeningFileWorks(string uri, bool expectedSuccess)
        {
            var stream = await Target.GetFileStreamAsync(new Uri(uri, UriKind.Absolute), FileAccess.Read);
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
