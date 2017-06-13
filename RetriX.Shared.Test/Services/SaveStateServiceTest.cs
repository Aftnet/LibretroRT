using RetriX.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return new SaveStateService();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task InvalidIdIsHandledCorrectly(string gameId)
        {
            await Task.Delay(InitializationDelayMs);

            Target.GameId = gameId;
            var result = await Target.SlotHasData(SlotID);
            Assert.False(result);

            result = await Target.SaveStateAsync(SlotID, TestSavePayload);
            Assert.False(result);

            result = await Target.SlotHasData(SlotID);
            Assert.False(result);

            var loaded = await Target.LoadStateAsync(SlotID);
            Assert.Null(loaded);
        }
    }
}
