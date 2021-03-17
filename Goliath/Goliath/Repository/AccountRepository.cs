using Goliath.Models;
using Goliath.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        /// The configuration for <b>appsettings.json</b>
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// The account repository is used for directly interacting
        /// with the ApplicationUser class.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="emailService"></param>
        /// <param name="config"></param>
        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _config = config;
        }

        /// <summary>
        /// Creates the user and adds them to
        /// the database using Identity core.
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="data">Contains some data that should be presented in the email.</param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel, string[] data)
        {
            // Create a new application user.
            ApplicationUser user = new()
            {
                UserName = userModel.Username,
                Email = userModel.Email,
            };
            // Use Identity Core to create the user.
            var result = await (_userManager.CreateAsync(user, userModel.Password));
            if (result.Succeeded)
            {
                // Send them a token.
                await GenerateEmailConfirmationToken(userModel, user, data);
            }
            return result;
        }

        /// <summary>
        /// Sends an email to a client with a generated token.
        /// <br />
        /// This version of the method should only be used at registration due to its dependency of SignUpUserModel
        /// </summary>
        /// <param name="signUpModel"></param>
        /// <param name="userModel"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task GenerateEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser userModel, string[] data)
        {
            // Generate a token using Identity Core.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                // The data passed in represents the following information.
                // data[0] = Browser Info/Computer Info (User Agent)
                // data[1] = IP Address (IPv4 Mapped)
                await SendEmailConfirmationToken(signUpModel, userModel, data[0], data[1], token);
            }
        }

        /// <summary>
        /// Sends an email to the user with a token.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task GenerateEmailConfirmationToken(ApplicationUser userModel, string[] data)
        {
            // Generate a token using Identity Core.
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
            // If the token is valid.
            if (!string.IsNullOrWhiteSpace(token))
            {
                // The data passed in represents the following information.
                // data[0] = Browser Info/Computer Info (User Agent)
                // data[1] = IP Address (IPv4 Mapped)
                await ResendEmailConfirmationToken(userModel, data[0], data[1], token);
            }
        }

        /// <summary>
        /// Returns the application user which is found from
        /// a specified email address.
        /// </summary>
        /// <param name="email">Email Address</param>
        /// <returns>Application User (async)</returns>
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Sign in to the application
        /// using the sign in model.
        /// </summary>
        /// <param name="signInModel"></param>
        /// <returns></returns>
        public async Task<SignInResult> PasswordSignInAsync(SignInModel signInModel)
        {
            return await _signInManager.PasswordSignInAsync(signInModel.Username, signInModel.Password, signInModel.RememberMe, false);
        }

        /// <summary>
        /// Manage signing out of the application.
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Converts a string to asterisk characters.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string ConvertToAstrisk(string str)
        {
            string j = string.Empty;
            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                j += "*";
            }
            return j;
        }

        /// <summary>
        /// Send an email to a user with a confirmation token as well
        /// as information about the computer sending the email.<br />
        /// This method does not use the SignUp model. [USE FOR RESENDS]
        /// </summary>
        /// <param name="user"></param>
        /// <param name="computer">User Agent</param>
        /// <param name="ip">IPv4 Address</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ResendEmailConfirmationToken(ApplicationUser user, string computer, string ip, string token)
        {
            // Get the information required to send the email from the appsettings.json.
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];
            // Send an email while replacing all placeholders.
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
                        "{{IPAddress}}", ip
                    },
                    {
                        "{{ComputerInfo}}", computer
                    },
                    {
                        "{{VerifyLink}}", string.Format(appDomain + verifyLink, user.Id, token)
                    },
                    {
                        "{{DateTime}}", DateTime.Now.ToString()
                    }
                        }.ToImmutableDictionary()
            });
        }

        /// <summary>
        /// Send an email to a user with a confirmation token as well
        /// as information about the computer sending the email.
        /// </summary>
        /// <param name="signUpModel">Register Model.</param>
        /// <param name="user">Current client.</param>
        /// <param name="computer">UserAgent info.</param>
        /// <param name="ip">Mapped IPv4 address.</param>
        /// <param name="token">Generated .NET Core token.</param>
        /// <returns></returns>
        private async Task SendEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser user, string computer, string ip, string token)
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
                        "{{IPAddress}}", ip
                    },
                    {
                        "{{ComputerInfo}}", computer
                    },
                    {
                        "{{VerifyLink}}", string.Format(appDomain + verifyLink, user.Id, token)
                    },
                    {
                        "{{DateTime}}", DateTime.Now.ToString()
                    }
                        }.ToImmutableDictionary()
            });
        }

        /// <summary>
        /// Confirms that a clients email and token match.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
        }
    }
}