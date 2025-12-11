using Azure.Core;
using Common;
using MimeKit;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Email
{
    public class EmailService : IEmailService, IScopedDependency
    {
        

        public Task SendEmailAsync(MailData mailData, CancellationToken ct)
        {
            try
            {
                MailMessage msg = new MailMessage
                {
                    Body = mailData.Body,
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress(mailData.From,mailData.DisplayName, Encoding.UTF8),
                    IsBodyHtml = mailData.HasHtmlText,
                    Priority = MailPriority.Normal
                };
                msg.Sender = msg.From;
                msg.Subject = mailData.Subject;
                msg.SubjectEncoding = Encoding.UTF8;

                // Receiver
                foreach (string mailAddress in mailData.To)
                    msg.To.Add(new MailAddress(mailAddress, mailAddress, Encoding.UTF8));

              
                SmtpClient smtp = new SmtpClient
                {
                    Host = mailData.EmailSMTPUrl,
                    Port = mailData.EmailSMTPPort.Value,
                    EnableSsl = mailData.EmailSSL,
                    Credentials = new NetworkCredential(mailData.EmailUsername, mailData.EmailPassword)
                };
                smtp.Send(msg);

            }
            catch 
            {

            }

            return Task.CompletedTask;
        }
    }
}
