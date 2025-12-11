using Common;
using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.ActiveSession;
using Shared.Api;
using System.Data;
using System.Threading;

namespace Web.Api.ActiveSession
{
    [ApiVersion("1")]
    [Authorize]
    [Route("api/common/activesession")]
    [ApiExplorerSettings(GroupName = RoleConsts.Admin)]
    [NonController]
    public class ActiveSessionController : BaseController
    {
        private readonly IActiveSessionService _activeSessionService;

        public ActiveSessionController(IActiveSessionService activeSessionService)
        {
                _activeSessionService = activeSessionService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetList(CancellationToken cancellationToken)
        {
            var res =await _activeSessionService.GetSession(User.Identity.GetUserIdInt(), HttpContext, cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }

        [HttpDelete("[action]")]
        public ActionResult Delete(int SessionId)
        {
            var res =  _activeSessionService.RemoveActiveSession(SessionId);
            if (res.IsSuccess)
                return Ok(res.Description);
            else
                return BadRequest(res.Description);
        }
    }
}
