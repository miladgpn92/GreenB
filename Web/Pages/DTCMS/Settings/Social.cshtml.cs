using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class SocialModel : PageModel
    {
        public SocialModel(IGlobalSettingService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public IGlobalSettingService Service { get; }
        public IMapper Mapper { get; }

        [BindProperty]
        public SocialSetting? Items { get; set; } = new();

        public void OnGet(int? id, CancellationToken cancellationToken)
        {

            var rs = Service.GetGlobalSetting();
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<SocialSetting>(rs.Model);
            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            Service.SetSocialSetting(Items);
            return RedirectToPage("./Index");
        }
    }
}
