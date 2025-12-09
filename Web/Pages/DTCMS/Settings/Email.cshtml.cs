using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.GlobalSetting;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class EmailModel : PageModel
    {
        public EmailModel(IGlobalSettingService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public IGlobalSettingService Service { get; }
        public IMapper Mapper { get; }

        [BindProperty]
        public EmailSetting? Items { get; set; } = new();

        public void OnGet(int? id, CancellationToken cancellationToken)
        {

            var rs = Service.GetGlobalSetting();
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<EmailSetting>(rs.Model);
            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            Service.SetEmailSetting(Items);
            return RedirectToPage("./Index");
        }
    }
}
