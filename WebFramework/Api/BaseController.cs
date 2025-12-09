using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFramework.Filters;

namespace Shared.Api
{
    [ApiController]
    //[AllowAnonymous]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]")]// api/v1/[controller]
    public class BaseController : ControllerBase
    {
        public bool UserIsAutheticated => HttpContext.User.Identity.IsAuthenticated;
    }
}
