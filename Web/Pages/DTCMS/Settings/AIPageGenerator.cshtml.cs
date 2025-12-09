using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class AIPageGeneratorModel : PageModel
    {
        private readonly IGlobalSettingService _settingService;
        public IMapper Mapper { get; }
        public AIPageGeneratorModel(IGlobalSettingService SettingService , IMapper mapper)
        {
            _settingService = SettingService;
            Mapper = mapper;
        }


        [BindProperty]
        public AIPageGeneratorDto? Items { get; set; } = new();

        public async Task OnGet(int? id, CancellationToken cancellationToken)
        {

       
            var rs = _settingService.GetGlobalSetting();
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<AIPageGeneratorDto>(rs.Model);
            }
        }

        public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
          
                return Page();
            }


            _settingService.SetAIPageGenerator(Items);
            return RedirectToPage("./Index");
        }

    }
}
