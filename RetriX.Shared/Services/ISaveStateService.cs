using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public interface ISaveStateService
    {
        string GameId { get; set; }

        Task<byte[]> LoadStateAsync(uint slotId);
        Task<bool> SaveStateAsync(uint slotId, byte[] data);
        Task<bool> SlotHasData(uint slotId);
    };
}
