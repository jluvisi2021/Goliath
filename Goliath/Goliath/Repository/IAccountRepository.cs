﻿using Goliath.Helper;
using Goliath.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Goliath.Repository
{
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

        Task CreateSuperUser();

        Task<bool> IsAdmin(ApplicationUser user);

        Task MoveUserToAdminRole(ApplicationUser user);

        Task MoveUserToDefaultRole(ApplicationUser user);
    }

}