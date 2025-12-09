using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class AddressCalllModel : PageModel
    {
        public AddressCalllModel(ISettingService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public ISettingService Service { get; }
        public IMapper Mapper { get; }
        [BindProperty]
        public AddressCallSetting? Items { get; set; } = new();

        public void OnGet(int? id, CancellationToken cancellationToken)
        {

            var rs = Service.GetSetting();
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<AddressCallSetting>(rs.Model);
            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            Service.SetAddressCallSetting(Items);
            return RedirectToPage("./Index");
        }
    }
}
