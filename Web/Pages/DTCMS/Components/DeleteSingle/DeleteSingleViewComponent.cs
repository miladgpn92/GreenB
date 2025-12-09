using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Pages.DTCMS.Components.PageAction;

namespace Web.Pages.DTCMS.Components.DeleteSingle
{
    public class DeleteSingleViewComponent : ViewComponent
    {

        public IViewComponentResult Invoke(string title, string id)
        {

            DeleteSingleModel model = new DeleteSingleModel()
            {
                Title = title,  
                Id = id 

            };

            return View("/Pages/DTCMS/Components/DeleteSingle/Index.cshtml", model);
        }

     
    }
    public class DeleteSingleModel
    {
        public string? Title { get; set; }
        public string? Id { get; set; }
    }
}
