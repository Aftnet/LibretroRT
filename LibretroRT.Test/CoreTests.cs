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
        public void LoadingRomWorks()
        {
            Target.LoadGame(null);

        }
    }
}
