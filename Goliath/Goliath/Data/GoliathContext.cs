﻿using Goliath.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goliath.Data
{
    /// <summary>
    /// Represents the EF Core database for Goliath.
    /// </summary>
    public class GoliathContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public GoliathContext(DbContextOptions<GoliathContext> options) : base(options)
        {
        }

        /// <summary>
        /// Tokens which are generated when a user completes a captcha successfully.
        /// </summary>
        public DbSet<ValidHumanVerifyTokens> ValidCaptchaTokens { get; set; }

        /// <summary>
        /// A table of timestamps to determine if a user can request another SMS resend.
        /// </summary>
        public DbSet<ResendSmsConfirmationToken> SmsVerifyTable { get; set; }

        /// <summary>
        /// A table which represents a UserId as well as columns of DateTimes.
        /// </summary>
        public DbSet<UserTimeouts> UserTimeoutsTable { get; set; }

        /// <summary>
        /// A table of TwoFactorTokens that are given to a user after they login through <see
        /// cref="Controllers.AuthController.Login(SignInModel)" /> but have yet to confirm a two
        /// factor code.
        /// </summary>
        public DbSet<TwoFactorAuthorizeToken> TwoFactorTokens { get; set; }
    }
}