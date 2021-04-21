using Goliath.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public class SmsVerifyTokensRepository : ISmsVerifyTokensRepository
    {
        private readonly GoliathContext _context;

        public SmsVerifyTokensRepository(GoliathContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a user id to the database with a timestamp.
        /// </summary>
        /// <param name="uid"> </param>
        /// <returns> </returns>
        public async Task AddRequestAsync(string uid)
        {
            ResendSmsConfirmationToken token = new()
            {
                UserID = uid,
                TokenSentTimestamp = DateTime.UtcNow.ToString()
            };

            await _context.SmsVerifyTable.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a user id from the database with a timestamp.
        /// </summary>
        /// <param name="uid"> </param>
        /// <returns> </returns>
        public async Task RemoveRequestAsync(string uid)
        {
            _context.SmsVerifyTable.Remove(await _context.SmsVerifyTable.FirstOrDefaultAsync(u => u.UserID.Equals(uid)));
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// </summary>
        /// <param name="uid"> </param>
        /// <returns> If the user can send another SMS verify code. </returns>
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