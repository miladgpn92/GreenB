using AutoMapper;
using Common;
using Common.Enums;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using ResourceLibrary.Resources.Setting;
using ResourceLibrary.Resources.Usermanager;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS.Setting
{
    public class SettingService : IScopedDependency, ISettingService
    {

        private readonly IRepository<SiteSetting> _settingRepository;
        private readonly IStringLocalizer<SettingRes> _localizer;
        private readonly IMemoryCache _memoryCache;
        private readonly IMapper _mapper;

        // Remove ISettingCacheService from constructor
        public SettingService(
            IRepository<SiteSetting> settingRepository,
            IMapper mapper,
            IStringLocalizer<SettingRes> localizer,
            IMemoryCache memoryCache)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
            _localizer = localizer;
            _memoryCache = memoryCache;
        }

        public ResponseModel<SettingSelectDto> GetSetting()
        {

            const string SETTINGS_CACHE_KEY = "MainSettings";

            // Try to get from cache
            return _memoryCache.GetOrCreate(SETTINGS_CACHE_KEY, entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromHours(1))
                     .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                var lang = CmsEx.GetCurrentLanguage();
                var res = _settingRepository.TableNoTracking
                    .FirstOrDefault(a => a.CmsLanguage == lang);

                if (res == null)
                {
                    return new ResponseModel<SettingSelectDto>(
                        false,
                        null,
                        _localizer["SettingNotFound"].Value
                    );
                }

                var setting = _mapper.Map<SettingSelectDto>(res);
                return new ResponseModel<SettingSelectDto>(true, setting, "");
            });
        }

        public ResponseModel SetAddressCallSetting(AddressCallSetting addressCallSetting)
        {
            return UpdateSettingBase(setting =>
            {
                setting.Phonenumber = addressCallSetting.Phonenumber;
                setting.Tell = addressCallSetting.Tell;
                setting.Address = addressCallSetting.Address;
                setting.Latitude = addressCallSetting.Latitude;
                setting.Longitude = addressCallSetting.Longitude;
            });
        }

        public ResponseModel SetPublicSetting(PublicSetting publicSetting)
        {
            return UpdateSettingBase(setting =>
            {
                setting.SiteTitle = publicSetting.SiteTitle;
                setting.LogoUrl = publicSetting.LogoUrl;
                setting.FavIconUrl = publicSetting.FavIconUrl;
                setting.AboutUs = publicSetting.AboutUs;
                setting.WorkingHours = publicSetting.WorkingHours;  
                setting.IntroVideoUrl = publicSetting.IntroVideoUrl;

            });
        }

        public ResponseModel SetSEOSetting(string seoList)
        {
            return UpdateSettingBase(setting =>
            {
                setting.SeoList = seoList;
            });
        }

        // Centralized update method to reduce code duplication
        private ResponseModel UpdateSettingBase(Action<SiteSetting> updateAction)
        {
            const string SETTINGS_CACHE_KEY = "MainSettings";
            var lang = CmsEx.GetCurrentLanguage();

            var res = _settingRepository.TableNoTracking
                .FirstOrDefault(a => a.CmsLanguage == lang);

            if (res == null)
            {
                return new ResponseModel(false, _localizer["SettingNotFound"].Value);
            }

            updateAction(res);
            _settingRepository.Update(res);

            // Remove cache entry manually
            _memoryCache.Remove(SETTINGS_CACHE_KEY);

            return new ResponseModel(true, "");
        }
    }
}
