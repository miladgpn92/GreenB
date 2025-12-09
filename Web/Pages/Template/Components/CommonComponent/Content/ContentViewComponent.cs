using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.Content;
using SharedModels.Dtos.Shared;
using SharedModels.Dtos.Shared.SharedModels.Dtos.Shared;

namespace Web.Pages.Template.Components.CommonComponent.Content
{
    public class ContentViewComponent : ViewComponent
    {
        private readonly IContentService _contentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContentViewComponent(IContentService contentService, IHttpContextAccessor httpContextAccessor)
        {
            _contentService = contentService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke(int id, string? Class = null, string? Alt = null, string? Attr = null, string? Tag = null)
        {
            var contentItem = _contentService.GetById(id);

            bool isAdmin = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated
                           && _httpContextAccessor.HttpContext.User.IsInRole("admin");

            // Check if edit mode is disabled in cookie
            bool isEditDisabled = false;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("content_edit_disabled", out string? value))
            {
                isEditDisabled = value == "true";
            }

            var model = new ContentViewModel
            {
                ContentItem = contentItem,
                IsAdmin = isAdmin && !isEditDisabled,
                Class = Class,
                Attr = Attr,
                Alt = Alt,
                Tag = Tag,
                IsEditDisabled = isEditDisabled
            };

            return View("/Pages/Template/Components/CommonComponent/Content/Index.cshtml", model);
        }
    }

    public class ContentViewModel
    {
        public ContentItem ContentItem { get; set; }
        public bool IsAdmin { get; set; }
        public string? Class { get; set; }
        public string? Attr  { get; set; }
        public string? Alt { get; set; }
        public string? Tag { get; set; }
        public bool IsEditDisabled { get; set; }
    }
}
