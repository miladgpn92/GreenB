using AutoMapper;
using Entities;
using SharedModels.Api;
using SharedModels.CustomMapping;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dtos
{
    public class SubmissionCategoryDto : SimpleBaseDto<SubmissionCategoryDto, SubmissionCategory>
    {
        [Required(ErrorMessage = "عنوان الزامی است.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان ۲۰۰ کاراکتر است.")]
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class SubmissionCategoryCreateDto : IHaveCustomMapping
    {
        [Required(ErrorMessage = "عنوان الزامی است.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان ۲۰۰ کاراکتر است.")]
        public string Title { get; set; }

        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<SubmissionCategoryCreateDto, SubmissionCategory>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title.Trim()))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
        }
    }

    public class SubmissionCategoryUpdateDto : IHaveCustomMapping
    {
        [Required(ErrorMessage = "شناسه الزامی است.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الزامی است.")]
        [MaxLength(200, ErrorMessage = "حداکثر طول عنوان ۲۰۰ کاراکتر است.")]
        public string Title { get; set; }

        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<SubmissionCategoryUpdateDto, SubmissionCategory>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title.Trim()))
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}
