﻿using System;
using System.Security.Cryptography;

namespace Goliath.Helper
{
    /// <summary>
    /// Secure random generator.
    /// Credit: https://stackoverflow.com/questions/42426420/how-to-generate-a-cryptographically-secure-random-integer-within-a-range
    /// </summary>
    public class RandomGenerator : IDisposable
    {
        private readonly RNGCryptoServiceProvider csp;

        /// <summary>
        /// Constructor
        /// </summary>
        public RandomGenerator()
        {
            csp = new RNGCryptoServiceProvider();
        }

        /// <summary>
        /// Get random value
        /// </summary>
        /// <param name="minValue"> The minimum value. </param>
        /// <param name="maxExclusiveValue"> The maximum value. </param>
        /// <returns> The cryptographic-secure random number. </returns>
        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue == maxExclusiveValue) return minValue;

            if (minValue > maxExclusiveValue)
            {
                throw new ArgumentOutOfRangeException($"{nameof(minValue)} must be lower than {nameof(maxExclusiveValue)}");
            }

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            byte[] randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            csp.GetBytes(buffer);
            return buffer;
        }

        private bool _disposed;

        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"> </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
                csp?.Dispose();
            }

            _disposed = true;
        }
    }
}