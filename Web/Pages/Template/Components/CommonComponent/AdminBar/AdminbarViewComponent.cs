using Microsoft.AspNetCore.Mvc;

namespace Web.Pages.Template.Components.CommonComponent.AdminBar
{
    public class AdminbarViewComponent:ViewComponent
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminbarViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke(string route, bool? list , bool? add , bool? edit , bool? delete , List<AdminBarCustomModel>? custom , int? id)
        {
            // Only show to Admin users
            if (!User.IsInRole("admin"))
                return Content(string.Empty); // Render nothing if not admin



            // Check if edit mode is disabled in cookie
            bool isEditDisabled = false;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("content_edit_disabled", out string? value))
            {
                isEditDisabled = value == "true";
            }


            AdminbarVCModel model = new()
            {
                Add = add,
                Edit = edit,
                Delete = delete,
                Custom = custom,
                Route = route,
                List = list,
                Id = id,
                IsEditDisabled = isEditDisabled
            };


            return View("/Pages/Template/Components/CommonComponent/AdminBar/Index.cshtml", model);
        }
    }


    public class AdminbarVCModel
    {
        public string? Route { get; set; }
        public bool? List { get; set; } = true;
        public bool? Add { get; set; }  
        public bool? Edit { get; set; }
        public bool? Delete { get; set; }
        public List<AdminBarCustomModel>? Custom { get; set; } = new();
        public int? Id { get; set; }

        public bool IsEditDisabled { get; set; }

    }

    public class AdminBarCustomModel
    {
        public string? Icon { get; set; }

        public string? Link { get; set; }

        public string? Text { get; set; }
    }
}
