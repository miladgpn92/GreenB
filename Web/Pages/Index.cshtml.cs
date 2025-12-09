using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ISettingService _settingService;

    public IndexModel(ISettingService settingService)
    {
        this._settingService = settingService;
    }

    [BindProperty]
    public SettingSelectDto Setting { get; set; } = new();

    public void OnGet()
    {
        var resSetting = _settingService.GetSetting();
        if (resSetting.IsSuccess)
        {
            Setting = resSetting.Model;
        }
    }
}
