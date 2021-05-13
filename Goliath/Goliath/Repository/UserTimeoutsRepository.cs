using Goliath.Data;
using Goliath.Enums;
using Goliath.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <inheritdoc cref="IUserTimeoutsRepository" />
    public class UserTimeoutsRepository : IUserTimeoutsRepository
    {
        private readonly GoliathContext _context;
        /// <summary>
        /// Represents the timeouts for performing different actions in Goliath.
        /// </summary>
        private readonly Dictionary<UserRequest, TimeSpan> _timeSpans = new()
        {
            { UserRequest.RequestUsernameEmail, new(0, 10, 0) },
            { UserRequest.InitalTwoFactorRequestSms, new(0, 6, 0) },
            { UserRequest.RequestDataDownload, new(0, 5, 0) },
            { UserRequest.RequestForgotPasswordEmail, new(0, 10, 0) },
            { UserRequest.RequestTwoFactorResendSms, new(0, 5, 0) },
            { UserRequest.RequestTwoFactorSmsAuthorized, new(0, 1, 0) },
            { UserRequest.RequestVerificationEmail, new(0, 7, 0) },
            { UserRequest.UpdateProfileSettings, new(0, 2, 0) }
        };

        public UserTimeoutsRepository(GoliathContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a user with the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId"> The userId to query. </param>
        /// <returns> The NumericId of that user. </returns>
        private async Task<int> GetUserNumericIndex(string userId)
        {
            UserTimeouts timeouts = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.UserId.Equals(userId));
            if (timeouts == null)
            {
                return -1;
            }
            return timeouts.NumericId;
        }

        public async Task UpdateRequestAsync(string userId, UserRequest requestType)
        {
            // Find the ID of the user in the timeouts table.
            int numericId = await GetUserNumericIndex(userId);
            // Check if they are a new user or not.
            bool isNewUser = numericId == -1;
            // Define a new object that will be either added to or modified in the timeouts database.
            UserTimeouts userTimeout = new();
            // Check if they are a new user or if they already exist.
            if(isNewUser)
            {
                userTimeout.UserId = userId;
            }
            else
            {
                userTimeout = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            }

            switch (requestType)
            {
                case UserRequest.RequestVerificationEmail:
                    userTimeout.RequestVerifyEmail = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.RequestUsernameEmail:
                    userTimeout.RequestForgotUsername = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.RequestForgotPasswordEmail:
                    userTimeout.RequestForgotPassword = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.InitalTwoFactorRequestSms:
                    userTimeout.RequestTwoFactorSmsInital = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.RequestTwoFactorResendSms:
                    userTimeout.RequestTwoFactorSmsResend = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.RequestDataDownload:
                    userTimeout.RequestDataDownload = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.RequestTwoFactorSmsAuthorized:
                    userTimeout.RequestTwoFactorSmsAuthorized = DateTime.UtcNow.ToString();
                    break;
                case UserRequest.UpdateProfileSettings:
                    userTimeout.UpdateProfileSettings = DateTime.UtcNow.ToString();
                    break;
                default:
                    GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "Invalid Enum for parameter requestType");
                    return;
            }
            // If they are a new user then add them to the table.
            if (isNewUser)
            {
                await _context.AddAsync(userTimeout);
            }
            // Save all changes.
            await _context.SaveChangesAsync();
        }


        public async Task<bool> CanRequestInitalTwoFactorSmsAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }

            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestTwoFactorSmsInital))
            {
                return true;
            }

            if (DateTime.Parse(existingUser.RequestTwoFactorSmsInital) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.InitalTwoFactorRequestSms]))
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
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestVerifyEmail))
            {
                return true;
            }

            if (DateTime.Parse(existingUser.RequestVerifyEmail) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.RequestVerificationEmail]))
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
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestForgotPassword))
            {
                return true;
            }

            if (DateTime.Parse(existingUser.RequestForgotPassword) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.RequestForgotPasswordEmail]))
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
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestForgotUsername))
            {
                return true;
            }

            if (DateTime.Parse(existingUser.RequestForgotUsername) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.RequestUsernameEmail]))
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
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestTwoFactorSmsResend))
            {
                return true;
            }

            if (DateTime.Parse(existingUser.RequestTwoFactorSmsResend) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.RequestTwoFactorResendSms]))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestDataDownloadAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestDataDownload))
            {
                return true;
            }

            if (DateTime.Parse(existingUser.RequestDataDownload) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.RequestDataDownload]))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestAuthorizedTwoFactorSmsAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.RequestTwoFactorSmsAuthorized))
            {
                return true;
            }
            // Over 2 Minutes Old
            if (DateTime.Parse(existingUser.RequestTwoFactorSmsAuthorized) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.RequestTwoFactorSmsAuthorized]))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CanRequestProfileSettingsUpdateAsync(string userId)
        {
            int numericId = await GetUserNumericIndex(userId);
            if (numericId == -1)
            {
                return true;
            }
            UserTimeouts existingUser = await _context.UserTimeoutsTable.FirstOrDefaultAsync(u => u.NumericId == numericId);
            if (string.IsNullOrWhiteSpace(existingUser.UpdateProfileSettings))
            {
                return true;
            }
            // Over 2 Minutes Old
            if (DateTime.Parse(existingUser.UpdateProfileSettings) < DateTime.UtcNow.Subtract(_timeSpans[UserRequest.UpdateProfileSettings]))
            {
                return true;
            }
            return false;
        }
    }
}