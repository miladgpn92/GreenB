using AutoMapper;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedModels.Dtos;
using SharedModels;
using Web.Infrastructure;
using Entities;
using Common.Utilities;
using DariaCMS.Common;
using Newtonsoft.Json;
using Services.Services.CMS;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Web.Pages.DTCMS.Usermanager
{

    [Authorize]
    public class IndexModel : PageModel
    {
        public IUsermanagerService Service { get; }
     

        public IndexModel(IUsermanagerService _Service , IMapper mapper)
        {

            Service = _Service;
            Mapper = mapper;
        }


        [BindProperty]
        public List<UserSelectDto> Items { get; set; } = new List<UserSelectDto>();

        [BindProperty]
        public PageListModel PageFilter { get; set; } = new();
        public IMapper Mapper { get; }

        public virtual async Task<IActionResult> OnGetAsync(PageListModel model, string filter, CancellationToken cancellationToken)
        {
            if (filter != null)
            {
                // sample: filter=[{"field":"title","value":"e","operator":"Contains"}]
                var FilterRaw = JsonConvert.DeserializeObject<List<FilterCriteria>>(filter);
                if (FilterRaw != null && FilterRaw.Count > 0)
                {
                    model.filters = FilterRaw;
                }
            }
            if (model.arg.PageSize > 100)
                model.arg.PageSize = 100;
            if (model.arg.PageSize == 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber == 0)
                model.arg.PageNumber = 1;
    

            PageFilter = model;

            var rs= await Service.GetListAsync(PageFilter, cancellationToken);

            if (rs.IsSuccess)
            {
                Items = rs.Model;
            }
            else
            {
               return BadRequest(rs.Message);
            }
           

            return Page();

        }

      
        public async Task<IActionResult> OnPostDeleteAsync(List<int> id, CancellationToken cancellationToken)
        {
            await Service.DeleteAsync(id);
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(int UserId,string Password, CancellationToken cancellationToken)
        {
            await Service.ChangePassword(UserId , Password,cancellationToken);
            return RedirectToPage("./Index");
        }

    }
}
