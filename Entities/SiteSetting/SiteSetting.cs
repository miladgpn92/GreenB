using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.IO;

namespace Entities
{
    public class SiteSetting : BaseEntity
    {
        #region Public
        public string SiteTitle { get; set; }

        public string LogoUrl { get; set; }
        public string FavIconUrl { get; set; }
        #endregion


        #region Address&Call
        public string Phonenumber { get; set; }

        public string Tell { get; set; }

        public string Address { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string TelegramLink { get; set; }

        public string WhatsappLink { get; set; }

        public string InstagramLink { get; set; }


        public string EaitaLink { get; set; }




        #endregion

        public string SMSText { get; set; }

    
    }

    public class SiteSettingconfiguration : IEntityTypeConfiguration<SiteSetting>
    {
        private string GetRouteForSeo()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "routes.json");

            try
            {
                string content = File.ReadAllText(filePath);
                return content;
            }
            catch
            {

                return $"[]";
            }
        }
        public void Configure(EntityTypeBuilder<SiteSetting> builder)
        {
            int id = 1;
      

            foreach (CmsLanguage lang in Enum.GetValues(typeof(CmsLanguage)))
            {
                builder.HasData(
                    new SiteSetting()
                    {
                        Id = id,
                        CmsLanguage = lang,
                        CreateDate = new DateTime(2025, 6, 1, 0, 0, 0), 
                        CreatorIP = "127.0.0.1",
                      
                        SMSText = "کد پیگیری شما ثبت شد.",
                    }
                );
                id++;
            }
        }
    }



}
