using SharedModels;
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailData mailData, CancellationToken ct);
    }
}
