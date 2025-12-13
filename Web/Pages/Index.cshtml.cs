using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DariaCMS.Common;
using Services.Services.CMS.Setting;
using Services.Services.CMS.BrochureFiles;
using SharedModels;
using SharedModels.Dtos;
using SharedModels.Dtos.Shared;

namespace Web.Pages;

[AllowAnonymous]
public class IndexModel : PageModel
{
    private readonly ISettingService _settingService;
    private readonly IBrochureFileService _brochureFileService;


    public IndexModel(ISettingService settingService, IBrochureFileService brochureFileService)
    {
        _settingService = settingService;
        _brochureFileService = brochureFileService;
    }

    [BindProperty]
    public SettingSelectDto Setting { get; set; } = new();

    public List<BrochureFileDto> Brochures { get; set; } = new();


    public async Task OnGet(CancellationToken cancellationToken)
    {
        var resSetting = _settingService.GetSetting();
        if (resSetting.IsSuccess)
        {
            Setting = resSetting.Model;
        }

        var brochureList = await _brochureFileService.GetListAsync(
            new PageListModel
            {
                arg = new Pageres
                {
                    PageNumber = 1,
                    PageSize = 20
                }
            },
            cancellationToken);

        if (brochureList.IsSuccess && brochureList.Model != null)
        {
            Brochures = brochureList.Model;
        }
    }
}
