using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.Content;
using Shared.Api;
using SharedModels.Dtos.Shared.SharedModels.Dtos.Shared;
using System.Data;

namespace Web.Api.Content
{
    [ApiVersion("1")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class ContentController : BaseController
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            this._contentService = contentService;
        }

        [HttpPost("update")]
        public IActionResult UpdateContent([FromBody] UpdateContentDto model)
        {
            if (model.Id <= 0 || string.IsNullOrEmpty(model.NewValue))
                return BadRequest(new { isSuccess = false, message = "Invalid input" });

            _contentService.UpdateValue(model.Id, model.NewValue);
            return Ok(new { isSuccess = true });
        }

        [HttpGet("all")]
        public IActionResult GetAllContent()
        {
            var pages = _contentService.GetAllPages();
            return Ok(new { isSuccess = true, data = pages });
        }

        [HttpPost("import")]
        public IActionResult ImportContent([FromBody] List<ContentPage> pages)
        {
            if (pages == null || !pages.Any())
                return BadRequest(new { isSuccess = false, message = "Invalid input" });

            _contentService.UpdateContent(pages);
            return Ok(new { isSuccess = true });
        }

        public class UpdateContentDto
        {
            public int Id { get; set; }
            public string NewValue { get; set; }
        }
    }
}
