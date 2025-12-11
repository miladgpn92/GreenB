using AutoMapper;
using Common;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ResourceLibrary.Resources.Usermanager;
using Services.Services.CMS;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.Profile
{
    public class ProfileService : IScopedDependency, IProfileService
    {
        private const long MaxProfileImageSize = 2 * 1024 * 1024;

        private static readonly HashSet<string> AllowedProfileImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".webp"
        };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFilemanagerService _filemanagerService;
        private readonly ProjectSettings _projectSettings;
        private readonly IMapper mapper;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            IMapper Mapper,
            IStringLocalizer<UsermanagerRes> localizer,
            IFilemanagerService filemanagerService,
            IOptionsSnapshot<ProjectSettings> projectSettings)
        {
            _userManager = userManager;
            mapper = Mapper;
            _filemanagerService = filemanagerService;
            _projectSettings = projectSettings?.Value ?? new ProjectSettings();
            _ = localizer;
        }

        public async Task<ResponseModel<ProfileDto>> GetProfile(int UserId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(UserId.ToString());
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
            var user = await _userManager.FindByIdAsync(profileDto.Id.ToString());
            if (user == null)
                return new ResponseModel(false);
            else
            {
                user.Name = profileDto.Name;
                user.Family = profileDto.Family;
                user.PicUrl = profileDto.UserPicUrl;
                user.Gender = profileDto.Gender;
                await _userManager.UpdateAsync(user);
                return new ResponseModel(true);
            }
        }

        public async Task<ResponseModel<string>> UploadProfilePictureAsync(int userId, IFormFile image, CancellationToken cancellationToken)
        {
            if (image == null || image.Length == 0)
            {
                return new ResponseModel<string>(false, null, "Profile image is required.");
            }

            if (image.Length > MaxProfileImageSize)
            {
                return new ResponseModel<string>(false, null, "Maximum allowed size is 2 MB.");
            }

            if (!image.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return new ResponseModel<string>(false, null, "Only image files are allowed.");
            }

            var extension = Path.GetExtension(image.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedProfileImageExtensions.Contains(extension))
            {
                return new ResponseModel<string>(false, null, "Unsupported image format.");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new ResponseModel<string>(false, null, "User not found.");
            }

            var uploadRequest = new UploadMultiFile
            {
                Filepath = $"user/{userId}",
                Files = new List<IFormFile> { image }
            };

            var baseUrl = _projectSettings?.ProjectSetting?.BaseUrl ?? string.Empty;
            var uploadResult = _filemanagerService.UploadFiles(uploadRequest, baseUrl);

            if (!uploadResult.IsSuccess || uploadResult.Model == null || uploadResult.Model.Count == 0)
            {
                var message = uploadResult.Message;
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "Uploading profile image failed.";
                }

                return new ResponseModel<string>(false, null, message);
            }

            var newPictureUrl = uploadResult.Model.First();

            var previousUrl = user.PicUrl;
            user.PicUrl = newPictureUrl;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new ResponseModel<string>(false, null, "Saving profile image failed.");
            }

            RemoveProfilePicture(previousUrl);

            return new ResponseModel<string>(true, newPictureUrl);
        }

        private void RemoveProfilePicture(string currentUrl)
        {
            if (string.IsNullOrWhiteSpace(currentUrl))
            {
                return;
            }

            try
            {
                var baseUrl = _projectSettings?.ProjectSetting?.BaseUrl;

                string relativePath;
                if (!string.IsNullOrWhiteSpace(baseUrl) && currentUrl.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = currentUrl.Substring(baseUrl.Length);
                }
                else
                {
                    relativePath = currentUrl;
                }

                relativePath = relativePath.TrimStart('/');

                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    return;
                }

                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch
            {
                // ignore cleanup failures
            }
        }
    }
}
