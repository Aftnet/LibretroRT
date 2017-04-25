using Xunit;

namespace GPGC_RT.Test
{
    public class UnitTest1
    {
        [Fact]
        public void TestMethod1()
        {
            var emu = new GPGX_RT.Class1();
            var version = emu.GetVersion();
            Assert.NotEmpty(version);
        }
    }
}
