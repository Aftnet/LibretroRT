using PCLCrypto;
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public class CryptographyService : ICryptographyService
    {
        public async Task<string> ComputeMD5Async(IFile file)
        {
            using (var inputStream = await file.OpenAsync(PCLStorage.FileAccess.Read))
            using (var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5).CreateHash())
            using (var nullStream = Stream.Null)
            {
                using (var cryptoStream = CryptoStream.WriteTo(nullStream, hasher))
                {
                    await inputStream.CopyToAsync(cryptoStream);
                    inputStream.Position = 0;
                    var hashBytes = hasher.GetValueAndReset();
                    var hashString = BitConverter.ToString(hashBytes);
                    return hashString.Replace("-", string.Empty);
                }
            }            
        }
    }
}
