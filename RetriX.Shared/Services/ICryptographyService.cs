using PCLStorage;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public interface ICryptographyService
    {
        Task<string> ComputeMD5Async(IFile file);
    }
}
