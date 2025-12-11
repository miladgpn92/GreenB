using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ResourceLibrary.Resources.SEO;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;
using System.ComponentModel.DataAnnotations;
 

namespace Web.Pages.Template.Components.CommonComponent.SeoGenerator
{
    public class SeoGeneratorViewComponent:ViewComponent
    {
      

        public IViewComponentResult Invoke(SettingSelectDto setting)
        {
            SEODto PageSeo = new SEODto();
            if (setting != null && !string.IsNullOrEmpty(setting.SeoList))
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                List<SEODto> SeoList = JsonConvert.DeserializeObject<List<SEODto>>(setting.SeoList);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                var RequestPath = $"{HttpContext.Request.Path}";

#pragma warning disable CS8604 // Possible null reference argument.
                SEODto? finedRes = SeoList.FirstOrDefault(a => a.Path.ToLower() == RequestPath.ToLower());
#pragma warning restore CS8604 // Possible null reference argument.
                if (finedRes != null)
                {
                    PageSeo = finedRes;
                }
                else
                {
                    PageSeo = ViewBag.Seo;
                }
                if (string.IsNullOrEmpty(PageSeo.SEOPic))
                {
                    PageSeo.SEOPic = setting.LogoUrl;
                }
                
                if(PageSeo.Date == null)
                {
                    PageSeo.Date = DateTime.Now;    
                }


            }

            VCSEOModel model = new VCSEOModel()
            {
                FavIcon=setting.FavIconUrl,
                Path=PageSeo.Path,
                SEODesc=TextUtility.TextLimit(PageSeo.SEODesc,170 ,true),
                SEOPic=PageSeo.SEOPic,
                SEOTitle=PageSeo.SEOTitle,
                SiteName = setting.SiteTitle,
                Date=PageSeo.Date
            };   

            return View("/Pages/Template/Components/CommonComponent/SeoGenerator/Index.cshtml", model);
        }
    }

    public class VCSEOModel
    {
        public string? Path { get; set; }
        public string? SEOTitle { get; set; }
        public string? SEODesc { get; set; }
        public string? SEOPic { get; set; }
        public string? FavIcon { get; set; }
        public string? SiteName { get; set; }

        public DateTime? Date { get; set; }
    }
}
