using Common.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class GlobalSetting:SimpleBaseEntity
    {
        #region SMS
        /// <summary>
        /// Represents the amount of SMS credit available.
        /// </summary>
        /// 

        public double SMSCredit { get; set; }

        public bool IsSMSActive { get; set; }
        #endregion

        #region Email
        //This property is used to store the URL of an SMTP server used for sending emails.
        public string EmailSMTPUrl { get; set; }
        //This property is used to store the port number used for SMTP email communication.
        public int? EmailSMTPPort { get; set; }
        // This property is used to store the username associated with an email address.
        public string EmailUsername { get; set; }
        //This property is used to store the password associated with an email address. It should be kept private and should not be exposed to the public.
        public string EmailPassword { get; set; }
        // This property is used to indicate whether or not an email should be sent using SSL encryption. If set to true, the email will be sent using SSL encryption.
        public bool? EmailSSL { get; set; }
        #endregion


        #region Social
        public string EmailLink { get; set; }
        public string WhatsappLink { get; set; }
        public string TelegramLink { get; set; }
        public string InstagramLink { get; set; }
        public string AparatLink { get; set; }
        public string FacebookLink { get; set; }
        public string YoutubeLink { get; set; }
        public string LinkeinLink { get; set; }
        public string BaleLink { get; set; }
        public string EitaLink { get; set; }
        public string TwitterLink { get; set; }
        #endregion

        #region Inject Code
        public string InjectHeader { get; set; }

        public string InjectFooter { get; set; }
        #endregion


        #region Ai
        public string AIToken { get; set; }

        public string AIModel { get; set; }

        public string AIPreDescription { get; set; }
        #endregion

        #region Ai Page Generator
        public bool? AIPageGeneratorEnable { get; set; }

        public string AIMainKeyword { get; set; }

        public string AISecondaryKeyword { get; set; }

        public string AILocationKeyword { get; set; }

        #endregion
    }

    public class GlobalSettingconfiguration : IEntityTypeConfiguration<GlobalSetting>
    {
        public void Configure(EntityTypeBuilder<GlobalSetting> builder)
        {

            builder.HasData(new GlobalSetting
            {
                Id = 1,
                SMSCredit=0
            });

          



        }
    }
}
