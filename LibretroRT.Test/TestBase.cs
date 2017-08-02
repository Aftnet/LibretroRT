using System;
using System.Threading.Tasks;
using Windows.Storage;
using Xunit;

namespace LibretroRT.Test
{
    [Collection(nameof(TestBase))]
    public abstract class TestBase
    {
        protected ICore Target { get; private set; }

        protected TestBase(Func<ICore> coreInstancer)
        {
            Target = coreInstancer();
        }

        [Fact]
        public async Task CoreInfoIsRead()
        {
            Assert.NotNull(Target.Name);
            Assert.NotNull(Target.Version);
            Assert.NotNull(Target.SupportedExtensions);

            Assert.NotNull(Target.Geometry);
            Assert.NotNull(Target.Timing);

            await Task.Delay(50);
            Assert.NotNull(Target.SystemFolder);
            Assert.NotEmpty(Target.SystemFolder.Path);
            Assert.NotNull(Target.SaveGameFolder);
            Assert.NotEmpty(Target.SaveGameFolder.Path);

            Assert.NotNull(Target.Options);
            foreach(var i in Target.Options)
            {
                Assert.NotEmpty(i.Key);

                var value = i.Value;
                Assert.NotNull(value);
                Assert.NotEmpty(value.Description);
                Assert.NotEmpty(value.Values);
                foreach(var j in value.Values)
                {
                    Assert.NotEmpty(j);
                }
            }

            Assert.NotNull(Target.FileDependencies);
            foreach(var i in Target.FileDependencies)
            {
                Assert.False(string.IsNullOrEmpty(i.Description) || string.IsNullOrWhiteSpace(i.Description));
                Assert.False(string.IsNullOrEmpty(i.Name) || string.IsNullOrWhiteSpace(i.Name));
                Assert.Equal(32, i.MD5.Length);
            }
        }

        public abstract Task LoadingRomWorks(string romName);

        protected async Task LoadingRomWorksInternal(string romName)
        {
            var romsFolder = await GetRomsFolderAsync();
            var provider = new StreamProvider(VFS.SystemPath, romsFolder);
            Target.OpenFileStream = provider.OpenFileStream;
            Target.CloseFileStream = provider.CloseFileStream;

            var romPath = VFS.SystemPath + romName;
            var loadResult = await Task.Run(() => Target.LoadGame(romPath));

            Assert.True(loadResult);
            Assert.NotEqual(PixelFormats.FormatUknown, Target.PixelFormat);

            var geometry = Target.Geometry;
            Assert.NotEqual(0, geometry.AspectRatio);
            Assert.NotEqual(0U, geometry.BaseWidth);
            Assert.NotEqual(0U, geometry.BaseHeight);
            Assert.NotEqual(0U, geometry.MaxHeight);
            Assert.NotEqual(0U, geometry.MaxWidth);

            var timing = Target.Timing;
            Assert.NotEqual(0, timing.FPS);
            Assert.NotEqual(0, timing.SampleRate);

            await Task.Run(() => Target.UnloadGame());
            loadResult = await Task.Run(() => Target.LoadGame(romPath));
            Assert.True(loadResult);
        }

        public abstract Task ExecutionWorks(string romName);

        protected async Task ExecutionWorksInternal(string romName)
        {
            var pollInputCalled = false;
            Target.PollInput = () => pollInputCalled = true;

            var getInputStateCalled = false;
            Target.GetInputState += (d, e) =>
            {
                getInputStateCalled = true;
                return 0;
            };

            var renderVideoFrameCalled = false;
            Target.RenderVideoFrame = (d, e, f, g) =>
            {
                Assert.True(e > 0);
                Assert.True(f > 0);
                Assert.True(g > 0);
                renderVideoFrameCalled = true;
            };

            var renderAudioFrameCalled = false;
            Target.RenderAudioFrames = (d) =>
            {
                Assert.NotEmpty(d);
                Assert.True(d.Length % 2 == 0);
                renderAudioFrameCalled = true;
            };

            var romsFolder = await GetRomsFolderAsync();
            var provider = new StreamProvider(VFS.SystemPath, romsFolder);
            Target.OpenFileStream = provider.OpenFileStream;
            Target.CloseFileStream = provider.CloseFileStream;

            var loadResult = await Task.Run(() => Target.LoadGame(VFS.SystemPath + romName));
            Assert.True(loadResult);

            await Task.Run(() =>
            {
                Target.RunFrame();
            });

            Assert.True(pollInputCalled);
            Assert.True(getInputStateCalled);
            Assert.True(renderVideoFrameCalled);
            Assert.True(renderAudioFrameCalled);
        }

        private Task<StorageFolder> GetRomsFolderAsync()
        {
            var installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            return installedLocation.GetFolderAsync("Roms").AsTask();
        }
    }
}
