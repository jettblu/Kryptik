using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CrypticPay.Services
{
    public class EmailSender: IEmailSender
    {

        private string apiKey;
        private string fromAddress;
        private string fromDisplayName;
        public EmailSender(string key, string fromEmail, string fromName)
        {
            apiKey = key;
            fromAddress = fromEmail;
            fromDisplayName = fromName;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromAddress, fromDisplayName),
                Subject = subject,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
        
            // uncomment below for gmail smtp

            /*// Our private configuration variables
            private string host;
            private int port;
            private bool enableSSL;
            private string userName;
            private string password;

            // Get our parameterized configuration
            public EmailSender(string host, int port, bool enableSSL, string userName, string password)
            {
                this.host = host;
                this.port = port;
                this.enableSSL = enableSSL;
                this.userName = userName;
                this.password = password;
            }

            // Use our configuration to send the email by using SmtpClient
            public Task SendEmailAsync(string email, string subject, string htmlMessage)
            {
                var client = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = enableSSL
                };
                return client.SendMailAsync(
                    new MailMessage(userName, email, subject, htmlMessage) { IsBodyHtml = true }
                );
                
                
            }*/
    }
}
