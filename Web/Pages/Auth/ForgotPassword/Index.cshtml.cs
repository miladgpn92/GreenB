using AspNetCore.ReCaptcha;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Services.Services.CMS.Auth;
using SharedModels.Dtos;

namespace Web.Pages.Auth.ForgotPassword;

[ValidateReCaptcha]
public class IndexModel : PageModel
{

    private readonly IAuthService _authService;

    public IndexModel(IAuthService authService) 
    {
        _authService = authService;
        model = new AuthDto();
    }

    [BindProperty]
    public AuthDto model { get; set; }
    public void OnGet()
    {

    }


    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            model.IsForgotpass= true;   
            var res = await _authService.SendCode(model, cancellationToken);
            if (res.IsSuccess)
            {
                return RedirectToPage(
                      "/auth/confirmcode/index",
                        new { username = model.UserName, IsRegister = false });
            }
            else
            {
                ModelState.AddModelError(string.Empty, res.Description);
            }
        }

        return Page();
    }
}
