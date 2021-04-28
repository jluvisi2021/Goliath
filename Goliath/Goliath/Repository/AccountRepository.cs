using Goliath.Data;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Goliath.Repository
{
    public class AccountRepository : IAccountRepository
    {
        /// <summary>
        /// The object which manages interacting directly with the Identity core methods for the user.
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Manages the sign in process for the user and can check the state of the user.
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        /// <summary>
        /// The object which can send direct emails to clients using HTML templates.
        /// </summary>
        private readonly IEmailService _emailService;

        /// <summary>
        /// The configuration for <b> appsettings.json </b>
        /// </summary>
        private readonly IConfiguration _config;

        private readonly ITwilioService _twilio;

        private readonly GoliathContext _context;

        /// <summary>
        /// The account repository is used for directly interacting with the ApplicationUser class.
        /// </summary>
        /// <param name="userManager"> </param>
        /// <param name="signInManager"> </param>
        /// <param name="emailService"> </param>
        /// <param name="config"> </param>
        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IEmailService emailService, IConfiguration config, ITwilioService twilio, GoliathContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _config = config;
            _twilio = twilio;
            _context = context;

        }

        /// <summary>
        /// Creates a "Super User" which can manage all roles in the panel for all users. Also
        /// creates the default roles.
        /// </summary>
        /// <returns> </returns>
        public async Task LoadDefaultsAsync()
        {
            if (!(await _roleManager.RoleExistsAsync(GoliathRoles.Administrator)))
            {
                await _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = GoliathRoles.Administrator,
                    Icon = HttpUtility.HtmlEncode("<span class='badge badge-pill badge-danger ml-1'>ADMIN</span>"),
                    IsAdministrator = true
                });
                await _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = GoliathRoles.Default,
                    Icon = ""
                });
                IdentityResult result = await _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = _config["SuperUser:Username"],
                    Email = _config["SuperUser:Email"],
                    EmailConfirmed = true,
                    BackgroundColor = "#FFFFFF",
                    DarkTheme = "false",
                    UserData = "",
                    PendingNotifications = "",
                    LogoutThreshold = 0,
                    AccountCreationDate = DateTime.UtcNow.ToString(),
                    LastPasswordUpdate = DateTime.UtcNow.ToString()
                },
                password: _config["SuperUser:Password"]
                );
                if (result.Succeeded)
                {
                    ApplicationUser superUser = await _userManager.FindByNameAsync("GoliathAdmin");

                    await _userManager.AddToRoleAsync(superUser, GoliathRoles.Administrator);
                }
                else
                {
                    GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "Could not create super user.");
                    return;
                }
                GoliathHelper.PrintDebugger("Created Super User.");
            }
        }

        /// <summary>
        /// Returns whether or not the user has the admin role.
        /// </summary>
        /// <param name="user"> </param>
        /// <returns> If the user is admin. </returns>
        public async Task<bool> IsAdminAsync(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                if ((await _roleManager.FindByNameAsync(role)).IsAdministrator)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> IsAdminAsync(string username)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                if ((await _roleManager.FindByNameAsync(role)).IsAdministrator)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<string> GetPrimaryRoleAsync(ApplicationUser user)
        {
            if ((await _userManager.GetRolesAsync(user)).Count != 0)
            {
                return (await _userManager.GetRolesAsync(user))[0];
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes all roles from a user and makes them an admin.
        /// </summary>
        /// <param name="user"> </param>
        /// <returns> </returns>
        public async Task MoveUserToAdminRoleAsync(ApplicationUser user)
        {
            if (!await _userManager.IsInRoleAsync(user, GoliathRoles.Administrator))
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRoleAsync(user, GoliathRoles.Administrator);
                IList<string> s = await _userManager.GetRolesAsync(user);
                await SendRoleMovedEmailAsync(user, s[0].ToString(), GoliathRoles.Administrator);
            }
        }

        /// <summary>
        /// Removes all roles from a user and makes them default.
        /// </summary>
        /// <param name="user"> </param>
        /// <returns> </returns>
        public async Task MoveUserToDefaultRoleAsync(ApplicationUser user)
        {
            if (!await _userManager.IsInRoleAsync(user, GoliathRoles.Default))
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRoleAsync(user, GoliathRoles.Default);
                IList<string> s = await _userManager.GetRolesAsync(user);
                await SendRoleMovedEmailAsync(user, s[0].ToString(), GoliathRoles.Administrator);
            }
        }

        public async Task CreateRoleAsync(string name)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name
            });
        }

        public async Task CreateRoleAsync(string name, bool isAdmin)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name,
                IsAdministrator = isAdmin
            });
        }

        public async Task CreateRoleAsync(string name, bool isAdmin, string excludedURLComponents)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name,
                IsAdministrator = isAdmin,
                ExcludedURLComponents = excludedURLComponents
            });
        }

        public async Task CreateRoleAsync(string name, string icon, bool isAdmin)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name,
                IsAdministrator = isAdmin,
                Icon = icon
            });
        }

        public async Task DeleteRoleAsync(string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                await _roleManager.DeleteAsync(await _roleManager.FindByNameAsync(name));
            }
        }

        public async Task<string> GetRoleIconAsync(string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                return (await _roleManager.FindByNameAsync(name)).Icon;
            }
            return "Error";
        }

        /// <summary>
        /// Get the icon for a specific role using a user claim.
        /// </summary>
        /// <param name="claims"> </param>
        /// <returns> </returns>
        public async Task<string> GetRoleIconAsync(ClaimsPrincipal claims)
        {
            return (await _roleManager.FindByNameAsync(await GetPrimaryRoleAsync(await GetUserFromContextAsync(claims)))).Icon;
        }

        public async Task<string> GetRoleExcludedURLComponentsAsync(string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                return (await _roleManager.FindByNameAsync(name)).ExcludedURLComponents;
            }
            return "Error";
        }

        public async Task<List<ApplicationUser>> GetAllUsersInRoleAsync(string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                return (List<ApplicationUser>)(await _userManager.GetUsersInRoleAsync(name));
            }
            return null;
        }

        /// <summary>
        /// Move a user to a specified role.
        /// </summary>
        /// <param name="user"> </param>
        /// <param name="name"> </param>
        /// <returns> </returns>
        public async Task MoveUserToRoleByNameAsync(ApplicationUser user, string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                if (!await _userManager.IsInRoleAsync(user, name))
                {
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    await _userManager.AddToRoleAsync(user, name);
                    IList<string> s = await _userManager.GetRolesAsync(user);
                    await SendRoleMovedEmailAsync(user, s[0].ToString(), GoliathRoles.Administrator);
                }
            }
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        /////////////////////////////////////////////

        /// <summary>
        /// Returns an application user from a claims principal.
        /// </summary>
        /// <param name="claimsPrincipal"> </param>
        /// <returns> </returns>
        public async Task<ApplicationUser> GetFromUserClaimAsync(ClaimsPrincipal claimsPrincipal)
        {
            return await _userManager.FindByNameAsync(claimsPrincipal.Identity.Name);
        }

        /// <summary> The correct way to get a user from a view. </summary> <param
        /// name="claims">@User.<></param> <returns>Get a user from a view.</returns>
        public async Task<ApplicationUser> GetUserFromContextAsync(ClaimsPrincipal claims)
        {
            return await _userManager.GetUserAsync(claims);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<bool> IsPasswordValidAsync(ApplicationUser user, string pass)
        {
            return await _userManager.CheckPasswordAsync(user, pass);
        }

        public async Task<IdentityResult> UpdatePasswordAsync(ApplicationUser user, string currPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currPassword, newPassword);
        }

        public async Task<bool> HasPhoneNumberAsync(ApplicationUser user)
        {
            return ((await _userManager.GetPhoneNumberAsync(user)) != null);
        }

        public async Task<bool> HasConfirmedPhoneNumberAsync(ApplicationUser user)
        {
            return await _userManager.IsPhoneNumberConfirmedAsync(user);
        }

        public async Task UpdatePhoneAsync(ApplicationUser user, string number)
        {
            await _userManager.SetPhoneNumberAsync(user, number);
        }

        public async Task<bool> DoesEmailExistAsync(string email)
        {
            ApplicationUser existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (existingEmail == null)
            {
                return false;
            }
            return true;
        }

        public async Task SetTwoFactorEnabledAsync(ApplicationUser user, TwoFactorMethod method)
        {
            user.TwoFactorMethod = (int)method;
            user.TwoFactorUpdated = DateTime.UtcNow.ToString();
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            await _userManager.UpdateAsync(user);
        }

        public async Task SetTwoFactorDisabledAsync(ApplicationUser user)
        {
            user.TwoFactorMethod = (int)TwoFactorMethod.None;
            user.TwoFactorUpdated = DateTime.UtcNow.ToString();
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.UpdateAsync(user);
        }

        public async Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token)
        {
            return await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultPhoneProvider, token, false, false);
        }

        public async Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token, bool rememberMe)
        {
            return await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultPhoneProvider, token, rememberMe, false);
        }

        public async Task SendTwoFactorCodeSms(ApplicationUser user)
        {
            string code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            await _twilio.SendSmsAsync(new SMSTextModel()
            {
                Message = $"Your Two-Factor Code is: {code}.",
                To = user.PhoneNumber
            });
        }
        public async Task<List<string>> GenerateUserRecoveryCodesAsync(ApplicationUser user)
        {
            return (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToList();
        }

        public async Task<int> GetUserRecoveryCodeAmountAsync(ApplicationUser user)
        {
            return await _userManager.CountRecoveryCodesAsync(user);
        }

        public async Task<IdentityResult> RedeemRecoveryCodeAsync(ApplicationUser user, string code)
        {
            return await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, code);
        }

        public async Task GenerateTwoFactorCode(ApplicationUser user)
        {
            await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
        }

        public async Task<bool> TwoFactorCodeValidAsync(ApplicationUser user, string token)
        {
            if (user.TwoFactorEnabled && user.TwoFactorMethod != (int)TwoFactorMethod.None)
            {
                return await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, token);
            }
            return false;
        }

        public async Task<bool> DoesPhoneNumberExistAsync(string phone)
        {
            ApplicationUser existingEmail = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber.ToLower() == phone.ToLower());
            if (existingEmail == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates the user and adds them to the database using Identity core.
        /// </summary>
        /// <param name="userModel"> </param>
        /// <param name="data"> Contains some data that should be presented in the email. </param>
        /// <returns> </returns>
        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel, DeviceParser device)
        {
            try
            {
                // Create a new application user.
                ApplicationUser user = new()
                {
                    BackgroundColor = "#FFFFFF",
                    DarkTheme = "false",
                    UserData = "",
                    PendingNotifications = "",
                    UserName = userModel.Username,
                    Email = userModel.Email,
                    LogoutThreshold = 0,
                    AccountCreationDate = DateTime.UtcNow.ToString(),
                    LastPasswordUpdate = DateTime.UtcNow.ToString()
                };

                // Use Identity Core to create the user.
                IdentityResult result = await (_userManager.CreateAsync(user, userModel.Password));
                if (result.Succeeded)
                {
                    // add them to default role.
                    await _userManager.AddToRoleAsync(user, GoliathRoles.Default);
                    // Send them a token.
                    await GenerateEmailConfirmationTokenAsync(userModel, user, device);
                }

                return result;
            }
            catch (Exception e)
            {
                GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "CreateUserAsync: Failed to execute.");
                return IdentityResult.Failed(
                    new IdentityError()
                    {
                        Code = e.TargetSite.Name,
                        Description = "Could not create account."
                    });
            }
        }

        /// <summary>
        /// Updates the "LastUserLogin" with DateTime.UtcNow.ToString()
        /// </summary>
        /// <returns> </returns>
        public async Task UpdateLastLoginAsync(ApplicationUser user)
        {
            user.LastUserLogin = DateTime.UtcNow.ToString();
            await UpdateUserAsync(user);
        }

        /// <summary>
        /// Updates the "LastUserLogin" with DateTime.UtcNow.ToString()
        /// </summary>
        /// <returns> </returns>
        public async Task UpdateLastLoginAsync(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            user.LastUserLogin = DateTime.UtcNow.ToString();
            await UpdateUserAsync(user);
        }

        /// <summary>
        /// Sends an email to a client with a generated token. <br /> This version of the method
        /// should only be used at registration due to its dependency of SignUpUserModel
        /// </summary>
        /// <param name="signUpModel"> </param>
        /// <param name="userModel"> </param>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public async Task GenerateEmailConfirmationTokenAsync(SignUpUserModel signUpModel, ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await SendEmailConfirmationTokenAsync(signUpModel, userModel, device, token);
            }
        }

        /// <summary>
        /// Sends an email to the user with a token.
        /// </summary>
        /// <param name="user"> </param>
        /// <returns> </returns>
        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await ResendEmailConfirmationTokenAsync(userModel, device, token);
            }
        }

        /// <summary>
        /// Sends an email to a user notifying them to verify their new email. Also sends an email
        /// to the user's old email notifying them of the change.
        /// </summary>
        /// <param name="userModel"> </param>
        /// <param name="device"> </param>
        /// <returns> </returns>
        public async Task GenerateNewEmailConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await SendNewEmailConfirmationTokenAsync(userModel, device, token);
                await SendNotifyOldEmailAsync(userModel, device);
            }
        }

        /// <summary>
        /// Generate a new token for a user's phone and send it to them. Also alerts the user's email.
        /// </summary>
        /// <param name="userModel"> </param>
        /// <param name="device"> </param>
        /// <returns> </returns>
        public async Task GenerateNewPhoneConfirmationTokenAsync(ApplicationUser userModel, DeviceParser device)
        {
            string token = await _userManager.GenerateChangePhoneNumberTokenAsync(userModel, userModel.UnverifiedNewPhone);
            if (!string.IsNullOrWhiteSpace(token))
            {
                await _twilio.SendSmsAsync(new SMSTextModel()
                {
                    To = userModel.UnverifiedNewPhone,
                    Message = $"This number has been request to be the new phone number for Goliath Account: {userModel.UserName}. Enter this number in the userpanel to confirm it: {token}"
                });
                await SendNewPhoneEmailAsync(userModel, device);
            }
        }

        /// <summary>
        /// Generates a token to a user's phone. Does not send an email alerting the user of the resend.
        /// </summary>
        /// <param name="userModel"> </param>
        /// <returns> </returns>
        public async Task GenerateNewPhoneConfirmationTokenAsync(ApplicationUser userModel)
        {
            string token = await _userManager.GenerateChangePhoneNumberTokenAsync(userModel, userModel.UnverifiedNewPhone);
            if (!string.IsNullOrWhiteSpace(token))
            {
                await _twilio.SendSmsAsync(new SMSTextModel()
                {
                    To = userModel.UnverifiedNewPhone,
                    Message = $"This number has been request to be the new phone number for Goliath Account: {userModel.UserName}. Enter this number in the userpanel to confirm it: {token}"
                });
            }
        }

        /// <summary>
        /// Sends an email to a client with their username.
        /// </summary>
        /// <param name="model"> </param>
        /// <param name="device"> </param>
        /// <returns> </returns>
        public async Task GenerateUsernameAsync(ApplicationUser user, DeviceParser device)
        {
            if (!(string.IsNullOrWhiteSpace(user?.UserName)))
            {
                await SendEmailWithUsernameAsync(user, device);
            }
        }

        /// <summary>
        /// Sends an email to the user with a token to reset password.
        /// </summary>
        /// <param name="userModel"> </param>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public async Task GenerateForgotPasswordTokenAsync(ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GeneratePasswordResetTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await SendForgotPasswordTokenAsync(userModel, device, token);
            }
        }

        /// <summary>
        /// Returns the application user which is found from a specified email address.
        /// </summary>
        /// <param name="email"> Email Address </param>
        /// <returns> Application User (async) </returns>
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Sign in to the application using the sign in model.
        /// </summary>
        /// <param name="signInModel"> </param>
        /// <returns> </returns>
        public async Task<SignInResult> PasswordSignInAsync(SignInModel signInModel)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, signInModel.RememberMe, false);

            return result;
        }

        /// <summary>
        /// Manage signing out of the application.
        /// </summary>
        /// <returns> </returns>
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Converts a string to asterisk characters.
        /// </summary>
        /// <param name="str"> </param>
        /// <returns> </returns>
        private static string ConvertToAstrisk(string str)
        {
            StringBuilder j = new();
            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                j.Append('*');
            }
            return j.ToString();
        }

        /// <summary>
        /// Send an email to a user with a confirmation token as well as information about the
        /// computer sending the email. <br /> This method does not use the SignUp model. [USE FOR RESENDS]
        /// </summary>
        /// <param name="user"> </param>
        /// <param name="computer"> User Agent </param>
        /// <param name="ip"> IPv4 Address </param>
        /// <param name="token"> </param>
        /// <returns> </returns>
        private async Task ResendEmailConfirmationTokenAsync(ApplicationUser user, DeviceParser device, string token)
        {
            // Get the information required to send the email from the appsettings.json.
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];
            // Send an email while replacing all placeholders.
            await _emailService.ResendConfirmationEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{Link}}", string.Format(appDomain + verifyLink, user.Id, token)
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    }
                        }
            });
        }

        private async Task SendNewPhoneEmailAsync(ApplicationUser user, DeviceParser device)
        {
            // Check if the phone number is null.
            string phone;
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                phone = "Not Specified.";
            }
            else
            {
                phone = user.PhoneNumber;
            }

            // Generate email with placeholders.
            await _emailService.SendVerifyPhoneEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    },
                    {
                        "{{PhoneNumber}}", phone
                    },
                    {
                        "{{NewPhone}}", user.UnverifiedNewPhone
                    },
                    {
                        "{{AccountCreationDate}}", user.AccountCreationDate
                    },
                    {
                        "{{LastLogin}}", user.LastUserLogin
                    },
                        }
            });
        }

        /// <summary>
        /// Sends an email to a client with a link to reset their password.
        /// </summary>
        /// <param name="user"> </param>
        /// <param name="computer"> </param>
        /// <param name="ip"> </param>
        /// <param name="token"> </param>
        /// <returns> </returns>
        private async Task SendForgotPasswordTokenAsync(ApplicationUser user, DeviceParser device, string token)
        {
            // Get the information required to send the email from the appsettings.json.
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:ForgotPassword"];
            // Send an email while replacing all placeholders.
            await _emailService.SendForgotPasswordEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{Link}}", string.Format(appDomain + verifyLink, user.Id, token)
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    }
                        }
            });
        }

        /// <summary>
        /// Send an email to a user with a confirmation token as well as information about the
        /// computer sending the email.
        /// </summary>
        /// <param name="signUpModel"> Register Model. </param>
        /// <param name="user"> Current client. </param>
        /// <param name="computer"> UserAgent info. </param>
        /// <param name="ip"> Mapped IPv4 address. </param>
        /// <param name="token"> Generated .NET Core token. </param>
        /// <returns> </returns>
        private async Task SendEmailConfirmationTokenAsync(SignUpUserModel signUpModel, ApplicationUser user, DeviceParser device, string token)
        {
            // Get values from appsettings.json
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];
            // Generate email with placeholders.
            await _emailService.SendConfirmationEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{Password}}", ConvertToAstrisk(signUpModel.Password)
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{Link}}", string.Format(appDomain + verifyLink, user.Id, token)
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    }
                        }
            });
        }

        /// <summary>
        /// Sends an email to a user's new email address when they update their email address asking
        /// them to verify.
        /// </summary>
        /// <param name="signUpModel"> </param>
        /// <param name="user"> </param>
        /// <param name="device"> </param>
        /// <param name="token"> </param>
        /// <returns> </returns>
        private async Task SendNewEmailConfirmationTokenAsync(ApplicationUser user, DeviceParser device, string token)
        {
            // Get values from appsettings.json
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];

            string phone;
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                phone = "Not Specified.";
            }
            else
            {
                phone = user.PhoneNumber;
            }

            // Generate email with placeholders.
            await _emailService.SendConfirmNewEmailAsync(new()
            {
                // send the token to the user's new unverified email.
                ToEmails = new List<string>() { user.UnverifiedNewEmail },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{Link}}", string.Format(appDomain + verifyLink, user.Id, token)
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    },
                    {
                        "{{PhoneNumber}}", phone
                    },
                    {
                        "{{AccountCreationDate}}", user.AccountCreationDate
                    },
                    {
                        "{{LastLogin}}", user.LastUserLogin
                    },
                        }
            });
        }

        /// <summary>
        /// Send an email to a user's old email account when they change their email.
        /// </summary>
        /// <param name="user"> </param>
        /// <param name="device"> </param>
        /// <returns> </returns>
        private async Task SendNotifyOldEmailAsync(ApplicationUser user, DeviceParser device)
        {
            // Check if the phone number is null.
            string phone;
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                phone = "Not Specified.";
            }
            else
            {
                phone = user.PhoneNumber;
            }

            // Generate email with placeholders.
            await _emailService.SendNotifyOldEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.UnverifiedNewEmail
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    },
                    {
                        "{{PhoneNumber}}", phone
                    },
                    {
                        "{{AccountCreationDate}}", user.AccountCreationDate
                    },
                    {
                        "{{LastLogin}}", user.LastUserLogin
                    },
                        }
            });
        }

        private async Task SendEmailWithUsernameAsync(ApplicationUser user, DeviceParser device)
        {
            // Generate email with placeholders.
            await _emailService.SendForgotUsernameEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{IPAddress}}", device.IPv4
                    },
                    {
                        "{{ComputerInfo}}", device.ToSimpleString()
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    }
                        }
            });
        }

        private async Task SendRoleMovedEmailAsync(ApplicationUser user, string previousRole, string currentRole)
        {
            // Generate email with placeholders.
            await _emailService.SendForgotUsernameEmailAsync(new()
            {
                ToEmails = new List<string>() { user.Email },
                Placeholders = new Dictionary<string, string> {
                    {
                        "{{Username}}", user.UserName
                    },
                    {
                        "{{Email}}", user.Email
                    },
                    {
                        "{{PreviousRole}}", previousRole
                    },
                    {
                        "{{CurrentRole}}", currentRole
                    },
                    {
                        "{{DateTime}}", DateTime.UtcNow.ToString()
                    }
                        }
            });
        }

        /// <summary>
        /// Confirms that a clients email and token match.
        /// </summary>
        /// <param name="uid"> </param>
        /// <param name="token"> </param>
        /// <returns> </returns>
        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            // If the email cannot be confirmed then return a failed attempt.
            try
            {
                // Get the user from the URL
                ApplicationUser user = await _userManager.FindByIdAsync(uid);
                // Check if the user is attempting to change their email.
                if (!string.IsNullOrWhiteSpace(user.UnverifiedNewEmail))
                {
                    user.Email = user.UnverifiedNewEmail;
                    user.EmailConfirmed = false;
                }
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    // Set the unverified email of the user to nothing.
                    user.UnverifiedNewEmail = string.Empty;

                    await UpdateUserAsync(user); // Update the user in database.

                    return IdentityResult.Success;
                }
                else
                {
                    // If the result failed then do not update any of the user data.
                    return IdentityResult.Failed();
                }
            }
            catch (Exception)
            {
                GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "ConfirmEmailAsync: Failed to execute.");
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> ConfirmPhoneAsync(ApplicationUser user, string token)
        {
            if (!string.IsNullOrWhiteSpace(user.UnverifiedNewPhone))
            {
                if (await _userManager.VerifyChangePhoneNumberTokenAsync(user, token, user.UnverifiedNewPhone))
                {
                    await _userManager.ChangePhoneNumberAsync(user, user.UnverifiedNewPhone, token);

                    return IdentityResult.Success;
                }
            }
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            // If the email cannot be confirmed then return a failed attempt.
            try
            {
                return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
            }
            catch (Exception)
            {
                GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "ResetPasswordAsync: Failed to execute.");
                return IdentityResult.Failed();
            }
        }
    }
}