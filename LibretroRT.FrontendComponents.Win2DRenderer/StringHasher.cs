using System;
using System.Security.Cryptography;
using System.Text;

namespace LibretroRT.FrontendComponents.Win2DRenderer
{
    public static class StringHasher
    {
        private static readonly SHA1 SHA1Algorithm = System.Security.Cryptography.SHA1.Create();
        private static readonly UTF8Encoding Encoder = new UTF8Encoding();

        public static string SHA1(this string input)
        {
            var bytes = Encoder.GetBytes(input);
            bytes = SHA1Algorithm.ComputeHash(bytes);
            var hash = Convert.ToBase64String(bytes);
            return hash;
        }
    }
}
