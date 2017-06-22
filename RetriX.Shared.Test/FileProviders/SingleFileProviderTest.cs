﻿using RetriX.Shared.FileProviders;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.FileProviders
{
    public class SingleFileProviderTest : FileProviderTestBase<SingleFileProvider>
    {
        private const string FilePath = "scheme:\\SomeFile.ext";

        protected override SingleFileProvider InstantiateTarget()
        {
            var file = GetTestFilesFolderAsync().Result.GetFileAsync("TestFile.txt").Result;
            return new SingleFileProvider(FilePath, file);
        }

        [Fact]
        public Task ListingEntriesWorks()
        {
            return ListingEntriesWorks(1);
        }

        [Theory]
        [InlineData(FilePath, true)]
        [InlineData("scheme2:\\SomeFile.ext", false)]
        [InlineData("scheme:\\SomeFi.ext", false)]
        [InlineData("scheme:\\Dir\\file.ext", false)]
        public Task OpeningFileWorks(string path, bool expectedSuccess)
        {
            return OpeningFileWorksInternal(path, expectedSuccess);
        }
    }
}
