using Goliath.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountRepository(UserManager<ApplicationUser> userManager) 
        {
            this._userManager = userManager;
        }
        /// <summary>
        /// Creates the user and adds them to
        /// the database using Identity core.
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = userModel.Username,
                Email = userModel.Email,
                
            };
            return await (_userManager.CreateAsync(user, userModel.Password));
        }


    }
}
