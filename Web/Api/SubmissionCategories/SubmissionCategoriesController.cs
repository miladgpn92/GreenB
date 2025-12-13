using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.SubmissionCategories;
using Shared.Api;
using SharedModels;
using SharedModels.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Api.SubmissionCategories
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = RoleConsts.Admin)]
    [Authorize(Roles = RoleConsts.Admin)]
    public class SubmissionCategoriesController : BaseController
    {
        private readonly ISubmissionCategoryService _submissionCategoryService;
        private const string NotFoundMessage = "دسته‌بندی یافت نشد";

        public SubmissionCategoriesController(ISubmissionCategoryService submissionCategoryService)
        {
            _submissionCategoryService = submissionCategoryService;
        }

        [HttpPost("PagedList")]
        public async Task<IActionResult> GetAll([FromBody] PageListModel model, CancellationToken cancellationToken)
        {
            var res = await _submissionCategoryService.GetListAsync(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var res = await _submissionCategoryService.GetByIdAsync(id, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubmissionCategoryCreateDto model, CancellationToken cancellationToken)
        {
            var res = await _submissionCategoryService.CreateAsync(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubmissionCategoryUpdateDto model, CancellationToken cancellationToken)
        {
            var res = await _submissionCategoryService.UpdateAsync(id, model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            if (res.Message == NotFoundMessage)
                return NotFound(res);

            return BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var res = await _submissionCategoryService.DeleteAsync(id, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            if (res.Message == NotFoundMessage)
                return NotFound(res);

            return BadRequest(res);
        }
    }
}
