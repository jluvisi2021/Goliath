﻿using Goliath.Models;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
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
            ApplicationUser user = new()
            {
                UserName = userModel.Username,
                Email = userModel.Email,
            };
            var result = await (_userManager.CreateAsync(user, userModel.Password));
            if (result.Succeeded)
            {
                await GenerateEmailConfirmationToken(userModel, user, data);
            }
            return result;
        }

        /// <summary>
        /// Sends an email to the user with a token.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task GenerateEmailConfirmationToken(SignUpUserModel signUpModel, ApplicationUser userModel, string[] data)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
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
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userModel);
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
        /// as information about the computer sending the email.
        /// This method does not use the SignUp model. [USE FOR RESENDS]
        /// </summary>
        /// <param name="user"></param>
        /// <param name="computer"></param>
        /// <param name="ip"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ResendEmailConfirmationToken(ApplicationUser user, string computer, string ip, string token)
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
        /// <param name="signUpModel"></param>
        /// <param name="user"></param>
        /// <param name="computer"></param>
        /// <param name="ip"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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
            });
        }

        /// <summary>
        /// Confirms the email with the users ID and a token.
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