using Goliath.Helper;
using Goliath.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Services
{
    /// <inheritdoc cref="IEmailService" />
    public class EmailService : IEmailService
    {
        private readonly SMTPConfigModel _smtpConfig;
        private readonly ILogger _logger;

        // For email templates
        private static readonly string footerPath = @$"Services/EmailTemplate/Partial/Footer.html";

        private static readonly string stylesPath = @$"Services/EmailTemplate/Partial/Styles.html";

        public EmailService(IOptions<SMTPConfigModel> options, ILogger<EmailService> logger)
        {
            _smtpConfig = options.Value;
            _logger = logger;
        }

        public async Task<bool> SendTestEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Test Email";
            options.Body = await GetTemplateAsync("Default", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendRoleMovedEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Goliath Notification";
            options.Body = await GetTemplateAsync("RoleMovedEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendConfirmationEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Confirm Your Account";
            options.Body = await GetTemplateAsync("ConfirmEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendVerifyPhoneEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Confirm your Phone";
            options.Body = await GetTemplateAsync("ConfirmPhoneEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> ResendConfirmationEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Confirm Your Account";
            options.Body = await GetTemplateAsync("ResendConfirmEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendForgotPasswordEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Reset Password";
            options.Body = await GetTemplateAsync("ForgotPasswordEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendForgotUsernameEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Forgot Username";
            options.Body = await GetTemplateAsync("ForgotUsernameEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendConfirmNewEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Email Notice";
            options.Body = await GetTemplateAsync("ConfirmNewEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendNotifyOldEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Email Notice";
            options.Body = await GetTemplateAsync("NotifyOldEmail", options);

            return await SendEmailAsync(options);
        }

        public async Task<bool> SendDataEncryptionEmailAsync(UserEmailOptions options)
        {
            options.Subject = "Goliath Data";
            options.Body = await GetTemplateAsync("DataEncryptionEmail", options);

            return await SendEmailAsync(options);
        }

        /// <summary>
        /// Send an email to a user while using a template from UserEmailOptions. <br /> This method
        /// is not directly called to send an email rather it is passed through one of the public
        /// async methods of this class.
        /// </summary>
        /// <param name="options"> </param>
        /// <returns> If the email was sent successfully. </returns>
        private async Task<bool> SendEmailAsync(UserEmailOptions options)
        {
            // Setup the mail message.
            MimeMessage message = new()
            {
                Subject = options.Subject,
                Body = new TextPart("html")
                {
                    Text = options.Body
                }
            };
            message.From.Add(new MailboxAddress(_smtpConfig.DisplayName, _smtpConfig.Address));
            // Send the email message to all recipients.
            foreach (string toEmail in options.ToEmails)
            {
                message.To.Add(new MailboxAddress("User", toEmail));
            }

            using (SmtpClient client = new())
            {
                try
                {
                    // Async connect to the SMTP host and validate it exists.
                    await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    // Attempt to login to the SMTP using the settings from appsettings.json
                    await client.AuthenticateAsync(_smtpConfig.Address, _smtpConfig.Password);
                    // Send the message to the client.
                    await client.SendAsync(message);
                    // Disconnect from the session.
                    await client.DisconnectAsync(true);
                    client.Timeout = 20000;
                    _logger.LogInformation($"Sent an email to {options.ToEmails.Aggregate((a, b) => a + ", " + b)} at {DateTime.UtcNow} (UTC).");
                    return true;
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Failed to send an email to {options.ToEmails.Aggregate((a, b) => a + ", " + b)}.");
                    GoliathHelper.PrintDebugger(GoliathHelper.PrintType.Error, e.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the HTML template for sending emails. Templates are located in <b>
        /// ~/Services/EmailTemplate/ </b>
        /// </summary>
        /// <param name="name"> </param>
        /// <returns> </returns>
        private static async Task<string> GetTemplateAsync(string name, UserEmailOptions options)
        {
            // Read all of the raw text data from the file.
            string template = await File.ReadAllTextAsync(@$"Services/EmailTemplate/{name}.html");

            string footer = await File.ReadAllTextAsync(footerPath);

            // Go through each of the individual keys in the placeholder (values to replace).
            foreach (string s in options.Placeholders.Keys)
            {
                // Replace each key in the HTML with the placeholders value.
                template = template.Replace(s, options.Placeholders[s]);
                footer = footer.Replace(s, options.Placeholders[s]);
            }
            // Replace the partial footer.
            template = template.Replace("{{$footer}}", footer);
            // Replace with CSS styles.
            template = template.Replace("{{$styles}}", await File.ReadAllTextAsync(stylesPath));
            return template;
        }
    }
}