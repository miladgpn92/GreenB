using AutoMapper;
using Common.Enums;
using Entities;
using ResourceLibrary.Resources.ErrorMsg;
using ResourceLibrary.Resources.Setting;
using SharedModels.CustomMapping;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dtos
{
    public class PublicSetting
    {
        [Display(Name = "SiteTitle", ResourceType = typeof(SettingRes))]
        [MaxLength(200, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string SiteTitle { get; set; }

        [Display(Name = "LogoUrl", ResourceType = typeof(SettingRes))]
        public string LogoUrl { get; set; }

        [Display(Name = "FavIconUrl", ResourceType = typeof(SettingRes))]
        public string FavIconUrl { get; set; }


        [Display(Name = "Phonenumber", ResourceType = typeof(SettingRes))]
        public string Phonenumber { get; set; }

        [Display(Name = "Tell", ResourceType = typeof(SettingRes))]
        public string Tell { get; set; }

        [Display(Name = "Address", ResourceType = typeof(SettingRes))]
        public string Address { get; set; }

        [Display(Name = " طول جغرافیایی")]
        public string Latitude { get; set; }

        [Display(Name = "عرض جغرافیایی")]
        public string Longitude { get; set; }


        [Display(Name = "تلگرام")]
        public string TelegramLink { get; set; }

        [Display(Name = "واتس اپ")]
        public string WhatsappLink { get; set; }

        [Display(Name = "اینستاگرام")]
        public string InstagramLink { get; set; }


        [Display(Name = "ایتا")]
        public string EaitaLink { get; set; }


        [Display(Name = "متن پیامک")]
        public string SMSText { get; set; }
    }








    public class AISetting
    {
        [Display(Name = "AIToken", ResourceType = typeof(SettingRes))]
        public string AIToken { get; set; }
        [Display(Name = "AIModel", ResourceType = typeof(SettingRes))]
        public string AIModel { get; set; }
        [Display(Name = "AIPreDescription", ResourceType = typeof(SettingRes))]
        public string AIPreDescription { get; set; }
    }

    public class AIPageGeneratorDto
    {
        [Display(Name = "????? ???? ??? ?? AI")]
        public bool? AIPageGeneratorEnable { get; set; }

        [Display(Name = "????? ????? ????")]
        public string AIMainKeyword { get; set; }
        [Display(Name = "????? ????? ????")]
        public string AISecondaryKeyword { get; set; }
        [Display(Name = "????? ????? ????")]
        public string AILocationKeyword { get; set; }
    }

    public class SettingSelectDto
    {
        public string SiteTitle { get; set; }
        public string LogoUrl { get; set; }

        public string FavIconUrl { get; set; }


        public string Phonenumber { get; set; }

        public string Tell { get; set; }

        public string Address { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }


        public string TelegramLink { get; set; }

        public string WhatsappLink { get; set; }

        public string InstagramLink { get; set; }


        public string EaitaLink { get; set; }


        public string SMSText { get; set; }

        public CmsLanguage CmsLanguage { get; set; }
    }

  

    public class SettingDtoMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<SettingSelectDto, SiteSetting>();
            profile.CreateMap<SiteSetting, SettingSelectDto>();

            profile.CreateMap<SiteSetting, PublicSetting>();
            profile.CreateMap<PublicSetting, SiteSetting>();

            profile.CreateMap<SettingSelectDto, PublicSetting>();
            profile.CreateMap<PublicSetting, SettingSelectDto>();



        }
    }
}
