using Goliath.Models;
using Goliath.Services;
using Microsoft.AspNetCore.Http;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

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
            ApplicationUser user = new()
            {
                UserName = userModel.Username,
                Email = userModel.Email,
            };
            var result = await (_userManager.CreateAsync(user, userModel.Password));
            if(result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if(!string.IsNullOrWhiteSpace(token))
                {
                    // data[0] = Browser Info
                    // data[1] = Computer Info
                    // data[2] = IP Address
                    await SendEmailConfirmationToken(userModel, user, data[0], data[1], token);
                }
            }
            return result;
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
            for(int i = 0; i < l; i++)
            {
                j += "*";
            }
            return j;
        }

        private async Task SendEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser user, string computer, string ip, string token)
        {
            string appDomain = _config["Application:AppDomain"];
            string verifyLink = _config["Application:EmailConfirmation"];

           
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
            }) ;

        }
    }
}