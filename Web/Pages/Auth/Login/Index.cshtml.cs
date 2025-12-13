using AspNetCore.ReCaptcha;
using Common;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Services.Services.CMS.Auth;
using SharedModels;

namespace Web.Pages.Auth.Login;

[ValidateReCaptcha]
public class IndexModel : PageModel
{
    private readonly ProjectSettings _projectsetting;
    private readonly IAuthService _authService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public IStringLocalizer<ResourceLibrary.Panel.Common.Auth.Login.Login> _Localizer { get; }
    public IndexModel(IOptionsSnapshot<ProjectSettings> settings, IAuthService authService, SignInManager<ApplicationUser> signInManager, IStringLocalizer<ResourceLibrary.Panel.Common.Auth.Login.Login> Localizer)
    {
        _projectsetting = settings.Value;
        _authService = authService;
        _signInManager = signInManager;
        _Localizer = Localizer;
        AuthDto = new TokenRequest();
    }

    public bool HasPhoneLogin { get; set; }

    public bool HasEmailLogin { get; set; }

    [BindProperty]
    public TokenRequest AuthDto { get; set; }


    public  IActionResult  OnGet()
    {
        HasEmailLogin = _projectsetting.ProjectSetting.IsEmailAuthEnable;
        HasPhoneLogin = _projectsetting.ProjectSetting.IsPhonenumberAuthEnable;

        if(User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/index");
        }
        return Page();

    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {

            var res = await _authService.Login(AuthDto, cancellationToken);
            if (res.IsSuccess)
            {
                var result = await _signInManager.PasswordSignInAsync(AuthDto.username, AuthDto.password, true, false);
                if (result.Succeeded)
                {
                    return RedirectToPage("/index");
                }

                ModelState.AddModelError(string.Empty, _Localizer["InvalidLogin"]);
            }
            else
            {
                ModelState.AddModelError(string.Empty, res.Message);
            }


        }

        return Page();
    }
}
