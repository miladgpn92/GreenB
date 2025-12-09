using Microsoft.AspNetCore.Mvc;

namespace Web.Pages.Template.Components.Pagination
{
    public class PaginationViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(VCPagination page)
        {

            VCPagination vCPagination = new VCPagination()
            {
                Page = page.Page,
                PageSize = page.PageSize,   
                Total = page.Total
            };

            return View("/Pages/Template/Components/Components/Pagination/Index.cshtml", vCPagination);
        }
    }

    public class VCPagination
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int Total { get; set; }
    }
}
