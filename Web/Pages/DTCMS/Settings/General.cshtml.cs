using AutoMapper;
using Common.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Utilities;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    [Authorize]
    public class GeneralModel : PageModel
    {
        public GeneralModel(ISettingService service , IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public ISettingService Service { get; }
        public IMapper Mapper { get; }
        [BindProperty]
        public PublicSetting? Items { get; set; } = new();

        public void OnGet(int? id, CancellationToken cancellationToken)
        {
   
            var rs = Service.GetSetting();
            if(rs.IsSuccess) { 
                Items = Mapper.Map<PublicSetting>(rs.Model);
            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

      
            Service.SetPublicSetting(Items);
            return RedirectToPage("./Index");
        }
    }
}
