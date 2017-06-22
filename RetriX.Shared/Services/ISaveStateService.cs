using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public interface ISaveStateService
    {
        void SetGameId(string id);

        Task<byte[]> LoadStateAsync(uint slotId);
        Task<bool> SaveStateAsync(uint slotId, byte[] data);
        Task<bool> SlotHasDataAsync(uint slotId);
        Task ClearSavesAsync();
    };
}
