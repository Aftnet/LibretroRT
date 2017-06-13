using RetriX.Shared.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetriX.Shared.Test.Services
{
    public class SaveStateServiceTest : TestBase<SaveStateService>
    {
        private const int InitializationDelayMs = 50;

        private const string GameId = nameof(GameId);
        private const int SlotID = 4;

        static readonly byte[] TestSavePayload = Enumerable.Range(0, byte.MaxValue).Select(d => (byte)d).ToArray();

        protected override SaveStateService InstanceTarget()
        {
            return new SaveStateService { GameId = GameId };
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task InvalidIdIsHandledCorrectly(string gameId)
        {
            await Task.Delay(InitializationDelayMs);

            Target.GameId = gameId;
            var result = await Target.SlotHasDataAsync(SlotID);
            Assert.False(result);

            result = await Target.SaveStateAsync(SlotID, TestSavePayload);
            Assert.False(result);

            result = await Target.SlotHasDataAsync(SlotID);
            Assert.False(result);

            var loaded = await Target.LoadStateAsync(SlotID);
            Assert.Null(loaded);
        }

        [Fact]
        public async Task SavingWorks()
        {
            await Task.Delay(InitializationDelayMs);

            var result = await Target.SlotHasDataAsync(SlotID);
            Assert.False(result);

            var loaded = await Target.LoadStateAsync(SlotID);
            Assert.Null(loaded);

            result = await Target.SaveStateAsync(SlotID, TestSavePayload);
            Assert.True(result);

            loaded = await Target.LoadStateAsync(SlotID);
            Assert.Equal(loaded, TestSavePayload);

            await Target.ClearSavesAsync();
        }
    }
}
