using System;
using System.Security.Cryptography;
using System.Text;

namespace RetriX.Shared.ExtensionMethods
{
    public static class StringHasher
    {
        private static readonly MD5 MD5Algorithm = System.Security.Cryptography.MD5.Create();
        private static readonly SHA1 SHA1Algorithm = System.Security.Cryptography.SHA1.Create();
        private static readonly UTF8Encoding Encoder = new UTF8Encoding();

        public static string MD5(this string input)
        {
            return HashString(input, MD5Algorithm);
        }

        public static string SHA1(this string input)
        {
            return HashString(input, SHA1Algorithm);
        }

        private static string HashString(string input, HashAlgorithm algorithm)
        {
            var bytes = Encoder.GetBytes(input);
            bytes = algorithm.ComputeHash(bytes);
            var hash = Convert.ToBase64String(bytes);
            return hash;
        }
    }
}
