using System.IO;

namespace RetriX.Shared.Services
{
    public interface ICryptographyService
    {
        string ComputeMD5(Stream stream);
    }
}
