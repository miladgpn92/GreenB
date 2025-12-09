using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ResourceLibrary.Resources.SharedModels;
using System.ComponentModel.DataAnnotations;
using Web.Pages.DTCMS.Components.PageAction;

namespace Web.Pages.DTCMS.Components.SEOBottomSheet
{
    public class SEOBottomSheetViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ModelExpression? Title , ModelExpression? Desc,string? PageRoute)
        {
            var model = new SEOBottomSheetViewModel();
            model.SEOTitle = Title;
            model.SEODesc = Desc;
            model.PageRoute= PageRoute;
            return View("/Pages/DTCMS/Components/SEOBottomSheet/Index.cshtml",model);
        }
    }


    public class SEOBottomSheetViewModel
    {
        public ModelExpression? SEOTitle { get; set; }
        public ModelExpression? SEODesc { get; set; }

        public string? PageRoute { get; set; }
    }
}
