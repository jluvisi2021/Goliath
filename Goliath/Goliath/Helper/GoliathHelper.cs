using System;
using System.Security.Cryptography;
using System.Text;

namespace Goliath.Helper
{
    /// <summary>
    /// A class which provides simple static methods and utilities.
    /// </summary>
    public sealed class GoliathHelper
    {
        /// <summary>
        /// The type that a debug message is.
        /// </summary>
        public enum PrintType
        {
            Error, Information
        }

        /// <summary>
        /// Prints a message to the debugger for ASP.NET Core.
        /// </summary>
        /// <param name="data"> </param>
        public static void PrintDebugger(string data)
        {
            System.Diagnostics.Debug.WriteLine(data);
        }

        /// <summary>
        /// Print a message to the debugger for ASP.NET Core <br /> with a specified type.
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="data"> </param>
        public static void PrintDebugger(PrintType type, string data)
        {
            switch (type)
            {
                case PrintType.Error:
                    System.Diagnostics.Debug.WriteLine("!-------------------------!");
                    System.Diagnostics.Debug.WriteLine($"Error: {data}");
                    System.Diagnostics.Debug.WriteLine("!-------------------------!");
                    return;

                case PrintType.Information:
                    System.Diagnostics.Debug.WriteLine($"DEBUG: {data}");
                    return;
            }
        }

        /// <summary>
        /// Generates a secure random number using the RNGCryptoServiceProvider.
        /// </summary>
        /// <returns> </returns>
        public static string GenerateSecureRandomNumber()
        {
            StringBuilder validCode = new();
            RNGCryptoServiceProvider provider = new();
            byte[] byteArray = new byte[4];
            provider.GetBytes(byteArray);
            uint randomInteger = BitConverter.ToUInt32(byteArray, 0);
            string x = System.Convert.ToString(randomInteger);
            for (int i = 0; i < x.Length; i++)
            {
                validCode.Append(x[i]);
            }
            provider.Dispose();
            return validCode.ToString();
        }
    }
}