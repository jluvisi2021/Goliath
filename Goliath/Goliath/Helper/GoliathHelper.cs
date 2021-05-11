using System.Text;

namespace Goliath.Helper
{
    /// <summary>
    /// A class which provides simple static methods and utilities.
    /// </summary>
    public sealed class GoliathHelper
    {
        /// <summary>
        /// The maximum length of cryptographic tokens. Migrations will have to be updated after
        /// modifying this value.
        /// </summary>
        public const int MaximumTokenValueLength = 32;

        /// <summary>
        /// The minimum length of cryptographic secure tokens. Migrations will have to be updated
        /// after modifying this value.
        /// </summary>
        public const int MinimumTokenValueLength = 32;

        /// <summary>
        /// The type that a debug message is. Migrations will have to be updated after modifying
        /// this value.
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
        /// Generates a secure random number using the <see cref="RandomGenerator" />. The maximum
        /// value and minimum value for the random numbers are set
        /// </summary>
        /// <returns> </returns>
        public static string GenerateSecureRandomNumber()
        {
            RandomGenerator generator = new();
            StringBuilder token = new();
            for (int i = 0; i < MinimumTokenValueLength; i++)
            {
                token.Append($"{generator.Next(0, 9)}");
            }
            // Add random exta numbers for a varying maximum possible value.
            for (int i = 0; i < MaximumTokenValueLength - MinimumTokenValueLength; i++)
            {
                if (generator.Next(0, 1) == 0)
                {
                    token.Append($"{generator.Next(0, 9)}");
                }
            }
            generator.Dispose();
            return token.ToString();
        }
    }
}