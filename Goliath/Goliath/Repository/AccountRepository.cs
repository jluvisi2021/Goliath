using Goliath.Helper;
using Goliath.Models;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// The object which can send direct emails to clients using HTML templates.
        /// </summary>
        private readonly IEmailService _emailService;

        /// <summary>
        /// The configuration for <b> appsettings.json </b>
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// The account repository is used for directly interacting with the ApplicationUser class.
        /// </summary>
        /// <param name="userManager"> </param>
        /// <param name="signInManager"> </param>
        /// <param name="emailService"> </param>
        /// <param name="config"> </param>
        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _config = config;
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
                    UserName = userModel.Username,
                    Email = userModel.Email,
                };
                // Use Identity Core to create the user.
                IdentityResult result = await (_userManager.CreateAsync(user, userModel.Password));
                if (result.Succeeded)
                {
                    // Send them a token.
                    await GenerateEmailConfirmationToken(userModel, user, device);
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
        /// Sends an email to a client with a generated token. <br /> This version of the method
        /// should only be used at registration due to its dependency of SignUpUserModel
        /// </summary>
        /// <param name="signUpModel"> </param>
        /// <param name="userModel"> </param>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public async Task GenerateEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await SendEmailConfirmationToken(signUpModel, userModel, device, token);
            }
        }

        /// <summary>
        /// Sends an email to the user with a token.
        /// </summary>
        /// <param name="user"> </param>
        /// <returns> </returns>
        public async Task GenerateEmailConfirmationToken(ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await ResendEmailConfirmationToken(userModel, device, token);
            }
        }

        /// <summary>
        /// Sends an email to a client with their username.
        /// </summary>
        /// <param name="model"> </param>
        /// <param name="device"> </param>
        /// <returns> </returns>
        public async Task GenerateUsername(ApplicationUser user, DeviceParser device)
        {
            if (!(string.IsNullOrWhiteSpace(user?.UserName)))
            {
                await SendEmailWithUsername(user, device);
            }
        }

        /// <summary>
        /// Sends an email to the user with a token to reset password.
        /// </summary>
        /// <param name="userModel"> </param>
        /// <param name="data"> </param>
        /// <returns> </returns>
        public async Task GenerateForgotPasswordToken(ApplicationUser userModel, DeviceParser device)
        {
            // Generate a token using Identity Core.
            string token = await _userManager.GeneratePasswordResetTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                await SendForgotPasswordToken(userModel, device, token);
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
            return await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, signInModel.RememberMe, false);
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
        private async Task ResendEmailConfirmationToken(ApplicationUser user, DeviceParser device, string token)
        {
            // Get the information required to send the email from the appsettings.json.
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];
            // Send an email while replacing all placeholders.
            await _emailService.ResendConfirmationEmail(new()
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
                        "{{DateTime}}", DateTime.Now.ToString()
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
        private async Task SendForgotPasswordToken(ApplicationUser user, DeviceParser device, string token)
        {
            // Get the information required to send the email from the appsettings.json.
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:ForgotPassword"];
            // Send an email while replacing all placeholders.
            await _emailService.SendForgotPasswordEmail(new()
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
                        "{{DateTime}}", DateTime.Now.ToString()
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
        private async Task SendEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser user, DeviceParser device, string token)
        {
            // Get values from appsettings.json
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];
            // Generate email with placeholders.
            await _emailService.SendConfirmationEmail(new()
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
                        "{{DateTime}}", DateTime.Now.ToString()
                    }
                        }
            });
        }

        private async Task SendEmailWithUsername(ApplicationUser user, DeviceParser device)
        {
            // Generate email with placeholders.
            await _emailService.SendForgotUsernameEmail(new()
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
                        "{{DateTime}}", DateTime.Now.ToString()
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
                return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
            }
            catch (Exception)
            {
                GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, "ConfirmEmailAsync: Failed to execute.");
                return IdentityResult.Failed();
            }
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