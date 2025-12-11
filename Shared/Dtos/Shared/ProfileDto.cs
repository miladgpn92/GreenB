using Common.Consts;
using Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dtos
{
    public class ProfileDto
    {
        public int Id { get; set; }

        [Display(Name = "نام")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLenMsg)]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(AllowEmptyStrings = false, ErrorMessage = ErrMsg.RequierdMsg)]
        [MaxLength(100, ErrorMessage = ErrMsg.MaxLenMsg)]
        public string Family { get; set; }

        [Display(Name = "جنسیت")]
        public Gender? Gender { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public string UserPicUrl { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }
    }
}
