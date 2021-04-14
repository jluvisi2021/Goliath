using Goliath.Helper;
using Goliath.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Goliath.Repository
{
    /// <summary>
    /// Assume all methods are Async unless specifically designated as not.
    /// </summary>
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel, DeviceParser device);

        Task<SignInResult> PasswordSignInAsync(SignInModel signInModel);

        Task SignOutAsync();

        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);

        Task<ApplicationUser> FindByEmailAsync(string email);

        Task GenerateEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser userModel, DeviceParser device);

        Task GenerateEmailConfirmationToken(ApplicationUser userModel, DeviceParser device);

        Task GenerateForgotPasswordToken(ApplicationUser userModel, DeviceParser device);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);

        Task GenerateUsername(ApplicationUser user, DeviceParser device);

        Task LoadDefaults();

        Task<bool> IsAdmin(ApplicationUser user);

        Task<bool> IsAdmin(string username);

        Task MoveUserToAdminRole(ApplicationUser user);

        Task MoveUserToDefaultRole(ApplicationUser user);

        Task CreateRole(string name);

        Task CreateRole(string name, bool isAdmin);

        Task CreateRole(string name, bool isAdmin, string excludedURLComponents);

        Task CreateRole(string name, string icon, bool isAdmin);

        Task DeleteRole(string name);

        Task<string> GetRoleIcon(string name);

        Task<string> GetRoleExcludedURLComponents(string name);

        Task<List<ApplicationUser>> GetAllUsersInRole(string name);

        Task MoveUserToRoleByName(ApplicationUser user, string name);

        Task<string> GetPrimaryRole(ApplicationUser user);

        Task<ApplicationUser> GetFromUserClaim(ClaimsPrincipal claimsPrincipal);

        Task<ApplicationUser> GetUserByName(string name);

        Task<IdentityResult> UpdateUser(ApplicationUser user);

        Task<bool> HasPhoneNumber(ApplicationUser user);
        Task<bool> HasConfirmedPhoneNumber(ApplicationUser user);

        Task UpdatePhone(ApplicationUser user, string number);
    }
}