using Goliath.Data;
using Goliath.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public class ValidHumanVerifyTokensRepository : IValidHumanVerifyTokensRepository
    {
        private readonly GoliathContext _context;

        public ValidHumanVerifyTokensRepository(GoliathContext context)
        {
            _context = context;
        }

        public async Task AddTokenAsync(string key)
        {
            ValidHumanVerifyTokens token = new()
            {
                Token = key,
                GeneratedDateTime = DateTime.UtcNow.ToString()
            };

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
            await CleanUpUnusedTokens();

            List<string> result = await _context.ValidTokens.Select(u => u.Token).ToListAsync();

            for (int i = 0; i < result.Count; i++)
            {
                if (GoliathHash.HashStringSHA256(result[i]).Equals(hashCode))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves throughout the database and removes all tokens over 5 minutes old.
        /// </summary>
        /// <returns> </returns>
        public async Task CleanUpUnusedTokens()
        {
            List<int> primaryKeys = await _context.ValidTokens.Select(u => u.NumericID).ToListAsync();
            foreach (int keyIndex in primaryKeys)
            {
                ValidHumanVerifyTokens key = await _context.ValidTokens.FindAsync(keyIndex);
                if (DateTime.Parse(key.GeneratedDateTime) < DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)))
                {
                    _context.Remove(key);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task ClearAllTokensAsync()
        {
            foreach (ValidHumanVerifyTokens token in _context.ValidTokens)
            {
                _context.Remove(token);
            }
            await _context.SaveChangesAsync();
        }
    }
}