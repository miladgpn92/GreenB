using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.BrochureFiles;
using Shared.Api;
using SharedModels;
using SharedModels.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Api.BrochureFiles
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = RoleConsts.Admin)]
    [Authorize(Roles = RoleConsts.Admin)]
    public class BrochureFilesController : BaseController
    {
        private readonly IBrochureFileService _brochureFileService;

        public BrochureFilesController(IBrochureFileService brochureFileService)
        {
            _brochureFileService = brochureFileService;
        }

        [HttpPost("PagedList")]
        public async Task<IActionResult> GetAll([FromBody] PageListModel model, CancellationToken cancellationToken)
        {
            var res = await _brochureFileService.GetListAsync(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var res = await _brochureFileService.GetByIdAsync(id, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return NotFound(res);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
        {
            var res = await _brochureFileService.GetBySlugAsync(slug, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrochureFileCreateDto model, CancellationToken cancellationToken)
        {
            var res = await _brochureFileService.CreateAsync(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BrochureFileUpdateDto model, CancellationToken cancellationToken)
        {
            var res = await _brochureFileService.UpdateAsync(id, model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            if (res.Message == "فایل بروشور یافت نشد")
                return NotFound(res);

            return BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var res = await _brochureFileService.DeleteAsync(id, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            if (res.Message == "فایل بروشور یافت نشد")
                return NotFound(res);

            return BadRequest(res);
        }
    }
}
