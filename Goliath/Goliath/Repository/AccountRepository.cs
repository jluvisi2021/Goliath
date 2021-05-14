using Goliath.Data;
using Goliath.Enums;
using Goliath.Helper;
using Goliath.Models;
using Goliath.Models.Accounts;
using Goliath.Models.Extra;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Goliath.Repository
{
    /// <inheritdoc cref="IAccountRepository" />
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly ITwilioService _twilio;
        private readonly ICookieManager _cookieManager;
        private readonly GoliathContext _context;
        private readonly ILogger _logger;

        /// <summary>
        /// The account repository is used for directly interacting with the ApplicationUser class.
        /// </summary>
        /// <param name="userManager"> </param>
        /// <param name="signInManager"> </param>
        /// <param name="emailService"> </param>
        /// <param name="config"> </param>
        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IEmailService emailService, IConfiguration config, ITwilioService twilio, ICookieManager cookieManager, GoliathContext context, ILogger<AccountRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _cookieManager = cookieManager;
            _config = config;
            _twilio = twilio;
            _context = context;
            _logger = logger;
        }

        public async Task LoadDefaultsAsync()
        {
            if (!(await _roleManager.RoleExistsAsync(GoliathRoles.Administrator)))
            {
                _logger.LogInformation("Creating SuperUser account and Goliath roles.");

                #region Create admin role

                await _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = GoliathRoles.Administrator,
                    Icon = HttpUtility.HtmlEncode("<span class='badge badge-pill badge-danger ml-1'>ADMIN</span>"),
                    IsAdministrator = true
                });

                #endregion Create admin role

                #region Create default role

                await _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = GoliathRoles.Default,
                    Icon = string.Empty
                });

                #endregion Create default role

                #region Create super user account

                IdentityResult result = await _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = _config["SuperUser:Username"],
                    Email = _config["SuperUser:Email"],
                    EmailConfirmed = true,
                    BackgroundColor = "#FFFFFF",
                    DarkTheme = "false",
                    UserData = string.Empty,
                    PendingNotifications = string.Empty,
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

                #endregion Create super user account
            }
        }

        public async Task AddTestingDataAsync(int amount)
        {
            _logger.LogInformation($"Created testing data for Goliath. Created {amount} accounts.");
            Random rand = new();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_";

            for (int i = 0; i < amount; i++)
            {
                string userName = new(Enumerable.Repeat(chars, 12)
                  .Select(s => s[rand.Next(s.Length)]).ToArray());

                await _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = userName,
                    Email = $"{rand.Next(100000, 9999999)}@email.com",
                    EmailConfirmed = true,
                    BackgroundColor = "#FFFFFF",
                    DarkTheme = "false",
                    UserData = string.Empty,
                    PendingNotifications = string.Empty,
                    LogoutThreshold = 0,
                    AccountCreationDate = DateTime.UtcNow.ToString(),
                    LastPasswordUpdate = DateTime.UtcNow.ToString()
                },
        password: $"HelloWorld123!"
        );
                await _userManager.AddToRoleAsync(await GetUserByNameAsync(userName), GoliathRoles.Default);
            }
        }

        public async Task<bool> IsAdminAsync(ApplicationUser user)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            #region Goes through the users role and checks if any of them are admin.

            foreach (string role in roles)
            {
                if ((await _roleManager.FindByNameAsync(role)).IsAdministrator)
                {
                    return true;
                }
            }

            #endregion Goes through the users role and checks if any of them are admin.

            return false;
        }

        public async Task<bool> IsAdminAsync(string username)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            IList<string> roles = await _userManager.GetRolesAsync(user);

            #region Goes through the users role and checks if any of them are admin.

            foreach (string role in roles)
            {
                if ((await _roleManager.FindByNameAsync(role)).IsAdministrator)
                {
                    return true;
                }
            }

            #endregion Goes through the users role and checks if any of them are admin.

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

        public async Task MoveUserToAdminRoleAsync(ApplicationUser user)
        {
            if (!await _userManager.IsInRoleAsync(user, GoliathRoles.Administrator))
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRoleAsync(user, GoliathRoles.Administrator);
                IList<string> s = await _userManager.GetRolesAsync(user);
                await SendRoleMovedEmailAsync(user, s[0].ToString(), GoliathRoles.Administrator);
                _logger.LogInformation($"Moved {user.UserName} to role {GoliathRoles.Administrator}.");
            }
        }

        public async Task MoveUserToDefaultRoleAsync(ApplicationUser user)
        {
            if (!await _userManager.IsInRoleAsync(user, GoliathRoles.Default))
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRoleAsync(user, GoliathRoles.Default);
                IList<string> s = await _userManager.GetRolesAsync(user);
                await SendRoleMovedEmailAsync(user, s[0].ToString(), GoliathRoles.Administrator);
                _logger.LogInformation($"Moved {user.UserName} to role {GoliathRoles.Default}.");
            }
        }

        public async Task CreateRoleAsync(string name)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name
            });
            _logger.LogInformation($"Created new Goliath role {name}.");
        }

        public async Task CreateRoleAsync(string name, bool isAdmin)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name,
                IsAdministrator = isAdmin
            });
            _logger.LogInformation($"Created new Goliath role {name}.");
        }

        public async Task CreateRoleAsync(string name, bool isAdmin, string excludedURLComponents)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name,
                IsAdministrator = isAdmin,
                ExcludedURLComponents = excludedURLComponents
            });
            _logger.LogInformation($"Created new Goliath role {name}.");
        }

        public async Task CreateRoleAsync(string name, string icon, bool isAdmin)
        {
            await _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = name,
                IsAdministrator = isAdmin,
                Icon = icon
            });
            _logger.LogInformation($"Created new Goliath role {name}.");
        }

        public async Task DeleteRoleAsync(string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                await _roleManager.DeleteAsync(await _roleManager.FindByNameAsync(name));
            }
            _logger.LogInformation($"Deleted Goliath role {name}.");
        }

        public async Task<string> GetRoleIconAsync(string name)
        {
            if (await _roleManager.RoleExistsAsync(name))
            {
                return (await _roleManager.FindByNameAsync(name)).Icon;
            }
            return "Error";
        }

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

        public async Task MoveUserToRoleByNameAsync(ApplicationUser user, string name)
        {
            if (await _roleManager.RoleExistsAsync(name) && !await _userManager.IsInRoleAsync(user, name))
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRoleAsync(user, name);
                IList<string> s = await _userManager.GetRolesAsync(user);
                await SendRoleMovedEmailAsync(user, s[0].ToString(), GoliathRoles.Administrator);
                _logger.LogInformation($"Moved {user.UserName} to role {name}.");
            }
        }

        /////////////////////////////////////////////

        public async Task<ApplicationUser> GetUserByNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<ApplicationUser> FindUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetFromUserClaimAsync(ClaimsPrincipal claimsPrincipal)
        {
            return await _userManager.FindByNameAsync(claimsPrincipal.Identity.Name);
        }

        public async Task<ApplicationUser> GetUserFromContextAsync(ClaimsPrincipal claims)
        {
            return await _userManager.GetUserAsync(claims);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            _logger.LogInformation($"Attempted to update the database values for {user.Id} ({user.UserName}).");
            return await _userManager.UpdateAsync(user);
        }

        public async Task<bool> IsPasswordValidAsync(ApplicationUser user, string pass)
        {
            return await _userManager.CheckPasswordAsync(user, pass);
        }

        public async Task<IdentityResult> UpdatePasswordAsync(ApplicationUser user, string currPassword, string newPassword)
        {
            _logger.LogInformation($"Updated password for {user.Id}.");
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
            _logger.LogInformation($"Updated phone number for {user.Id}.");
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
            _logger.LogInformation($"Enabled two-factor authentication for user {user.Id}.");
        }

        public async Task SetTwoFactorDisabledAsync(ApplicationUser user)
        {
            user.TwoFactorMethod = (int)TwoFactorMethod.None;
            user.TwoFactorUpdated = DateTime.UtcNow.ToString();
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.UpdateAsync(user);
            _logger.LogInformation($"Disabled two-factor authentication for user {user.Id}.");
        }

        public async Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token, string remoteIp)
        {
            _logger.LogInformation($"Attempting to authorize user {user.Id} through two-factor.");
            _cookieManager.DeleteCookie(CookieKeys.TwoFactorAuthorizeCookie);
            SignInResult result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultPhoneProvider, token, false, false);
            if (result.Succeeded)
            {
                await AddNewLoginTracebackEntryAsync(user.UserName, remoteIp);
            }
            return result;
        }

        public async Task<SignInResult> AuthorizeUserTwoFactorAsync(ApplicationUser user, string token, string remoteIp, bool rememberMe)
        {
            _logger.LogInformation($"Attempting to authorize user {user.Id} through two-factor.");
            _cookieManager.DeleteCookie(CookieKeys.TwoFactorAuthorizeCookie);
            SignInResult result = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultPhoneProvider, token, rememberMe, false);
            if (result.Succeeded)
            {
                await AddNewLoginTracebackEntryAsync(user.UserName, remoteIp);
            }
            return result;
        }

        public async Task SendTwoFactorCodeSms(ApplicationUser user)
        {
            _logger.LogInformation($"Attempting to send SMS verification code to user {user.Id}.");
            string code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
            await _twilio.SendSmsAsync(new SMSTextModel()
            {
                Message = $"Your Two-Factor Code is: {code}.",
                To = user.PhoneNumber
            });
        }

        public async Task<List<string>> GenerateUserRecoveryCodesAsync(ApplicationUser user)
        {
            _logger.LogInformation($"Retrieved recovery codes for user {user.Id}.");
            return (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToList();
        }

        public async Task<int> GetUserRecoveryCodeAmountAsync(ApplicationUser user)
        {
            return await _userManager.CountRecoveryCodesAsync(user);
        }

        public async Task<IdentityResult> RedeemRecoveryCodeAsync(ApplicationUser user, string code)
        {
            _logger.LogInformation($"Redeemed recovery codes for user {user.Id}.");
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

        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel, DeviceParser device)
        {
            try
            {
                #region Create a user with some default values.

                // Create a new application user.
                ApplicationUser user = new()
                {
                    BackgroundColor = "#FFFFFF",
                    DarkTheme = "false",
                    UserData = string.Empty,
                    PendingNotifications = string.Empty,
                    UserName = userModel.Username,
                    Email = userModel.Email,
                    LogoutThreshold = 0,
                    AccountCreationDate = DateTime.UtcNow.ToString(),
                    LastPasswordUpdate = DateTime.UtcNow.ToString()
                };

                #endregion Create a user with some default values.

                // Use Identity Core to create the user.
                IdentityResult result = await (_userManager.CreateAsync(user, userModel.Password));
                if (result.Succeeded)
                {
                    // add them to default role.
                    await _userManager.AddToRoleAsync(user, GoliathRoles.Default);
                    // Send them a token.
                    await GenerateEmailConfirmationTokenAsync(userModel, user, device);
                }
                _logger.LogInformation($"Created user \"{user.Id}\" with username \"{user.UserName}\" and email \"{user.Email}\".");
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to create new user.");
                return IdentityResult.Failed(
                    new IdentityError()
                    {
                        Code = e.TargetSite.Name,
                        Description = "Could not create account."
                    });
            }
        }

        public async Task<string> UserToJsonAsync(ApplicationUser user)
        {
            UserDataModel model = new()
            {
                AccountCreationDate = user.AccountCreationDate,
                AccountLoginHistory = JsonConvert.DeserializeObject<List<LoginTracebackEntryModel>>(user.AccountLoginHistory),
                BackgroundColor = user.BackgroundColor,
                DarkTheme = user.DarkTheme,
                Email = user.Email,
                EmailConfirmed = $"{user.EmailConfirmed}",
                LastPasswordUpdate = user.LastPasswordUpdate,
                LastUserLogin = user.LastUserLogin,
                LogoutThreshold = $"{user.LogoutThreshold}",
                PendingNotifications = user.PendingNotifications,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = $"{user.PhoneNumberConfirmed}",
                TwoFactorMethod = $"{user.TwoFactorMethod}",
                TwoFactorUpdated = user.TwoFactorUpdated,
                UnverifiedNewEmail = user.UnverifiedNewEmail,
                UnverifiedNewPhone = user.UnverifiedNewPhone,
                UserData = user.UserData,
                UserId = user.Id,
                UserName = user.UserName,
                UserRole = await GetPrimaryRoleAsync(user)
            };
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }

        public async Task UpdateLastLoginAsync(ApplicationUser user)
        {
            user.LastUserLogin = DateTime.UtcNow.ToString();
            await UpdateUserAsync(user);
        }

        public async Task UpdateLastLoginAsync(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            user.LastUserLogin = DateTime.UtcNow.ToString();
            await UpdateUserAsync(user);
        }

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

        public async Task GenerateNewDataEncryptionEmailAsync(ApplicationUser userModel, DeviceParser device, string key, string link)
        {
            await SendDataEncryptionEmailAsync(userModel, device, key, link);
        }

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

        public async Task GenerateUsernameAsync(ApplicationUser user, DeviceParser device)
        {
            if (!(string.IsNullOrWhiteSpace(user?.UserName)))
            {
                await SendEmailWithUsernameAsync(user, device);
            }
        }

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

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SignInResult> PasswordSignInAsync(SignInModel signInModel, string remoteIp)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, signInModel.RememberMe, false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {signInModel.Username} - Logged in.");
                await AddNewLoginTracebackEntryAsync(signInModel.Username, remoteIp);
            }
            return result;
        }

        private async Task AddNewLoginTracebackEntryAsync(string userName, string ipAddress)
        {
            List<LoginTracebackEntryModel> loginHistory;
            ApplicationUser user = await _userManager.FindByNameAsync(userName);

            string tracebackHistory = user.AccountLoginHistory;

            LoginTracebackEntryModel tracebackEntryModel = new()
            {
                IPAddress = ipAddress,
                Timestamp = DateTime.UtcNow.ToString()
            };

            if (string.IsNullOrWhiteSpace(tracebackHistory))
            {
                loginHistory = new();
                loginHistory.Add(tracebackEntryModel);
            }
            else
            {
                loginHistory = JsonConvert.DeserializeObject<List<LoginTracebackEntryModel>>(tracebackHistory);
                loginHistory.Add(tracebackEntryModel);
                if (loginHistory.Count > 10)
                {
                    loginHistory.RemoveAt(0);
                }
            }

            tracebackHistory = JsonConvert.SerializeObject(loginHistory);
            user.AccountLoginHistory = tracebackHistory;
            await _userManager.UpdateAsync(user);
        }

        public async Task ClearLoginTracebackAsync(ApplicationUser user)
        {
            user.AccountLoginHistory = string.Empty;
            await _userManager.UpdateAsync(user);
        }

        public async Task DeleteUserAccountAsync(ApplicationUser user)
        {
            _logger.LogInformation($"User {user.Id} ({user.UserName}) has been deleted.");
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            foreach(UserLoginInfo login in await _userManager.GetLoginsAsync(user))
            {
                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }
            await _userManager.DeleteAsync(user);
        }

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

        private async Task SendDataEncryptionEmailAsync(ApplicationUser user, DeviceParser device, string key, string link)
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
            await _emailService.SendDataEncryptionEmailAsync(new()
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
                    {
                        "{{Link}}", link
                    },
                    {
                        "{{PrivateKey}}", key
                    }
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
                    _logger.LogInformation($"Confirmed email address for user {uid}.");
                    return IdentityResult.Success;
                }
                else
                {
                    _logger.LogInformation($"Failed to confirm email for user {uid}.");
                    // If the result failed then do not update any of the user data.
                    return IdentityResult.Failed();
                }
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to confirm email for user {uid} due to internal problem?");
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
                    _logger.LogInformation($"Confirmed phone number for user {user.Id}.");
                    return IdentityResult.Success;
                }
            }
            _logger.LogInformation($"Failed to confirm phone number for user {user.Id}.");
            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            // If the email cannot be confirmed then return a failed attempt.
            try
            {
                _logger.LogInformation($"Reset password for user {model.UserId}.");
                return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to reset password for user {model.UserId}.");
                return IdentityResult.Failed();
            }
        }
    }
}