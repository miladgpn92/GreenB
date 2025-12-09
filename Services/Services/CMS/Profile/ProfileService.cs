using AutoMapper;
using Common;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Resources.Usermanager;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.Profile
{
    public class ProfileService : IScopedDependency, IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;

        public ProfileService(UserManager<ApplicationUser> userManager, IMapper Mapper, IStringLocalizer<UsermanagerRes> localizer)
        {
            _userManager = userManager;
            mapper = Mapper;



        }

        public async Task<ResponseModel<ProfileDto>> GetProfile(int UserId, CancellationToken cancellationToken)
        {
            var user =await _userManager.FindByIdAsync(UserId.ToString());
            if (user == null)
            {
                return new ResponseModel<ProfileDto>(false);
            }
            else
            {
                ProfileDto profileDto = new ProfileDto()
                {
                    Family = user.Family,
                    Gender = user.Gender,
                    Id = user.Id,
                    Name = user.Name,
                    UserPicUrl = user.PicUrl    
                };   
                return new ResponseModel<ProfileDto>(true, profileDto);
            }
        }

        public async Task<ResponseModel> UpdateProfile(ProfileDto profileDto, CancellationToken cancellationToken)
        {
            var user =await _userManager.FindByIdAsync(profileDto.Id.ToString());
            if (user == null)
                return new ResponseModel(false);
            else
            {
                user.Name = profileDto.Name;
                user.Family= profileDto.Family;
                user.PicUrl = profileDto.UserPicUrl;
                user.Gender= profileDto.Gender; 
                await _userManager.UpdateAsync(user);
                return new ResponseModel(true);
            }

        }
    }
}
