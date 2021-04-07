using System;
using System.Security.Cryptography;
using System.Text;

namespace Goliath.Helper
{
    /// <summary>
    /// A utility class which handles the hashing algorithms.
    /// </summary>
    public static class GoliathHash
    {
        public static string HashStringSHA256(string plain)
        {
            using SHA256 sha256hash = SHA256.Create();
            return GetHash(sha256hash, plain);
        }

        public static bool ValidateStringSHA256(string plain)
        {
            using SHA256 sha256hash = SHA256.Create();
            if (VerifyHash(sha256hash, plain, HashStringSHA256(plain)))
            {
                return true;
            }
            return false;
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
        {
            string hashOfInput = GetHash(hashAlgorithm, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            return comparer.Compare(hashOfInput, hash) == 0;
        }
    }
}