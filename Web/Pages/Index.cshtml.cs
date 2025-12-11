using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.Setting;
 
using SharedModels.Dtos;
using SharedModels.Dtos.Shared;

namespace Web.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ISettingService _settingService;
 

    public IndexModel(ISettingService settingService )
    {
        _settingService = settingService;
      
    }

    [BindProperty]
    public SettingSelectDto Setting { get; set; } = new();

 

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var resSetting = _settingService.GetSetting();
        if (resSetting.IsSuccess)
        {
            Setting = resSetting.Model;
        }

     
    }
}
