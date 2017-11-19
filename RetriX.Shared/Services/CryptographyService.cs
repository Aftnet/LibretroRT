using PCLStorage;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public class CryptographyService : ICryptographyService
    {
        public async Task<string> ComputeMD5Async(IFile file)
        {
            using (var inputStream = await file.OpenAsync(PCLStorage.FileAccess.Read))
            using (var hasher = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            using (var nullStream = Stream.Null)
            {
                var buffer = new byte[1024 * 1024];
                var numRead = 0;
                do
                {
                    numRead = await inputStream.ReadAsync(buffer, numRead, buffer.Length);
                    hasher.AppendData(buffer, 0, numRead);
                }
                while (numRead > 0);

                var hashBytes = hasher.GetHashAndReset();
                var hashString = BitConverter.ToString(hashBytes);
                return hashString.Replace("-", string.Empty);
            }            
        }
    }
}
