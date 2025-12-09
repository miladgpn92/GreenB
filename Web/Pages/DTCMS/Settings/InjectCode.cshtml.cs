using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class InjectCodeModel : PageModel
    {
        public InjectCodeModel(IGlobalSettingService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public IGlobalSettingService Service { get; }
        public IMapper Mapper { get; }

        [BindProperty]
        public InjectSetting? Items { get; set; } = new();

        public void OnGet(int? id, CancellationToken cancellationToken)
        {

            var rs = Service.GetGlobalSetting();
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<InjectSetting>(rs.Model);
            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            Service.SetInjectCodeSetting(Items);
            return RedirectToPage("./Index");
        }
    }
}
