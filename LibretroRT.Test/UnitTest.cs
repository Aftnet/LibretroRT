using Xunit;

namespace LibretroRT.Test
{
    public class UnitTest1
    {
        [Fact]
        public void CoreInfoIsRead()
        {
            var core = GPGXRT.GPGXCore.Instance;

            Assert.NotNull(core.Name);
            Assert.NotNull(core.Version);
            Assert.NotNull(core.SupportedExtensions);

            Assert.NotNull(core.Geometry);
            Assert.NotNull(core.Timing);
        }
    }
}
