using Goliath.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Creates the user and adds them to
        /// the database using Identity core.
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
        {
            ApplicationUser user = new()
            {
                UserName = userModel.Username,
                Email = userModel.Email,
            };
            return await (_userManager.CreateAsync(user, userModel.Password));
        }

        public async Task<SignInResult> PasswordSignInAsync(SignInModel signInModel)
        {
            return await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, signInModel.RememberMe, false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}