using GPGX_RT;
using Xunit;

namespace LibRetroRT.Test
{
    public class GPGXCoreTest : TestBase<GPGXCore>
    {
        protected override GPGXCore GetTarget()
        {
            return GPGXCore.Instance;
        }

        [Fact]
        public void CoreInfoCanBeRead()
        {
            Assert.NotEmpty(Target.Name);
            Assert.NotEmpty(Target.Version);
            Assert.NotEmpty(Target.SupportedExtensions);

            Assert.NotEqual(0f, Target.AVInfo.Geometry.AspectRatio);
            Assert.NotEqual(0U, Target.AVInfo.Geometry.BaseHeight);
            Assert.NotEqual(0U, Target.AVInfo.Geometry.BaseWidth);
            Assert.NotEqual(0U, Target.AVInfo.Geometry.MaxHeight);
            Assert.NotEqual(0U, Target.AVInfo.Geometry.MaxWidth);

            Assert.NotEqual(0, Target.AVInfo.Timings.AudioSampleRate);
            Assert.NotEqual(0, Target.AVInfo.Timings.FPS);
        }
    }
}
