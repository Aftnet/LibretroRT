﻿using RetriX.Shared.FileProviders;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.FileProviders
{
    class ArchiveFileProviderTest : FileProviderTestBase<ArchiveFileProvider>
    {
        protected override ArchiveFileProvider InstantiateTarget()
        {
            var file = GetTestFilesFolderAsync().Result.GetFileAsync("Archive.zip").Result;
            return new ArchiveFileProvider("scheme:\\", file);
        }

        [Fact]
        public Task ListingEntriesWorks()
        {
            return ListingEntriesWorks(4);
        }

        [Theory]
        [InlineData("scheme:\\TestFile.txt", true)]
        [InlineData("scheme:\\AnotherFile.cds", true)]
        [InlineData("scheme:\\Afolder\\File.zzz", true)]
        [InlineData("scheme2:\\SomeFile.ext", false)]
        [InlineData("scheme:\\SomeFi.ext", false)]
        [InlineData("scheme:\\Dir\\file.ext", false)]
        public Task OpeningFileWorks(string path, bool expectedSuccess)
        {
            return OpeningFileWorksInternal(path, expectedSuccess);
        }
    }
}
