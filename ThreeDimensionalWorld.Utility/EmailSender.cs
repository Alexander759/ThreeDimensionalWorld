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
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string mail = "aleks.asenov2005@gmail.com";
            string pass = "jvuu gxwf mcym pxwl";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pass),
            };


            return client.SendMailAsync(new MailMessage(mail, email, subject, htmlMessage) { IsBodyHtml = true });
        }
    }
}
