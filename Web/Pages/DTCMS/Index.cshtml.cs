using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
 
using SharedModels.Dtos;

namespace Web.Pages.DTCMS
{
    [Authorize]
    public class IndexModel : PageModel
    {
 

        public IndexModel( )
        {
       
        }

 
        public async Task OnGetAsync(CancellationToken cancellationToken)
        {

      

        }
    }
}
