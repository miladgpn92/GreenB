using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Common.Enums;
using System.IO;

namespace Entities
{
    public class SiteSetting : BaseEntity
    {
        #region Public
        public string SiteTitle { get; set; }

        public string AboutUs { get; set; }

        public string WorkingHours { get; set; }


        public string LogoUrl { get; set; }
        public string FavIconUrl { get; set; }
        #endregion

        public string SeoList { get; set; }

        #region Address&Call
        public string Phonenumber { get; set; }

        public string Tell { get; set; }

        public string Address { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }


        public string IntroVideoUrl { get; set; }



        #endregion

        // New property to store content data
        public string ContentData { get; set; }
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
            // مقدار ثابت برای SeoList
            string staticSeoList = GetRouteForSeo(); // مقدار واقعی را اینجا قرار دهید

            foreach (CmsLanguage lang in Enum.GetValues(typeof(CmsLanguage)))
            {
                builder.HasData(
                    new SiteSetting()
                    {
                        Id = id,
                        CmsLanguage = lang,
                        CreateDate = new DateTime(2025, 6, 1, 0, 0, 0), // مقدار ثابت
                        CreatorIP = "127.0.0.1",
                        SeoList = staticSeoList, // مقدار ثابت
                    }
                );
                id++;
            }
        }
    }



}
