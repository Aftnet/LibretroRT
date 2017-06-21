using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public interface ICryptographyService
    {
        Task<string> ComputeMD5Async(Stream stream);
    }
}
