using Common;
using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.Profile;
using Shared.Api;
using SharedModels.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Api.User
{
    [ApiVersion("1")]
    [Route("api/common/userprofile")]
    [ApiExplorerSettings(GroupName = RoleConsts.Admin)]
    [Authorize(Roles = RoleConsts.Admin)]
    public class UserProfileController : BaseController
    {
        private readonly IProfileService _profileService;

        public UserProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("profile-picture")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1024)]
        public async Task<IActionResult> UploadProfilePicture([FromForm] ProfilePictureUploadRequest request, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserIdInt();

            var result = await _profileService.UploadProfilePictureAsync(userId, request?.Image, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message ?? "Failed to upload profile picture.");
            }

            return Ok(new { url = result.Model });
        }
    }
}
