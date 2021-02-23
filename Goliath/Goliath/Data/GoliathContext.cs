using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goliath.Data
{
    public class GoliathContext : IdentityDbContext
    {
        public GoliathContext(DbContextOptions<GoliathContext> options) : base(options)
        {
        }
    }
}