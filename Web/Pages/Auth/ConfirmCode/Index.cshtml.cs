using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Auth.Register;
using Services.Services.CMS.Auth;
using SharedModels.Dtos;

namespace Web.Pages.Auth.ConfirmCode
{
  
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;

        private readonly SignInManager<ApplicationUser> _signInManager;
        public IndexModel(IAuthService authService, SignInManager<ApplicationUser> SignInManager)
        {
            _authService = authService;
            _signInManager = SignInManager;
        }

        [BindProperty]
        public AccountConfirmationDto model { get; set; } = new AccountConfirmationDto();

        [BindProperty]  
        public bool IsRegister { get; set; }

        public IActionResult OnGet(string Username , bool IsRegister)
        {

            if (string.IsNullOrEmpty(Username))
            {
                if (IsRegister)
                {
                    return RedirectToPage("/auth/register/index");
                }
                else
                {
                    return RedirectToPage("/auth/forgotPassword/index");
                }
            }
               
            model.UserName = Username;
            this.IsRegister = IsRegister;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {

                var res = await _authService.AccountVerification(model, cancellationToken);
                if (res.IsSuccess)
                {
                 await _signInManager.SignInAsync(res.Model, false);

                    return RedirectToPage("/auth/SetPasswordStep/index");


                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message);
                }
            }

            return Page();
        }
    }
}
