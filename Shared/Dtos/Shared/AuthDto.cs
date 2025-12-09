using Common.Consts;
using Common.Enums;
using Microsoft.AspNetCore.Http;
using ResourceLibrary;
using ResourceLibrary.Resources.ErrorMsg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedModels.Dtos
{
    public class AuthDto
    {
        [Display(Name = "UserName", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(150, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string UserName { get; set; }

        public bool IsForgotpass { get; set; }

        /// <summary>
        /// Android device token for auto fill sms
        /// </summary>
        public string Token { get; set; }
      
    }

    public class ChangePassword
    {
        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = ErrMsg.RequierdMsg)]
        public string Password { get; set; }

        [Display(Name = "تکرار رمز عبور")]
        [Compare(nameof(Password), ErrorMessage = ErrMsg.PassWordNotMatch)]
        [Required(ErrorMessage = ErrMsg.RequierdMsg)]
        public string ConfirmPassword { get; set; }
    }

    public class AuthConfirmDto
    {

        [Display(Name = "موبایل ")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string UserName { get; set; }
        [Display(Name = "کد تایید")]
        public string VerificationCode { get; set; }
        public UsersRole usersRole { get; set; }
    }




    public class AccountConfirmationDto
    {
        //نام کاربری(موبایل)
        [Display(Name = "UserName", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(150, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string UserName { get; set; }


        [Display(Name = "VerificationCode", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(4, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MinLength(4, ErrorMessageResourceName = "MinLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string VerificationCode { get; set; }
  
    }



    /// <summary>
    /// انتخاب نام کاربری عمومی و پسورد
    /// </summary>
    public class AuthSelecPasswordDto
    {

        [Display(Name = "رمز عبور")]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string Password { get; set; }

        [Display(Name = " تکرار رمز عبور ")]
        [Compare("Password")]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string ConfirmPassword { get; set; }
    }




    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    public class AuthChangePasswordDto
    {
        //نام کاربری(موبایل) یا ایمیل را می پذیرد
        [Display(Name = "نام کاربری")]
        [MaxLength(150, ErrorMessage = ErrMsg.MaxLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string UserName { get; set; }

        [Display(Name = "کد تایید")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string VerificationCode { get; set; }

        [Display(Name = "رمز عبور")]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string Password { get; set; }

        [Display(Name = " تکرار رمز عبور ")]
        [Compare("Password")]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// ثبت اطلاعات برای بازیابی رمز عبور
    /// </summary>
    public class AuthForgotPasswordDto
    {
        [Display(Name = " موبایل -ایمیل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string UserName { get; set; }

        /// <summary>
        /// Android device token for auto fill sms
        /// </summary>
        public string Token { get; set; }

    }

    public class AuthSetProfileImage
    {

        [Display(Name = "تصویر کاربر")]
        public string UserImg { get; set; }

        [Display(Name = "تصویر کاربر")]
        public IFormFile UserImgUplod { get; set; }
    }
    /// <summary>
    /// تغییر رمز عبور
    /// </summary>
    public class AuthChangePasswordByUserDto
    {
        [Display(Name = "رمز عبور")]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string Password { get; set; }

        [Display(Name = "رمز عبور")]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string NewPassword { get; set; }

        [Display(Name = " تکرار رمز عبور ")]
        [Compare(nameof(NewPassword))]
        [MinLength(6, ErrorMessage = ErrMsg.MinLenMsg)]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        public string ConfirmNewPassword { get; set; }
    }

    /// <summary>
    /// Refresh Tocken
    /// </summary>
    public class RefreshTokenDto
    {
        public Guid RefreshToken { get; set; }
    }


    public class SetPassword
    {
        [Display(Name = "Password", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MinLength(6, ErrorMessageResourceName = "MinLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string Password { get; set; }     
    }
}
