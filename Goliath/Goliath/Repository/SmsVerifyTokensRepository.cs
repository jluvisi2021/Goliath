using Goliath.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <inheritdoc cref="ISmsVerifyTokensRepository" />
    public class SmsVerifyTokensRepository : ISmsVerifyTokensRepository
    {
        private readonly GoliathContext _context;

        public SmsVerifyTokensRepository(GoliathContext context)
        {
            _context = context;
        }

        public async Task AddRequestAsync(string uid)
        {
            ResendSmsConfirmationToken timeStamp = new()
            {
                UserID = uid,
                TokenSentTimestamp = DateTime.UtcNow.ToString()
            };

            await _context.SmsVerifyTable.AddAsync(timeStamp);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRequestAsync(string uid)
        {
            _context.SmsVerifyTable.Remove(await _context.SmsVerifyTable.FirstOrDefaultAsync(u => u.UserID.Equals(uid)));
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserResendValidAsync(string uid)
        {
            ResendSmsConfirmationToken user = await _context.SmsVerifyTable.FirstOrDefaultAsync(u => u.UserID.Equals(uid));
            if (user == null)
            {
                // User does not exist in database and therefor can request a resend.
                return true;
            }
            // Check if the last token request is over 2 hours old.
            if (DateTime.Parse(user.TokenSentTimestamp).AddMinutes(2) < DateTime.UtcNow)
            {
                await RemoveRequestAsync(uid);
                return true;
            }
            return false;
        }
    }
}