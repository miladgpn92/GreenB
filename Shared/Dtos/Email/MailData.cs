using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels
{
    public class MailData
    {
        // Receiver
        public List<string> To { get; }
  

        // Sender
        public string From { get; }
        public string DisplayName { get; }
    

        // Content
        public string Subject { get; }
        public string Body { get; }

        public bool HasHtmlText { get; set; }

        //This property is used to store the URL of an SMTP server used for sending emails.
        public string EmailSMTPUrl { get; set; }
        //This property is used to store the port number used for SMTP email communication.
        public int? EmailSMTPPort { get; set; }
        // This property is used to store the username associated with an email address.
        public string EmailUsername { get; set; }
        //This property is used to store the password associated with an email address. It should be kept private and should not be exposed to the public.
        public string EmailPassword { get; set; }
        // This property is used to indicate whether or not an email should be sent using SSL encryption. If set to true, the email will be sent using SSL encryption.
        public bool EmailSSL { get; set; }




        public MailData(List<string> to, string subject, string body = null, string from = null, string displayName = null,bool hasHtmlText=false, string emailSMTPUrl=null, int? emailSMTPPort=25, string emailUsername=null, string emailPassword=null, bool emailSSL=false)
        {
            // Receiver
            To = to;
          

            // Sender
            From = from;
            DisplayName = displayName;
          

            // Content
            Subject = subject;
            Body = body;
            HasHtmlText = hasHtmlText;

            //SMTP
            EmailSMTPUrl = emailSMTPUrl;
            EmailSMTPPort = emailSMTPPort;
            EmailUsername = emailUsername;
            EmailPassword = emailPassword;
            EmailSSL = emailSSL;    


        }
    }
}
