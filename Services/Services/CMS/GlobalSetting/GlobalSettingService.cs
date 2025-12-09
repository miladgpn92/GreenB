using AutoMapper;
using Common;
using Common.Enums;
using Data.Repositories;
using Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Resources.Setting;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS.GlobalSetting
{
    public class GlobalSettingService : IScopedDependency, IGlobalSettingService
    {
        private readonly IRepository<Entities.GlobalSetting> _globalRepository;
        private readonly IStringLocalizer<SettingRes> _localizer;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        public GlobalSettingService(
            IRepository<Entities.GlobalSetting> globalRepository,
            IMapper mapper,
            IStringLocalizer<SettingRes> localizer,
            IMemoryCache memoryCache)
        {
            _globalRepository = globalRepository;
            _mapper = mapper;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }

        private const string GLOBAL_SETTINGS_CACHE_KEY = "GlobalSettings";

        public ResponseModel<GetGlobalSettingDto> GetGlobalSetting()
        {
            return _memoryCache.GetOrCreate(GLOBAL_SETTINGS_CACHE_KEY, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(1))
                     .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                var res = _globalRepository.TableNoTracking.FirstOrDefault();

                if (res == null)
                {
                    return new ResponseModel<GetGlobalSettingDto>(
                        false,
                        null,
                        _localizer["SettingNotFound"].Value
                    );
                }

                var setting = _mapper.Map<GetGlobalSettingDto>(res);
                return new ResponseModel<GetGlobalSettingDto>(true, setting, "");
            });
        }

        // Centralized update method to reduce code duplication and manage cache
        private ResponseModel UpdateSettingBase(Action<Entities.GlobalSetting> updateAction)
        {
            var res = _globalRepository.TableNoTracking.FirstOrDefault();

            if (res == null)
            {
                return new ResponseModel(false, _localizer["SettingNotFound"].Value);
            }

            updateAction(res);
            _globalRepository.Update(res);

            // Remove cache entry manually
            _memoryCache.Remove(GLOBAL_SETTINGS_CACHE_KEY);

            return new ResponseModel(true, "");
        }

        public ResponseModel SetSocialSetting(SocialSetting socialSetting)
        {
            return UpdateSettingBase(setting =>
            {
                setting.TelegramLink = socialSetting.TelegramLink;
                setting.InstagramLink = socialSetting.InstagramLink;
                setting.AparatLink = socialSetting.AparatLink;
                setting.FacebookLink = socialSetting.FacebookLink;
                setting.TwitterLink = socialSetting.TwitterLink;
                setting.YoutubeLink = socialSetting.YoutubeLink;
                setting.LinkeinLink = socialSetting.LinkeinLink;
                setting.BaleLink = socialSetting.BaleLink;
                setting.EitaLink = socialSetting.EitaLink;
                setting.EmailLink = socialSetting.EmailLink;
                setting.WhatsappLink = socialSetting.WhatsappLink;
            });
        }

        public ResponseModel SetInjectCodeSetting(InjectSetting injectSetting)
        {
            return UpdateSettingBase(setting =>
            {
                setting.InjectHeader = injectSetting.InjectHeader;
                setting.InjectFooter = injectSetting.InjectFooter;
            });
        }

        public ResponseModel SetEmailSetting(EmailSetting emailSetting)
        {
            return UpdateSettingBase(setting =>
            {
                setting.EmailSMTPUrl = emailSetting.EmailSMTPUrl;
                setting.EmailSMTPPort = emailSetting.EmailSMTPPort;
                setting.EmailUsername = emailSetting.EmailUsername;
                setting.EmailPassword = emailSetting.EmailPassword;
                setting.EmailSSL = emailSetting.EmailSSL;
            });
        }

        public ResponseModel<bool> ChangeSMSServiceState()
        {
            var res = _globalRepository.TableNoTracking.FirstOrDefault();

            if (res == null)
            {
                return new ResponseModel<bool>(false, false, _localizer["SettingNotFound"].Value);
            }

            res.IsSMSActive = !res.IsSMSActive;
            _globalRepository.Update(res);

            // Remove cache entry manually
            _memoryCache.Remove(GLOBAL_SETTINGS_CACHE_KEY);

            return new ResponseModel<bool>(true, res.IsSMSActive);
        }

        public ResponseModel SetAISetting(AISetting aISetting)
        {
            return UpdateSettingBase(setting =>
            {
                setting.AIModel = aISetting.AIModel;
                setting.AIPreDescription = aISetting.AIPreDescription;
                setting.AIToken = aISetting.AIToken;
 
            });
        }

        public ResponseModel SetAIPageGenerator(AIPageGeneratorDto model)
        {
            return UpdateSettingBase(setting =>
            {
                setting.AIPageGeneratorEnable = model.AIPageGeneratorEnable;
                setting.AIMainKeyword = model.AIMainKeyword;
                setting.AISecondaryKeyword= model.AISecondaryKeyword;
                setting.AILocationKeyword = model.AILocationKeyword;

            });
        }
    }
}
