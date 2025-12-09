using Common;
using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.Profile;
using Shared.Api;
using SharedModels.Dtos;
using System.Data;

namespace Web.Api.Profile
{
    [ApiVersion("1")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var res = await _profileService.GetProfile(User.Identity.GetUserIdInt(), cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SetProfile(ProfileDto profile, CancellationToken cancellationToken)
        {
            profile.Id = User.Identity.GetUserIdInt();
            var res = await _profileService.UpdateProfile(profile, cancellationToken);  
            if (res.IsSuccess)
                return Ok(res.Description);
            else
                return BadRequest(res.Description);
        }

    }
}
