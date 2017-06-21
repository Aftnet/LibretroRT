using PCLStorage;

namespace RetriX.Shared.Services
{
    public interface ICryptographyService
    {
        string ComputeMD5(IFile file);
    }
}
