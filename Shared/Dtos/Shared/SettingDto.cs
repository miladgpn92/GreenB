using AutoMapper;
using Common.Enums;
using Entities;
using ResourceLibrary.Resources.ErrorMsg;
using ResourceLibrary.Resources.Setting;
using SharedModels.CustomMapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Display(Name = "درباره ما")]
        public string AboutUs { get; set; }

        [Display(Name = "ساعت کاری")]
        public string WorkingHours { get; set; }


        [Display(Name = "ویدئو معرفی")]
        public string IntroVideoUrl { get; set; }

        [Display(Name = "کاتالوگ")]
        public string CatalogUrl { get; set; }

    }
    public class AddressCallSetting
    {
        [Display(Name = "Phonenumber", ResourceType = typeof(SettingRes))]
        public string Phonenumber { get; set; }

        [Display(Name = "Tell", ResourceType = typeof(SettingRes))]
        public string Tell { get; set; }

        [Display(Name = "Address", ResourceType = typeof(SettingRes))]
        public string Address { get; set; }

        [Display(Name = "  طول جغرافیایی")]
        public string Latitude { get; set; }

        [Display(Name = "  عرض جغرافیایی")]
        public string Longitude { get; set; }

        [Display(Name = "    آدرس دوم")]
        public string Address2 { get; set; }


        [Display(Name = "  طول جغرافیایی")]

        public string Latitude2 { get; set; }

        [Display(Name = "  عرض جغرافیایی")]
        public string Longitude2 { get; set; }



    }
    public class SEOSetting
    {
        [Display(Name = "SeoList", ResourceType = typeof(SettingRes))]
        public string SeoList { get; set; }
    }
    public class InjectSetting
    {
        [Display(Name = "InjectHeader", ResourceType = typeof(SettingRes))]
        public string InjectHeader { get; set; }

        [Display(Name = "InjectFooter", ResourceType = typeof(SettingRes))]
        public string InjectFooter { get; set; }
    }
    public class EmailSetting
    {
        [Display(Name = "EmailSMTPUrl", ResourceType = typeof(SettingRes))]
        public string EmailSMTPUrl { get; set; }

        [Display(Name = "EmailSMTPPort", ResourceType = typeof(SettingRes))]
        public int? EmailSMTPPort { get; set; }

        [Display(Name = "EmailUsername", ResourceType = typeof(SettingRes))]
        public string EmailUsername { get; set; }

        [Display(Name = "EmailPassword", ResourceType = typeof(SettingRes))]
        public string EmailPassword { get; set; }

        [Display(Name = "EmailSSL", ResourceType = typeof(SettingRes))]
        public bool EmailSSL { get; set; }
    }
    public class SocialSetting
    {
        [Display(Name = "TelegramLink", ResourceType = typeof(SettingRes))]
        public string TelegramLink { get; set; }

        [Display(Name = "InstagramLink", ResourceType = typeof(SettingRes))]
        public string InstagramLink { get; set; }

        [Display(Name = "AparatLink", ResourceType = typeof(SettingRes))]
        public string AparatLink { get; set; }

        [Display(Name = "FacebookLink", ResourceType = typeof(SettingRes))]
        public string FacebookLink { get; set; }

        [Display(Name = "YoutubeLink", ResourceType = typeof(SettingRes))]
        public string YoutubeLink { get; set; }

        [Display(Name = "LinkeinLink", ResourceType = typeof(SettingRes))]
        public string LinkeinLink { get; set; }

        [Display(Name = "BaleLink", ResourceType = typeof(SettingRes))]
        public string BaleLink { get; set; }

        [Display(Name = "EitaLink", ResourceType = typeof(SettingRes))]
        public string EitaLink { get; set; }

        [Display(Name = "TwitterLink", ResourceType = typeof(SettingRes))]
        public string TwitterLink { get; set; }

        [Display(Name = " آدرس ایمیل")]
        public string EmailLink { get; set; }

        [Display(Name = " واتس اپ")]
        public string WhatsappLink { get; set; }
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
        [Display(Name = "وضعیت صفحه ساز با AI")]
        public bool? AIPageGeneratorEnable { get; set; }

        [Display(Name = "کلمات کلیدی اصلی")]
        public string AIMainKeyword { get; set; }
        [Display(Name = "کلمات کلیدی فرعی")]
        public string AISecondaryKeyword { get; set; }
        [Display(Name = "کلمات کلیدی مکان")]
        public string AILocationKeyword { get; set; }
    }

    public class SettingSelectDto
    {
        public string SiteTitle { get; set; }
        public string LogoUrl { get; set; }
        public string FavIconUrl { get; set; }
        public string SeoList { get; set; }
        public string Phonenumber { get; set; }
        public string Tell { get; set; }
        public string Address { get; set; }

        public string AboutUs { get; set; }

        public string WorkingHours { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }


        public string Address2 { get; set; }

        public string Latitude2 { get; set; }

        public string Longitude2 { get; set; }



        public string IntroVideoUrl { get; set; }

        public string CatalogUrl { get; set; }

        public CmsLanguage CmsLanguage { get; set; }
    }

    public class GetGlobalSettingDto
    {

        public bool IsSMSActive { get; set; }
        public double SMSCredit { get; set; }
        public string EmailSMTPUrl { get; set; }
        public int? EmailSMTPPort { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
        public bool? EmailSSL { get; set; }

        public string TelegramLink { get; set; }
        public string InstagramLink { get; set; }
        public string AparatLink { get; set; }
        public string FacebookLink { get; set; }
        public string YoutubeLink { get; set; }
        public string LinkeinLink { get; set; }
        public string BaleLink { get; set; }
        public string EitaLink { get; set; }
        public string TwitterLink { get; set; }


        public string EmailLink { get; set; }
        public string WhatsappLink { get; set; }


        public string InjectHeader { get; set; }

        public string InjectFooter { get; set; }


        public string AIToken { get; set; }

        public string AIModel { get; set; }

        public string AIPreDescription { get; set; }



        public bool? AIPageGeneratorEnable { get; set; }

        public string AIMainKeyword { get; set; }

        public string AISecondaryKeyword { get; set; }

        public string AILocationKeyword { get; set; }


    }

    public class SettingDtoMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<SettingSelectDto, SiteSetting>();
            profile.CreateMap<SiteSetting, SettingSelectDto>();

            profile.CreateMap<Entities.GlobalSetting, GetGlobalSettingDto>();
            profile.CreateMap<GetGlobalSettingDto, Entities.GlobalSetting>();

            profile.CreateMap<SettingSelectDto,PublicSetting>();
            profile.CreateMap<SettingSelectDto, AddressCallSetting>();
            profile.CreateMap<SettingSelectDto, SEOSetting>();

            profile.CreateMap<GetGlobalSettingDto , EmailSetting>();
            profile.CreateMap<GetGlobalSettingDto, SocialSetting>();
            profile.CreateMap<GetGlobalSettingDto, InjectSetting>();
            profile.CreateMap<GetGlobalSettingDto, AISetting>();
            profile.CreateMap<GetGlobalSettingDto, AIPageGeneratorDto>();
        }
    }
}
