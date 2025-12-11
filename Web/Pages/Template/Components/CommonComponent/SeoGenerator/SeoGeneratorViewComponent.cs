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
