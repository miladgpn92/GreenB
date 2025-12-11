using Common;
using Microsoft.AspNetCore.Http;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.Profile
{
    public interface IProfileService
    {
       Task<ResponseModel<ProfileDto>> GetProfile(int UserId, CancellationToken cancellationToken);

        Task<ResponseModel> UpdateProfile(ProfileDto profileDto, CancellationToken cancellationToken);

        Task<ResponseModel<string>> UploadProfilePictureAsync(int userId, IFormFile image, CancellationToken cancellationToken);
    }
}
