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
    /// The repository which manages the <see cref="ApplicationUser" />, <see cref="ApplicationRole"
    /// /> and <see cref="SignInManager{TUser}" />. Anything that is related to modifying a user's
    /// attributes, authorizing them, or interacting with their contact information is done through
    /// <see cref="IAccountRepository" />.
    /// <para>
    /// The account repository is also able to send emails to users using the <see
    /// cref="Services.IEmailService" />.
    /// </para>
    /// <para>
    /// <em> Assume that all methods under <see cref="IAccountRepository" /> are asynchronous unless
    /// otherwise specified. </em>
    /// </para>
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Attempts to create a user and then if successful sends a verification email to their account.
        /// </summary>
        /// <param name="userModel"> The model to create the user with. </param>
        /// <param name="device"> A <see cref="DeviceParser" /> object. </param>
        /// <returns> If the task was successful. </returns>
        Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel, DeviceParser device);

        /// <summary>
        /// Attempts to sign the user in using IdentityCore and a <paramref name="signInModel" />.
        /// </summary>
        /// <param name="signInModel"> The model to sign the user in with. </param>
        /// <param name="remoteIp"> The remoteIp address of the user. </param>
        /// <returns> If the task was successful. </returns>
        Task<SignInResult> PasswordSignInAsync(SignInModel signInModel, string remoteIp);

        /// <summary>
        /// Signs the current user out and removes confidential cookies.
        /// </summary>
        /// <returns> </returns>
        Task SignOutAsync();

        /// <summary>
        /// Confirms an email using a <paramref name="uid" /> as well as a confirmation <paramref
        /// name="token" />.
        /// </summary>
        /// <param name="uid"> UserId </param>
        /// <param name="token"> An email confirmation token. </param>
        /// <returns> If the task was successful. </returns>
        Task<IdentityResult> ConfirmEmailAsync(string uid, string token);

        /// <summary>
        /// Locates a user by their <paramref name="email" /> address.
        /// </summary>
        /// <param name="email"> The <paramref name="email" /> address to search for. </param>
        /// <returns> The <see cref="ApplicationUser" /> with the <paramref name="email" />. </returns>
        Task<ApplicationUser> FindByEmailAsync(string email);

        /// <summary>
        /// Locates a user by their <paramref name="userId" />.
        /// </summary>
        /// <param name="userId"> The <paramref name="userId" /> to search for. </param>
        /// <returns> The <see cref="ApplicationUser" /> with the <paramref name="userId" />. </returns>
        Task<ApplicationUser> FindUserById(string userId);

        /// <summary>
        /// Convert a user to a JSON string.
        /// </summary>
        /// <param name="user">The user to convert.</param>
        /// <returns>The serialized json.</returns>
        Task<string> UserToJsonAsync(ApplicationUser user);

        /// <summary>
        /// Sends an email to the user with a key to decrypt their data.
        /// </summary>
        /// <param name="userModel">Model of the user.</param>
        /// <param name="device">UserAgent</param>
        /// <param name="key">Key to send.</param>
        /// <returns></returns>
        Task GenerateNewDataEncryptionEmailAsync(ApplicationUser userModel, DeviceParser device, string key);

        /// <summary>
        /// Attempts to confirm a <paramref name="user" /> phone number using Identity Core.
        /// </summary>
        /// <param name="user"> The user object to confirm. </param>
        /// <param name="token"> A email confirmation token. </param>
        /// <returns> If the task was successful. </returns>
        Task<IdentityResult> ConfirmPhoneAsync(ApplicationUser user, string token);

        /// <summary>
        /// Sends an email to a user with a link to confirm their email.
        /// </summary>
        /// <param name="signUpModel"> Model used to confirm user. </param>
        /// <param name="userModel"> A <see cref="ApplicationUser" /> to confirm. </param>
        /// <param name="device"> The UserAgent. </param>
        /// <returns> </returns>
        Task GenerateEmailConfirmationTokenAsync(SignUpUserModel signUpModel, ApplicationUser userModel, DeviceParser device);

        /// <summary>
        /// Sends an email to a user with a link to confirm their email.
        /// </summary>
        /// <param name="userModel"> Model used to confirm user. </param>
        /// <param name="device"> The UserAgent. </param>
        /// <returns> </returns>
        Task GenerateEmailConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device);

        /// <param name="user"> The user object to check. </param>
        /// <param name="token"> The token the user inputs. </param>
        /// <returns>
        /// If the specified <paramref name="token" /> is a valid two-factor code for the <paramref
        /// name="user" />
        /// </returns>
        Task<bool> TwoFactorCodeValidAsync(ApplicationUser user, string token);

        /// <summary>
        /// Generates a new two-factor code in IdentityCore.
        /// </summary>
        /// <param name="user"> User to generate a code for. </param>
        /// <returns> </returns>
        Task GenerateTwoFactorCode(ApplicationUser user);

        /// <summary>
        /// Attempts to authorize a user using a two factor code.
        /// </summary>
        /// <param name="user"> The user to authorize. </param>
        /// <param name="token"> The token the user has input. </param>
        /// <param name="remoteIp"> The remote ip address of the user. </param>
        /// <returns> If the task is successful. </returns>
        Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token, string remoteIp);

        /// <summary>
        /// Attempts to authorize a user using a two factor code.
        /// </summary>
        /// <param name="user"> The user to authorize. </param>
        /// <param name="token"> The token the user has input. </param>
        /// <param name="remoteIp"> The remote ip address of the user. </param>
        /// <param name="rememberMe"> If this two-factor authorization should be cached. </param>
        /// <returns> If the task is successful. </returns>
        Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token, string remoteIp, bool rememberMe);

        /// <summary>
        /// Sends a two-factor code to a <paramref name="user" /> through their phone number.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to send the code to. </param>
        /// <returns> </returns>
        Task SendTwoFactorCodeSms(ApplicationUser user);

        /// <param name="user"> The <paramref name="user" /> to get two-factor codes for. </param>
        /// <returns> A list of valid one-time-use two-factor codes. </returns>
        Task<List<string>> GenerateUserRecoveryCodesAsync(ApplicationUser user);

        /// <summary>
        /// Disables two-factor authentication for a <paramref name="user" />.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to disable two-factor for. </param>
        /// <returns> </returns>
        Task SetTwoFactorDisabledAsync(ApplicationUser user);

        /// <summary>
        /// Enables two-factor authentication for a <paramref name="user" />.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to enable two-factor for. </param>
        /// <param name="method">
        /// The method of two-factor that the <paramref name="user" /> is using.
        /// </param>
        /// <returns> </returns>
        Task SetTwoFactorEnabledAsync(ApplicationUser user, TwoFactorMethod method);

        /// <param name="user"> The <paramref name="user" /> to query. </param>
        /// <returns> The amount of two-factor recovery codes the <paramref name="user" /> has. </returns>
        Task<int> GetUserRecoveryCodeAmountAsync(ApplicationUser user);

        /// <summary>
        /// Attempts to redeem a single-use two-factor code.
        /// </summary>
        /// <param name="user">
        /// The <paramref name="user" /> who has input the <paramref name="code" />.
        /// </param>
        /// <param name="code"> The two-factor code. </param>
        /// <returns> If the recovery code is valid. </returns>
        Task<IdentityResult> RedeemRecoveryCodeAsync(ApplicationUser user, string code);

        /// <summary>
        /// Sends a email to a user with a link for them to reset their password.
        /// </summary>
        /// <param name="userModel"> The user to send the code to. </param>
        /// <param name="device"> The UserAgent. </param>
        /// <returns> </returns>
        Task GenerateForgotPasswordTokenAsync(ApplicationUser userModel, DeviceParser device);

        /// <summary>
        /// Sends a email to a user with a link to confirm a new email address.
        /// </summary>
        /// <param name="userModel"> The user to send the code to. </param>
        /// <param name="device"> The UserAgent. </param>
        /// <returns> </returns>
        Task GenerateNewEmailConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device);

        /// <summary>
        /// Sends a SMS text message to a user with a new verification code as well as sends an
        /// email to the users email address alerting them of the change.
        /// </summary>
        /// <param name="userModel"> The user to verify. </param>
        /// <param name="device"> The UserAgent. </param>
        /// <returns> </returns>
        Task GenerateNewPhoneConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device);

        /// <summary>
        /// Sends a SMS text message to a user with a new verification code.
        /// </summary>
        /// <param name="userModel"> The user to verify. </param>
        /// <returns> </returns>
        Task GenerateNewPhoneConfirmationTokenAsync(ApplicationUser userModel);

        /// <param name="phone"> A string representing a phone number. </param>
        /// <returns> If <paramref name="phone" /> is a valid phone number that is in use. </returns>
        Task<bool> DoesPhoneNumberExistAsync(string phone);

        /// <param name="email"> A string representing a email address. </param>
        /// <returns> If the <paramref name="email" /> is a valid email address that is in use. </returns>
        Task<bool> DoesEmailExistAsync(string email);

        /// <summary>
        /// Resets the user's password using the values from the <paramref name="model" />.
        /// </summary>
        /// <param name="model"> The <paramref name="model" /> to reset the user's password with. </param>
        /// <returns> If the task is successful. </returns>
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);

        /// <summary>
        /// Sends a email to a user with their username.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to send the email to. </param>
        /// <param name="device"> The UserAgent. </param>
        /// <returns> </returns>
        Task GenerateUsernameAsync(ApplicationUser user, DeviceParser device);

        /// <summary>
        /// A method to be run at startup. <br /> Creates the default GoliathAdmin account as well
        /// as creates any necessary <see cref="ApplicationRole" />.
        /// </summary>
        /// <returns> </returns>
        Task LoadDefaultsAsync();

        /// <summary>
        /// Creates a set <paramref name="amount" /> of dummy accounts and adds them into the
        /// database with default role. This should only be used for debugging and testing the speed
        /// of EF Core.
        /// </summary>
        /// <returns> </returns>
        Task AddTestingDataAsync(int amount);

        /// <param name="user"> The user to check. </param>
        /// <returns> If a <paramref name="user" /> is an administrator. </returns>
        Task<bool> IsAdminAsync(ApplicationUser user);

        /// <param name="username"> The username to query. </param>
        /// <returns> If the user with the <paramref name="username" /> is an administrator. </returns>
        Task<bool> IsAdminAsync(string username);

        /// <summary>
        /// Moves a user to <see cref="ApplicationRole" /> with administrative privileges.
        /// </summary>
        /// <param name="user"> The user to move. </param>
        /// <returns> </returns>
        Task MoveUserToAdminRoleAsync(ApplicationUser user);

        /// <summary>
        /// Moves a user to a <see cref="ApplicationRole" /> with default permissions.
        /// </summary>
        /// <param name="user"> The user to move. </param>
        /// <returns> </returns>
        Task MoveUserToDefaultRoleAsync(ApplicationUser user);

        /// <summary>
        /// Creates a <see cref="ApplicationRole" />.
        /// </summary>
        /// <param name="name"> The name of the role. </param>
        /// <returns> </returns>
        Task CreateRoleAsync(string name);

        /// <summary>
        /// Creates a <see cref="ApplicationRole" />.
        /// </summary>
        /// <param name="name"> The name of the role. </param>
        /// <param name="isAdmin"> If the role should have administrative privileges. </param>
        /// <returns> </returns>
        Task CreateRoleAsync(string name, bool isAdmin);

        /// <summary>
        /// Creates a <see cref="ApplicationRole" />.
        /// </summary>
        /// <param name="name"> The name of the role. </param>
        /// <param name="isAdmin"> If the role should have administrative privileges. </param>
        /// <param name="excludedURLComponents">
        /// The url components the user is not allowed to visit.
        /// </param>
        /// <returns> </returns>
        Task CreateRoleAsync(string name, bool isAdmin, string excludedURLComponents);

        /// <summary>
        /// Creates a <see cref="ApplicationRole" />.
        /// </summary>
        /// <param name="name"> The name of the role. </param>
        /// <param name="icon"> The icon to display for the role. </param>
        /// <param name="isAdmin"> If the role should have administrative privileges. </param>
        /// <returns> </returns>
        Task CreateRoleAsync(string name, string icon, bool isAdmin);

        /// <summary>
        /// Finds and deletes a role with the specified <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The name of the role to query. </param>
        /// <returns> </returns>
        Task DeleteRoleAsync(string name);

        /// <param name="name"> The name of the role to query. </param>
        /// <returns> The excluded URL components for a role. </returns>
        Task<string> GetRoleExcludedURLComponentsAsync(string name);

        /// <summary>
        /// Returns a <see cref="List{ApplicationUser}" /> of all users in the specified <see
        /// cref="ApplicationRole" /> which has the name of <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The name of the role. </param>
        /// <returns> All users in the specified <see cref="ApplicationRole" />. </returns>
        Task<List<ApplicationUser>> GetAllUsersInRoleAsync(string name);

        /// <summary>
        /// Moves a <paramref name="user" /> to a <see cref="ApplicationRole" /> with name of
        /// <paramref name="name" />.
        /// </summary>
        /// <param name="user"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        Task MoveUserToRoleByNameAsync(ApplicationUser user, string name);

        /// <summary>
        /// Gets the first role of the <paramref name="user" />.
        /// </summary>
        /// <param name="user"> The user to query. </param>
        /// <returns> The name of the first role. </returns>
        Task<string> GetPrimaryRoleAsync(ApplicationUser user);

        /// <summary>
        /// Gets a user from a <see cref="ClaimsPrincipal" />.
        /// </summary>
        /// <param name="claimsPrincipal"> A <see cref="ClaimsPrincipal" /> object. </param>
        /// <returns> A user. </returns>
        Task<ApplicationUser> GetFromUserClaimAsync(ClaimsPrincipal claimsPrincipal);

        /// <param name="name"> The <paramref name="name" /> to search for. </param>
        /// <returns> A user with the name of <paramref name="name" />. </returns>
        Task<ApplicationUser> GetUserByNameAsync(string name);

        /// <summary>
        /// Updates the values that have been changed for a specified <paramref name="user" />.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to update. </param>
        /// <returns> If the task completed. </returns>
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        /// <param name="user"> The <paramref name="user" /> to query. </param>
        /// <returns> If the user has a phone number. </returns>
        Task<bool> HasPhoneNumberAsync(ApplicationUser user);

        /// <param name="user"> The <paramref name="user" /> to query. </param>
        /// <returns> If the user has a confirmed phone number. </returns>
        Task<bool> HasConfirmedPhoneNumberAsync(ApplicationUser user);

        /// <summary>
        /// Updates the user's phone number.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to update. </param>
        /// <param name="number"> The new phone <paramref name="number" /> of the user. </param>
        /// <returns> </returns>
        Task UpdatePhoneAsync(ApplicationUser user, string number);

        /// <summary>
        /// Updates the password of a specified <paramref name="user" />.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to update. </param>
        /// <param name="currPassword"> The current password of the <paramref name="user" />. </param>
        /// <param name="newPassword"> The new password to set. </param>
        /// <returns> If the task completed. </returns>
        Task<IdentityResult> UpdatePasswordAsync(ApplicationUser user, string currPassword, string newPassword);

        /// <param name="user"> The user to query. </param>
        /// <param name="pass"> The password to compare against. </param>
        /// <returns>
        /// If <paramref name="pass" /> is a valid password for <paramref name="user" />.
        /// </returns>
        Task<bool> IsPasswordValidAsync(ApplicationUser user, string pass);

        /// <summary>
        /// Remove the login traceback data from the user.
        /// </summary>
        /// <param name="user">The <paramref name="user"/> to remove the data from.</param>
        /// <returns></returns>
        Task ClearLoginTracebackAsync(ApplicationUser user);

        /// <summary>
        /// Gets a user from a <see cref="ClaimsPrincipal" />.
        /// </summary>
        /// <param name="claims"> The <see cref="ClaimsPrincipal" /> to find the user with. </param>
        /// <returns> A user. </returns>
        Task<ApplicationUser> GetUserFromContextAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Gets the decoded Html of a user's icon from a <see cref="ClaimsPrincipal" />
        /// </summary>
        /// <param name="claims"> The <see cref="ClaimsPrincipal" /> to find the user with. </param>
        /// <returns> The Html icon. </returns>
        Task<string> GetRoleIconAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Gets the Html decoded Icon for a user with the name of <paramref name="name" />.
        /// </summary>
        /// <param name="name"> The name to query. </param>
        /// <returns> The Html icon. </returns>
        Task<string> GetRoleIconAsync(string name);

        /// <summary>
        /// Updates the last login of the <paramref name="user" />.
        /// </summary>
        /// <param name="user"> The <paramref name="user" /> to update. </param>
        /// <returns> </returns>
        Task UpdateLastLoginAsync(ApplicationUser user);

        /// <summary>
        /// Updates the last login of the <paramref name="user" />.
        /// </summary>
        /// <param name="userName"> The username of the <see cref="ApplicationUser" /> to update. </param>
        Task UpdateLastLoginAsync(string userName);
    }
}