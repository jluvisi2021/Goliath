﻿using Goliath.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goliath.Data
{
    public class GoliathContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public GoliathContext(DbContextOptions<GoliathContext> options) : base(options)
        {
        }
    }
}