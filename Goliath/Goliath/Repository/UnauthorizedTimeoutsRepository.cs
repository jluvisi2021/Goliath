using Goliath.Data;
using Goliath.Enums;
using Goliath.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public class UnauthorizedTimeoutsRepository : IUnauthorizedTimeoutsRepository
    {
        private readonly GoliathContext _context;

        public UnauthorizedTimeoutsRepository(GoliathContext context)
        {
            _context = context;
        }

        private async Task<int> GetUserNumericIndex(string userId)
        {
            UnauthorizedTimeouts timeouts = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.UserId.Equals(userId));
            if (timeouts == null)
            {
                return -1;
            }
            return timeouts.NumericID;
        }

        public async Task UpdateRequestAsync(string userId, UnauthorizedRequest requestType)
        {
            int numericId = await GetUserNumericIndex(userId);
            switch (requestType)
            {
                case UnauthorizedRequest.RequestVerificationEmail:
                    if (numericId != -1)
                    {
                        UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
                        existingUser.RequestVerifyEmail = DateTime.UtcNow.ToString();
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestVerifyEmail = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                        await _context.SaveChangesAsync();
                    }
                    return;
                case UnauthorizedRequest.RequestUsernameEmail:
                    if (numericId != -1)
                    {
                        UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
                        existingUser.RequestForgotUsername = DateTime.UtcNow.ToString();
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestForgotUsername = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                        await _context.SaveChangesAsync();
                    }
                    return;
                case UnauthorizedRequest.RequestForgotPasswordEmail:
                    return;
                case UnauthorizedRequest.InitalTwoFactorRequestSms:
                    return;
                case UnauthorizedRequest.RequestTwoFactorResendSms:
                    return;
                default:
                    GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "Invalid Enum for parameter requestType");
                    return;
            }
        }
    }
}
