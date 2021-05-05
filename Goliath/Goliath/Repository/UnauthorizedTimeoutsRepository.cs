using Goliath.Data;
using Goliath.Enums;
using Goliath.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <inheritdoc cref="IUnauthorizedTimeoutsRepository" />
    public class UnauthorizedTimeoutsRepository : IUnauthorizedTimeoutsRepository
    {
        private readonly GoliathContext _context;

        public UnauthorizedTimeoutsRepository(GoliathContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a user with the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId"> The userId to query. </param>
        /// <returns> The NumericID of that user. </returns>
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
                        break;
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestVerifyEmail = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                    }
                    break;

                case UnauthorizedRequest.RequestUsernameEmail:
                    if (numericId != -1)
                    {
                        UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
                        existingUser.RequestForgotUsername = DateTime.UtcNow.ToString();
                        break;
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestForgotUsername = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                    }
                    break;

                case UnauthorizedRequest.RequestForgotPasswordEmail:
                    if (numericId != -1)
                    {
                        UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
                        existingUser.RequestForgotPassword = DateTime.UtcNow.ToString();
                        break;
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestForgotPassword = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                    }
                    break;

                case UnauthorizedRequest.InitalTwoFactorRequestSms:
                    if (numericId != -1)
                    {
                        UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
                        existingUser.RequestTwoFactorSmsInital = DateTime.UtcNow.ToString();
                        break;
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestTwoFactorSmsInital = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                    }
                    break;

                case UnauthorizedRequest.RequestTwoFactorResendSms:
                    if (numericId != -1)
                    {
                        UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
                        existingUser.RequestTwoFactorSmsResend = DateTime.UtcNow.ToString();
                        break;
                    }
                    else
                    {
                        UnauthorizedTimeouts newUser = new()
                        {
                            UserId = userId,
                            RequestTwoFactorSmsResend = DateTime.UtcNow.ToString()
                        };
                        await _context.AddAsync(newUser);
                    }
                    break;

                default:
                    GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "Invalid Enum for parameter requestType");
                    return;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanRequestTwoFactorSmsAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestTwoFactorSmsInital))
            {
                return true;
            }
            // Over 5 Minutes Old
            if (DateTime.Parse(existingUser.RequestTwoFactorSmsInital) < DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestEmailResendVerifyAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestVerifyEmail))
            {
                return true;
            }
            // Over 5 Minutes Old
            if (DateTime.Parse(existingUser.RequestVerifyEmail) < DateTime.UtcNow.Subtract(new TimeSpan(0, 5, 0)))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestForgotPasswordAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestForgotPassword))
            {
                return true;
            }
            // Over 5 Minutes Old
            if (DateTime.Parse(existingUser.RequestForgotPassword) < DateTime.UtcNow.Subtract(new TimeSpan(0, 15, 0)))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestUsernameAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestForgotUsername))
            {
                return true;
            }
            // Over 5 Minutes Old
            if (DateTime.Parse(existingUser.RequestForgotUsername) < DateTime.UtcNow.Subtract(new TimeSpan(0, 15, 0)))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestResendTwoFactorSmsAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UnauthorizedTimeouts existingUser = await _context.TimeoutsUnauthorizedTable.FirstOrDefaultAsync(u => u.NumericID == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestTwoFactorSmsResend))
            {
                return true;
            }
            // Over 2 Minutes Old
            if (DateTime.Parse(existingUser.RequestTwoFactorSmsResend) < DateTime.UtcNow.Subtract(new TimeSpan(0, 2, 0)))
            {
                return true;
            }
            return false;
        }
    }
}