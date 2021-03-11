using Goliath.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Goliath.Services
{
    public class EmailService
    {
        private readonly SMTPConfigModel _smtpConfig;
        public EmailService(IOptions<SMTPConfigModel> options)
        {
            _smtpConfig = options.Value;
        }

        /// <summary>
        /// Send an email to user(s).
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private async Task SendEmail(UserEmailOptions options)
        {
            MimeMessage message = new()
            {
                Subject = options.Subject,
                Body = new TextPart("html")
                {
                    Text = options.Body
                }
            };
            message.From.Add(new MailboxAddress(_smtpConfig.DisplayName, _smtpConfig.Address));

            foreach(string toEmail in options.ToEmails)
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
                    
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("! --------------------------- !");
                    System.Diagnostics.Debug.WriteLine("Error sending email");
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    System.Diagnostics.Debug.WriteLine("! --------------------------- !");
                }

            }
        }

        /// <summary>
        /// Get the HTML template for sending emails.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetTemplate(string name)
        {
            return File.ReadAllText(@$"Services/EmailTemplate/{name}.html");
        }

    }
}
