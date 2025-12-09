using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{

    [Authorize]
    public class SeoModel : PageModel
    {
        public SeoModel(ISettingService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public ISettingService Service { get; }
        public IMapper Mapper { get; }
        [BindProperty]
        public SEOSetting? SEOSetting { get; set; } = new();

        [BindProperty]
        public List<SEODto>? Items { get; set; } = new();

        public void OnGet(int? id, CancellationToken cancellationToken)
        {
           

            var rs = Service.GetSetting();
            if (rs.IsSuccess)
            {


                SEOSetting = Mapper.Map<SEOSetting>(rs.Model);

                Items = JsonConvert.DeserializeObject<List<SEODto>>(SEOSetting.SeoList);

            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            Service.SetSEOSetting(SEOSetting.SeoList);
            return RedirectToPage("./Index");
        }
    }
}
