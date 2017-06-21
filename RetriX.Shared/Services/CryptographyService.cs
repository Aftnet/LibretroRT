using PCLCrypto;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RetriX.Shared.Services
{
    public class CryptographyService : ICryptographyService
    {
        private static readonly CryptographicHash MD5Hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5).CreateHash();

        public async Task<string> ComputeMD5Async(Stream stream)
        {
            using (var outStream = new MemoryStream())
            using (var cryptoStream = CryptoStream.WriteTo(outStream, MD5Hasher))
            {
                await stream.CopyToAsync(cryptoStream);
                var hashBytes = outStream.ToArray();
                var hashString = BitConverter.ToString(hashBytes);
                return hashString;
            }
        }
    }
}
