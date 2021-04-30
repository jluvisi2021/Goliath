using Goliath.Enums;
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

        Task<IdentityResult> ConfirmPhoneAsync(ApplicationUser user, string token);

        Task GenerateEmailConfirmationTokenAsync(SignUpUserModel signUpModel, ApplicationUser userModel, DeviceParser device);

        Task GenerateEmailConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device);

        Task<bool> TwoFactorCodeValidAsync(ApplicationUser user, string token);

        Task GenerateTwoFactorCode(ApplicationUser user);

        Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token);

        Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token, bool rememberMe);

        Task SendTwoFactorCodeSms(ApplicationUser user);

        Task<List<string>> GenerateUserRecoveryCodesAsync(ApplicationUser user);

        Task SetTwoFactorDisabledAsync(ApplicationUser user);

        Task SetTwoFactorEnabledAsync(ApplicationUser user, TwoFactorMethod method);

        Task<int> GetUserRecoveryCodeAmountAsync(ApplicationUser user);

        Task<IdentityResult> RedeemRecoveryCodeAsync(ApplicationUser user, string code);

        Task GenerateForgotPasswordTokenAsync(ApplicationUser userModel, DeviceParser device);

        Task GenerateNewEmailConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device);

        Task GenerateNewPhoneConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device);

        Task GenerateNewPhoneConfirmationTokenAsync(ApplicationUser userModel);

        Task<bool> DoesPhoneNumberExistAsync(string phone);

        Task<bool> DoesEmailExistAsync(string email);

        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);

        Task GenerateUsernameAsync(ApplicationUser user, DeviceParser device);

        Task LoadDefaultsAsync();

        Task<bool> IsAdminAsync(ApplicationUser user);

        Task<bool> IsAdminAsync(string username);

        Task MoveUserToAdminRoleAsync(ApplicationUser user);

        Task MoveUserToDefaultRoleAsync(ApplicationUser user);

        Task CreateRoleAsync(string name);

        Task CreateRoleAsync(string name, bool isAdmin);

        Task CreateRoleAsync(string name, bool isAdmin, string excludedURLComponents);

        Task CreateRoleAsync(string name, string icon, bool isAdmin);

        Task DeleteRoleAsync(string name);

        Task<string> GetRoleExcludedURLComponentsAsync(string name);

        Task<List<ApplicationUser>> GetAllUsersInRoleAsync(string name);

        Task MoveUserToRoleByNameAsync(ApplicationUser user, string name);

        Task<string> GetPrimaryRoleAsync(ApplicationUser user);

        Task<ApplicationUser> GetFromUserClaimAsync(ClaimsPrincipal claimsPrincipal);

        Task<ApplicationUser> GetUserByNameAsync(string name);

        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        Task<bool> HasPhoneNumberAsync(ApplicationUser user);

        Task<bool> HasConfirmedPhoneNumberAsync(ApplicationUser user);

        Task UpdatePhoneAsync(ApplicationUser user, string number);

        Task<IdentityResult> UpdatePasswordAsync(ApplicationUser user, string currPassword, string newPassword);

        Task<bool> IsPasswordValidAsync(ApplicationUser user, string pass);

        Task<ApplicationUser> GetUserFromContextAsync(ClaimsPrincipal claims);

        Task<string> GetRoleIconAsync(ClaimsPrincipal claims);

        Task<string> GetRoleIconAsync(string name);

        Task UpdateLastLoginAsync(ApplicationUser user);

        Task UpdateLastLoginAsync(string userName);
    }
}