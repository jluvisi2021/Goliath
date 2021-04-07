using System;

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
                    System.Diagnostics.Debug.WriteLine($"ERROR: {data}");
                    System.Diagnostics.Debug.WriteLine("!-------------------------!");
                    return;

                case PrintType.Information:
                    System.Diagnostics.Debug.WriteLine($"DEBUG: {data}");
                    return;
            }
        }

        public static string GenerateSecureRandomNumber()
        {
            Random rand = new Random();
            return $"{rand.Next(7650000)}";
        }
    }
}