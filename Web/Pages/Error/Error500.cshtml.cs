using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedModels.Dtos;

namespace Web.Pages.Error
{
    public class Error500Model : PageModel
    {
        public void OnGet()
        {
            SEODto PageSeo = new SEODto()
            {
                SEOTitle = "",
                SEODesc = "",
                SEOPic = "",

            };

            ViewData["Seo"] = PageSeo;
        }
    }
}
