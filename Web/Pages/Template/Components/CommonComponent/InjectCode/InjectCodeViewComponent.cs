using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos;
using Web.Pages.DTCMS.Components.Category;

namespace Web.Pages.Template.Components.CommonComponent.InjectCode
{
    public class InjectCodeViewComponent:ViewComponent
    {
        private readonly IGlobalSettingService _service;

        public InjectCodeViewComponent(IGlobalSettingService service)
        {
            _service = service;
        }

        public IViewComponentResult Invoke(GetGlobalSettingDto globalSettingDto , bool isHeader)
        {

            VCInjectCodeModel model = new()
            {
                GlobalSettingDto = globalSettingDto,
                IsHeader = isHeader
            };
            return View("/Pages/Template/Components/CommonComponent/InjectCode/Index.cshtml", model);
        }
        

    
    }
    public class VCInjectCodeModel
    {
        public required GetGlobalSettingDto GlobalSettingDto { get; set; }

        public bool IsHeader { get; set; }
    }
}
