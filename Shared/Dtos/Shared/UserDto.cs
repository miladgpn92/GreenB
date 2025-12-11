using AutoMapper;

using Common.Consts;
using Common.Enums;

using Entities;
using ResourceLibrary.Resources.ErrorMsg;
using ResourceLibrary.Resources.Usermanager;
using SharedModels.Api;
using SharedModels.CustomMapping;
using SharedModels.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dtos
{
    public class UserDto
    {

        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(UsermanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Name { get; set; }

        [Display(Name = "Family", ResourceType = typeof(UsermanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Family { get; set; }

        [Display(Name = "PicUrl", ResourceType = typeof(UsermanagerRes))]
        public string PicUrl { get; set; }

        [Display(Name = "Email", ResourceType = typeof(UsermanagerRes))]
        [EmailAddress(ErrorMessageResourceName = "RegEx", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Email { get; set; }


        [Display(Name = "PhoneNumber", ResourceType = typeof(UsermanagerRes))]
        [ValidateIranPhonenumber(ErrorMessageResourceName = "MobileNumberErr", ErrorMessageResourceType = typeof(ErrorMsg))]
        [Required(ErrorMessage = ErrMsg.RequierdMsg)]
        public string PhoneNumber { get; set; }


        [Display(Name = "UserRole", ResourceType = typeof(UsermanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public UsersRole UserRole { get; set; }

        [Display(Name = "Password", ResourceType = typeof(UsermanagerRes))]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ErrorMsg))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(UsermanagerRes))]
        public Gender? Gender { get; set; }

    }


    public class UserUpdateDto
    {

        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(UsermanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Name { get; set; }

        [Display(Name = "Family", ResourceType = typeof(UsermanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(100, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Family { get; set; }

        [Display(Name = "PicUrl", ResourceType = typeof(UsermanagerRes))]
        public string PicUrl { get; set; }

        [Display(Name = "Email", ResourceType = typeof(UsermanagerRes))]
        [EmailAddress(ErrorMessageResourceName = "RegEx", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Email { get; set; }


        [Display(Name = "PhoneNumber", ResourceType = typeof(UsermanagerRes))]
        [ValidateIranPhonenumber(ErrorMessageResourceName = "MobileNumberErr", ErrorMessageResourceType = typeof(ErrorMsg))]
        [Required(ErrorMessage = ErrMsg.RequierdMsg)]
        public string PhoneNumber { get; set; }


        [Display(Name = "UserRole", ResourceType = typeof(UsermanagerRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public UsersRole UserRole { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(UsermanagerRes))]
        public Gender? Gender { get; set; }



    }

    public class UserSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string PicUrl { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public Gender Gender { get; set; }
        public UsersRole UserRole { get; set; }
        public string ValidationCode { get; set; }
        public DateTime LastLoginDate { get; set; }

        public DateTime CreateDate { get; set; }

    }

    public class UserDtoMapping : IHaveCustomMapping
    {
        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<UserSelectDto, ApplicationUser>();
            profile.CreateMap<ApplicationUser, UserSelectDto>();
            profile.CreateMap<UserDto, ApplicationUser>();
            profile.CreateMap<ApplicationUser, UserDto>();

            profile.CreateMap<UserSelectDto,UserDto>();

            profile.CreateMap<UserDto, UserUpdateDto>();
        }
    }
}
