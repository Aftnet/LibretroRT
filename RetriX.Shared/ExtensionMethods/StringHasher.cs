using PCLCrypto;
using System;
using System.Text;

namespace RetriX.Shared.ExtensionMethods
{
    public static class StringHasher
    {
        private static readonly CryptographicHash MD5Algorithm = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Md5).CreateHash();
        private static readonly CryptographicHash SHA1Algorithm = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1).CreateHash();
        private static readonly UTF8Encoding Encoder = new UTF8Encoding();

        public static string MD5(this string input)
        {
            return HashString(input, MD5Algorithm);
        }

        public static string SHA1(this string input)
        {
            return HashString(input, SHA1Algorithm);
        }

        private static string HashString(string input, CryptographicHash algorithm)
        {
            var bytes = Encoder.GetBytes(input);
            algorithm.Append(bytes);
            bytes = algorithm.GetValueAndReset();
            var hash = BitConverter.ToString(bytes);
            return hash.Replace("-", string.Empty).ToLower();
        }
    }
}
