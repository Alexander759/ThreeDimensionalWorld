using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _mail;
        private readonly string _password;

        public EmailSender(string smtpServer, int port, string mail, string password)
        {
            _smtpServer = smtpServer;
            _port = port;
            _mail = mail;
            _password = password;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_smtpServer, _port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_mail, _password),
            };

            return client.SendMailAsync(new MailMessage(_mail, email, subject, htmlMessage) { IsBodyHtml = true });
        }
    }
}
