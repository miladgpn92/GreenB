using Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Web.Pages.DTCMS.Components.PageAction
{
    public class PageActionViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PageActionViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke(string pageName,bool hasSearch = true, bool hasFilter=false, bool hasAdd=true , string SearchFilterQuery = "title")
        {
            
            PageActionModel model = new PageActionModel()
            {
                hasAdd = hasAdd,
                hasSearch = hasSearch,
                hasFilter = hasFilter,
                pageName = pageName,
                SearchFilterQuery = SearchFilterQuery,
               
            };
            var query = _httpContextAccessor.HttpContext?.Request.Query;
            if(query != null)
            {
                var SearchQuery = query["Search"];
                if(SearchQuery.ToString() != null)
                {
                    model.Search = SearchQuery.ToString();
                }
               
            }
          

          
            return View("/Pages/DTCMS/Components/PageAction/Index.cshtml",model);
        }

    }

    public class PageActionModel
    {
        public bool hasSearch { get; set; }

        public bool hasFilter { get; set; }
        public bool hasAdd { get; set; }


        public string? pageName { get; set; }

        public string? SearchFilterQuery {  get; set; }

        public string? Search { get; set; }
    }
}
