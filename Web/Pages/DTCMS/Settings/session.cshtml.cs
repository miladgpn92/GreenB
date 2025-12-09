using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.ActiveSession;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class SessionModel : PageModel
    {
        public SessionModel(IActiveSessionService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public IActiveSessionService Service { get; }
        public IMapper Mapper { get; }

        [BindProperty]
        public List<ActiveSessionDto>? Items { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserIdInt();
            var rs =await Service.GetSession(userId,HttpContext,cancellationToken);
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<List<ActiveSessionDto>>(rs.Model);
            }
        }

        public IActionResult OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


           // Service.SetEmailSetting(Items);
            return RedirectToPage("./Index");
        }
    }
}
