using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Goliath.Helper
{
    //Credit: https://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt
    /// <summary>
    /// <para>
    /// Class for handling AES encryption in Goliath.
    /// </para>
    /// <para>
    /// The salts used in AesHelper are usually usernames + <see cref="extraSaltData"/>.
    /// </para>
    /// </summary>
    public static class AesHelper
    {
        // Number of iterations to perform.
        const int iterations = 1000;
        // Extra data to be added onto the salt with the username already.
        const string extraSaltData = "abcdefg12345";

        /// <summary>
        /// Encrypt text using AES encryption.
        /// </summary>
        /// <param name="input">The text to encrypt.</param>
        /// <param name="password">The private key.</param>
        /// <param name="salt">Salt data (usually a username)</param>
        /// <returns></returns>
        public static string EncryptText(string input, string password, string salt)
        {
            try
            {
                // Get the bytes of the string
                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes, salt);

                string result = Convert.ToBase64String(bytesEncrypted);

                return result;
            } catch (Exception)
            {
                return "Error";
            }
            
        }

        /// <summary>
        /// Decrypt text using AES encryption.
        /// </summary>
        /// <param name="input">The text to decrypt.</param>
        /// <param name="password">The private key.</param>
        /// <param name="salt">Salt data (usually a username)</param>
        /// <returns></returns>
        public static string DecryptText(string input, string password, string salt)
        {
            try
            {
                // Get the bytes of the string
                byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes, salt);

                string result = Encoding.UTF8.GetString(bytesDecrypted);

                return result;
            } catch (Exception)
            {
                return "Error";
            }
            
        }
        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, string salt)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = Encoding.ASCII.GetBytes($"{salt}{extraSaltData}");

            using (MemoryStream ms = new())
            {
                using (RijndaelManaged AES = new())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, string salt)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = Encoding.ASCII.GetBytes($"{salt}{extraSaltData}");

            using (MemoryStream ms = new())
            {
                using (RijndaelManaged AES = new())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}
