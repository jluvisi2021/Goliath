using Goliath.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel, string[] data);

        Task<SignInResult> PasswordSignInAsync(SignInModel signInModel);

        Task SignOutAsync();

        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);

        Task<ApplicationUser> FindByEmailAsync(string email);

        Task GenerateEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser userModel, string[] data);

        Task GenerateEmailConfirmationToken(ApplicationUser userModel, string[] data);
    }
}