using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.UserSubmissions;
using Shared.Api;
using SharedModels;
using SharedModels.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Api.UserSubmissions
{
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = RoleConsts.Admin)]
    [Authorize(Roles = RoleConsts.Admin, AuthenticationSchemes = "JwtScheme")]
    public class UserSubmissionsController : BaseController
    {
        private readonly IUserSubmissionService _userSubmissionService;
        private const string NotFoundMessage = "درخواست مورد نظر یافت نشد";

        public UserSubmissionsController(IUserSubmissionService userSubmissionService)
        {
            _userSubmissionService = userSubmissionService;
        }

        [HttpPost("PagedList")]
        public async Task<IActionResult> GetAll([FromBody] PageListModel model, [FromQuery] int? categoryId, [FromQuery] string phone, CancellationToken cancellationToken)
        {
            var res = await _userSubmissionService.GetListAsync(model, categoryId, phone, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var res = await _userSubmissionService.GetByIdAsync(id, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return NotFound(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserSubmissionCreateDto model, CancellationToken cancellationToken)
        {
            var res = await _userSubmissionService.CreateAsync(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            return BadRequest(res);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var res = await _userSubmissionService.DeleteAsync(id, cancellationToken);
            if (res.IsSuccess)
                return Ok(res);

            if (res.Message == NotFoundMessage)
                return NotFound(res);

            return BadRequest(res);
        }
    }
}
