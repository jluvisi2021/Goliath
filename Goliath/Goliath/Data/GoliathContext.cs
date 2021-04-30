using Goliath.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goliath.Data
{
    public class GoliathContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public GoliathContext(DbContextOptions<GoliathContext> options) : base(options)
        {
        }

        public DbSet<ValidHumanVerifyTokens> ValidTokens { get; set; }
        public DbSet<ResendSmsConfirmationToken> SmsVerifyTable { get; set; }
        public DbSet<UnauthorizedTimeouts> TimeoutsUnauthorizedTable { get; set; }
        public DbSet<TwoFactorAuthorizeToken> TwoFactorTokens { get; set; }
    }
}