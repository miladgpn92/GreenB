using AspNetCore.ReCaptcha;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.Auth;
using SharedModels.Dtos;

namespace Web.Pages.Auth.SetPasswordStep
{
    [ValidateReCaptcha]
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;

        public IndexModel(IAuthService authService)
        {
            _authService = authService;
            model = new SetPassword();
        }

        [BindProperty]
        public SetPassword model { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {


                var user = User.Identity.GetUserIdInt();
                var res = await _authService.SetPassword(model, user, cancellationToken);
                if (res.IsSuccess)
                {

                    return RedirectToPage("/dtcms/index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Description);
                }
            }

            return Page();
        }

    }
}
