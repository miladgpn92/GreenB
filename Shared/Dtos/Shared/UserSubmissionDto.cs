using AutoMapper;
using Entities;
using SharedModels.Api;
using SharedModels.CustomMapping;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dtos
{
    public class UserSubmissionDto : SimpleBaseDto<UserSubmissionDto, UserSubmission>
    {
        [Required(ErrorMessage = "شماره تماس الزامی است.")]
        [MaxLength(50, ErrorMessage = "حداکثر طول شماره تماس ۵۰ کاراکتر است.")]
        public string Phone { get; set; }

        [MaxLength(150, ErrorMessage = "حداکثر طول نام ۱۵۰ کاراکتر است.")]
        public string FirstName { get; set; }

        [MaxLength(150, ErrorMessage = "حداکثر طول نام خانوادگی ۱۵۰ کاراکتر است.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "انتخاب دسته‌بندی الزامی است.")]
        public int SubmissionCategoryId { get; set; }

        public string SubmissionCategoryTitle { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public override void CustomMappings(IMappingExpression<UserSubmission, UserSubmissionDto> mapping)
        {
            mapping.ForMember(d => d.SubmissionCategoryTitle, opt => opt.MapFrom(s => s.SubmissionCategory.Title));
        }
    }

    public class UserSubmissionCreateDto : IHaveCustomMapping
    {
        [Required(ErrorMessage = "شماره تماس الزامی است.")]
        [MaxLength(50, ErrorMessage = "حداکثر طول شماره تماس ۵۰ کاراکتر است.")]
        public string Phone { get; set; }

        [MaxLength(150, ErrorMessage = "حداکثر طول نام ۱۵۰ کاراکتر است.")]
        public string FirstName { get; set; }

        [MaxLength(150, ErrorMessage = "حداکثر طول نام خانوادگی ۱۵۰ کاراکتر است.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "انتخاب دسته‌بندی الزامی است.")]
        public int SubmissionCategoryId { get; set; }

        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<UserSubmissionCreateDto, UserSubmission>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.Phone.Trim()))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.FirstName.Trim()))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.LastName.Trim()))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
        }
    }
}
