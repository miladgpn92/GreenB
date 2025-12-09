using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Web.Pages.DTCMS.Components.SEOBottomSheet;

namespace Web.Pages.DTCMS.Components.Category
{
    public class CategoryViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ModelExpression? Id, ModelExpression? Title, string? ApiName)
        {
            var model = new VCCategoryModel();
            model.VCCategoryId = Id;
            model.VCCategoryTitle = Title;
            model.ApiName = ApiName;
            return View("/Pages/DTCMS/Components/Category/Index.cshtml", model);
        }
     
    }
    public class VCCategoryModel
    {
        public ModelExpression? VCCategoryId { get; set; }

        public ModelExpression? VCCategoryTitle { get; set; }

        public string? ApiName { get; set; }
    }
}
