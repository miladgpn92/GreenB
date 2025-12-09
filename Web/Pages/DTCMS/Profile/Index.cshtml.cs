using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.Profile;
using Services.Services.CMS.Setting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Profile
{
    public class IndexModel : PageModel
    {
        public IndexModel(IProfileService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;
        }

        public IProfileService Service { get; }
        public IMapper Mapper { get; }
        [BindProperty]
        public ProfileDto? Items { get; set; } = new();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {

            var rs = await Service.GetProfile(User.Identity.GetUserIdInt(), cancellationToken);
            if (rs.IsSuccess)
            {
                Items = rs.Model;
            }
        }

        public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            Items.Id = User.Identity.GetUserIdInt();
            await Service.UpdateProfile(Items, cancellationToken);
            return RedirectToPage("./Index");
        }
    }
}
