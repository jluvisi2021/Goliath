using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Data
{
    public class GoliathContext : IdentityDbContext
    {
        public GoliathContext(DbContextOptions<GoliathContext> options) : base(options)
        {
            
        }
    }
}
