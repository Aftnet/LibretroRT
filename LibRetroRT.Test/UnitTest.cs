using GPGX_RT;
using Xunit;

namespace LibRetroRT.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            var core = GPGXCore.Instance;
            Assert.NotEmpty(core.Name);
            Assert.NotEmpty(core.Version);
            Assert.NotEmpty(core.SupportedExtensions);
        }
    }
}
