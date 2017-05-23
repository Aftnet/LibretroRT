using System;
using System.Threading.Tasks;
using Xunit;

namespace LibretroRT.Test
{
    public class CoreTests
    {
        private ICore Target { get; set; }

        public CoreTests()
        {
            Target = GPGXRT.GPGXCore.Instance;
        }

        [Fact]
        public void CoreInfoIsRead()
        {
            Assert.NotNull(Target.Name);
            Assert.NotNull(Target.Version);
            Assert.NotNull(Target.SupportedExtensions);

            Assert.NotNull(Target.Geometry);
            Assert.NotNull(Target.Timing);
        }

        [Fact]
        public async Task LoadingRomWorks()
        {
            var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await installedLocation.GetFileAsync("Roms\\Sonic2.md");
            await Task.Run(() =>
            {
                Target.LoadGame(file);
            });
        }
    }
}
