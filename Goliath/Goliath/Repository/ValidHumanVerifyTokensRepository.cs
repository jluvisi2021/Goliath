using Goliath.Data;
using Goliath.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <inheritdoc cref="IValidHumanVerifyTokensRepository" />
    public class ValidHumanVerifyTokensRepository : IValidHumanVerifyTokensRepository
    {
        private readonly GoliathContext _context;
        private readonly ILogger _logger;

        public ValidHumanVerifyTokensRepository(GoliathContext context, ILogger<ValidHumanVerifyTokensRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddTokenAsync(string key)
        {
            ValidHumanVerifyTokens token = new()
            {
                Token = key,
                GeneratedDateTime = DateTime.UtcNow.ToString()
            };
            _logger.LogInformation($"Created a new token for {nameof(ValidHumanVerifyTokens)}.");
            await _context.ValidTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DoesTokenExistAsync(string hashCode)
        {
            if (!GoliathHash.ValidateStringSHA256(hashCode))
            {
                return false;
            }

            // Remove old tokens before searching database.
            await CleanUpUnusedTokensAsync();

            // Get all tokens in the database.
            List<string> result = await _context.ValidTokens.Select(u => u.Token).ToListAsync();

            // Compare Them
            for (int i = 0; i < result.Count; i++)
            {
                if (GoliathHash.HashStringSHA256(result[i]).Equals(hashCode))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task CleanUpUnusedTokensAsync()
        {
            List<int> primaryKeys = await _context.ValidTokens.Select(u => u.NumericId).ToListAsync();
            int amount = 0;
            foreach (int keyIndex in primaryKeys)
            {
                ValidHumanVerifyTokens key = await _context.ValidTokens.FindAsync(keyIndex);
                // Check if token is over 5 minutes old.
                if (DateTime.Parse(key.GeneratedDateTime) < DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)))
                {
                    amount++;
                    _context.Remove(key);
                }
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Cleaned table {nameof(ValidHumanVerifyTokens)}. Removed {amount} tokens from a size of {primaryKeys.Count}.");
        }

        public async Task ClearAllTokensAsync()
        {
            _logger.LogInformation($"Cleared all tokens from {nameof(ValidHumanVerifyTokens)} repository.");
            foreach (ValidHumanVerifyTokens token in _context.ValidTokens)
            {
                _context.Remove(token);
            }
            await _context.SaveChangesAsync();
        }
    }
}