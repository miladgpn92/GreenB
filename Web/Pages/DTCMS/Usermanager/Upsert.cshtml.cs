using AutoMapper;
using Common.Enums;
using Common.Utilities;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services.Services.CMS;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Usermanager
{

    [Authorize]
    public class UpsertModel : PageModel
    {
        public UpsertModel(IUsermanagerService service, IMapper mapper)
        {
            Service = service;
            Mapper = mapper;

            // Initialize AvailableRoles here
            AvailableRoles = GetAvailableRoles();
        }

        [BindProperty]
        public UserDto? Items { get; set; } = new();

        [BindProperty]
        public bool IsEdit { get; set; }
        public IUsermanagerService Service { get; }
        public IMapper Mapper { get; }

        [BindProperty]
        public List<SelectListItem> AvailableRoles { get; set; }

        public async Task OnGetAsync(int? id, CancellationToken cancellationToken)
        {
            if (id.HasValue)
            {
                IsEdit = true;
                var rs = await Service.GetByIdAsync(id.Value);
                if (rs.IsSuccess)
                {
                    Items = Mapper.Map<UserDto>(rs.Model);
                    AvailableRoles = GetAvailableRoles();

                    // Check if AvailableRoles is not null before accessing its properties
                    var selectedRoleItem = AvailableRoles.Find(x => x.Value == rs.Model.UserRole.ToString());
                    if (selectedRoleItem != null)
                    {
                        selectedRoleItem.Selected = true;
                    }
                }
            }
            else
            {
                AvailableRoles = GetAvailableRoles();
                IsEdit = false;
            }
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                AvailableRoles = GetAvailableRoles();
                return Page();
            }

            if (IsEdit)
            {
                var updateModel = Mapper.Map<UserUpdateDto>(Items);
                if(Items != null)
                   await Service.UpdateAsync(Items.Id, updateModel);
            }
            else
            {
                await Service.CreateAsync(Items);
            }

            return RedirectToPage("./Index");
        }

        private List<SelectListItem> GetAvailableRoles()
        {
            return Enum.GetValues(typeof(UsersRole))
                .Cast<UsersRole>()
                .Select(r => new SelectListItem
                {
                    Value = ((int)r).ToString(),
                    Text = r.GetDisplayName() // Use a method to get the display name
                })
                .ToList();
        }
    }

}
