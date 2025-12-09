using Common;
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
    }
}
