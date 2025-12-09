using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Ui;

namespace Web.Controllers
{
    [Authorize]
   
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
             _signInManager = signInManager;
        }

        [HttpPost]
        [Route("[controller]/Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            string homePage = "/auth/login";
     
            return new RedirectResult(homePage);
        }
 
    }
}
