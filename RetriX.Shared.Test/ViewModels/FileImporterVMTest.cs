using Moq;
using Plugin.FileSystem.Abstractions;
using RetriX.Shared.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.ViewModels
{
    public class FileImporterVMTest : TestBase<FileImporterVM>
    {
        protected override FileImporterVM InstantiateTarget()
        {
            return new FileImporterVM(FileSystemMock.Object, DialogsServiceMock.Object, LocalizationServiceMock.Object, PlatformServiceMock.Object, CryptographyServiceMock.Object, GetTestFilesFolderAsync().Result, "TargetFile.ext", "Target file description", "SomeMD5");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ImportingWorks(bool providedFileMD5Matches)
        {
            await Task.Delay(50);
            Assert.False(Target.FileAvailable);
            Assert.Null(await Target.GetTargetFileAsync());
            Assert.True(Target.ImportCommand.CanExecute(null));

            var folder = await GetTestFilesFolderAsync();
            var pickedFile = await folder.GetFileAsync("TestFile.txt");
            FileSystemMock.Setup(d => d.PickFileAsync(It.Is<IEnumerable<string>>(e => e.Contains(Path.GetExtension(Target.TargetFileName))))).Returns(Task.FromResult(pickedFile));

            var computedHash = providedFileMD5Matches ? Target.TargetMD5.ToUpperInvariant() : "otherHash";
            CryptographyServiceMock.Setup(d => d.ComputeMD5Async(pickedFile)).Returns(Task.FromResult(computedHash));

            string localizedString = nameof(localizedString);
            LocalizationServiceMock.Setup(d => d.GetLocalizedString(It.IsAny<string>())).Returns(localizedString);

            Target.ImportCommand.Execute(null);
            await Task.Delay(100);

            var expectedDialogServiceCalledTimes = providedFileMD5Matches ? Times.Never() : Times.Once();
            DialogsServiceMock.Verify(d => d.AlertAsync(localizedString, localizedString, null, null), expectedDialogServiceCalledTimes);

            Assert.Equal(providedFileMD5Matches, Target.FileAvailable);
            Assert.Equal(!providedFileMD5Matches, Target.ImportCommand.CanExecute(null));

            if (providedFileMD5Matches)
            {
                var targetFile = await Target.GetTargetFileAsync();
                Assert.NotNull(targetFile);
                await targetFile.DeleteAsync();
            }
        }

        [Fact]
        public async Task NoFileSelectionIsHandled()
        {
            await Task.Delay(50);
            Assert.False(Target.FileAvailable);

            FileSystemMock.Setup(d => d.PickFileAsync(It.Is<IEnumerable<string>>(e => e.Contains(Path.GetExtension(Target.TargetFileName))))).Returns(Task.FromResult(default(IFileInfo)));

            CryptographyServiceMock.Verify(d => d.ComputeMD5Async(It.IsAny<IFileInfo>()), Times.Never);
            LocalizationServiceMock.Verify(d => d.GetLocalizedString(It.IsAny<string>()), Times.Never);
            DialogsServiceMock.Verify(d => d.AlertAsync(It.IsAny<string>(), It.IsAny<string>(), null, null), Times.Never);

            Target.ImportCommand.Execute(null);
            await Task.Delay(100);

            Assert.False(Target.FileAvailable);
        }
    }
}
