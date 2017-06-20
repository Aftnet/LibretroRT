using RetriX.Shared.FileProviders;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.FileProviders
{
    public class SingleFileProviderTest : TestBase<SingleFileProvider>
    {
        private const string FilePath = "scheme:\\SomeFile.ext";

        protected override SingleFileProvider InstantiateTarget()
        {
            var folder = GetTestFilesFolderAsync().Result;
            var file = folder.GetFileAsync("TestFile.txt").Result;
            return new SingleFileProvider(FilePath, file);
        }

        [Theory]
        [InlineData(FilePath, true)]
        [InlineData("scheme2:\\SomeFile.ext", false)]
        [InlineData("scheme:\\SomeFi.ext", false)]
        [InlineData("scheme:\\Dir\\file.ext", false)]
        public async Task OpeningFileWorks(string path, bool expectedSuccess)
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
