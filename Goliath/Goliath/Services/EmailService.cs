using Goliath.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Goliath.Services
{
    /// <summary>
    /// The service for sending emails to clients through Goliath.
    /// <br />
    /// Uses the <strong>MailKit</strong> NuGet package for sending emails.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly SMTPConfigModel _smtpConfig;

        public EmailService(IOptions<SMTPConfigModel> options)
        {
            _smtpConfig = options.Value;
        }

        /// <summary>
        /// Send a test email.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<bool> SendTestEmail(UserEmailOptions options)
        {
            options.Subject = "Test Email";
            options.Body = GetTemplate("Default", options);

            return await SendEmail(options);
        }

        /// <summary>
        /// Send a confirmation link to a user.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>If the email was sent.</returns>
        public async Task<bool> SendConfirmationEmail(UserEmailOptions options)
        {
            options.Subject = "Confirm Your Account";
            options.Body = GetTemplate("ConfirmEmail", options);

            return await SendEmail(options);
        }

        /// <summary>
        /// Sends a resend verification email to a user.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<bool> ResendConfirmationEmail(UserEmailOptions options)
        {
            options.Subject = "Confirm Your Account";
            options.Body = GetTemplate("ResendConfirmEmail", options);

            return await SendEmail(options);
        }

        /// <summary>
        /// Send an email to user(s).
        /// </summary>
        /// <param name="options"></param>
        /// <returns>If the email was sent successfully.</returns>
        private async Task<bool> SendEmail(UserEmailOptions options)
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
                    await client.ConnectAsync(_smtpConfig.Host, _smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpConfig.Address, _smtpConfig.Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    client.Timeout = 20000;
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("! --------------------------- !");
                    System.Diagnostics.Debug.WriteLine("Error sending email");
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine("! --------------------------- !");
                    return false;
                }
            }
        }

        /// <summary>
        /// Get the HTML template for sending emails.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetTemplate(string name, UserEmailOptions options)
        {
            string template = File.ReadAllText(@$"Services/EmailTemplate/{name}.html");
            foreach (string s in options.Placeholders.Keys)
            {
                template = template.Replace(s, options.Placeholders[s]);
            }
            return template;
        }
    }
}