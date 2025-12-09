using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.GlobalSetting;
 
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages.Template.Components.Components.Header
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService;
    
        private readonly IGlobalSettingService _globalSettingService;

        public HeaderViewComponent(ISettingService settingService , IGlobalSettingService globalSettingService)
        {
          _settingService = settingService;
           _globalSettingService = globalSettingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            HeaderVCModel model = new HeaderVCModel();

            var resSetting= _settingService.GetSetting();
            if (resSetting.IsSuccess)
            {
                model.Setting = resSetting.Model;
            }

            var resGlobalSetting = _globalSettingService.GetGlobalSetting();
            if (resGlobalSetting.IsSuccess)
            {
                model.GlobalSetting = resGlobalSetting.Model;
            }

  



            return View("/Pages/Template/Components/Components/Header/Index.cshtml", model);
        }

    }

    public class HeaderVCModel
    {
        public SettingSelectDto Setting { get; set; } = new();

     

        public GetGlobalSettingDto GlobalSetting { get; set; } = new();

    }
}
