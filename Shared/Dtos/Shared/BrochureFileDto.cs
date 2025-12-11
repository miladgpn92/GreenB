using AutoMapper;
using Entities;
using SharedModels.Api;
using SharedModels.CustomMapping;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.Dtos
{
    public class BrochureFileDto : SimpleBaseDto<BrochureFileDto, BrochureFile>
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(400, ErrorMessage = "حداکثر طول عنوان ۴۰۰ کاراکتر است")]
        public string Title { get; set; }

        [Required(ErrorMessage = "آدرس فایل PDF الزامی است")]
        [MaxLength(1500, ErrorMessage = "حداکثر طول آدرس فایل ۱۵۰۰ کاراکتر است")]
        public string PdfFileUrl { get; set; }

        [MaxLength(450, ErrorMessage = "حداکثر طول اسلاگ ۴۵۰ کاراکتر است")]
        public string Slug { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class BrochureFileCreateDto : IHaveCustomMapping
    {
        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(400, ErrorMessage = "حداکثر طول عنوان ۴۰۰ کاراکتر است")]
        public string Title { get; set; }

        [Required(ErrorMessage = "آدرس فایل PDF الزامی است")]
        [MaxLength(1500, ErrorMessage = "حداکثر طول آدرس فایل ۱۵۰۰ کاراکتر است")]
        public string PdfFileUrl { get; set; }

        [MaxLength(450, ErrorMessage = "حداکثر طول اسلاگ ۴۵۰ کاراکتر است")]
        public string Slug { get; set; }

        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<BrochureFileCreateDto, BrochureFile>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.Slug, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore());
        }
    }

    public class BrochureFileUpdateDto : IHaveCustomMapping
    {
        [Required(ErrorMessage = "شناسه الزامی است")]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الزامی است")]
        [MaxLength(400, ErrorMessage = "حداکثر طول عنوان ۴۰۰ کاراکتر است")]
        public string Title { get; set; }

        [Required(ErrorMessage = "آدرس فایل PDF الزامی است")]
        [MaxLength(1500, ErrorMessage = "حداکثر طول آدرس فایل ۱۵۰۰ کاراکتر است")]
        public string PdfFileUrl { get; set; }

        [MaxLength(450, ErrorMessage = "حداکثر طول اسلاگ ۴۵۰ کاراکتر است")]
        public string Slug { get; set; }

        public void CreateMappings(Profile profile)
        {
            profile.CreateMap<BrochureFileUpdateDto, BrochureFile>()
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.Slug, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}
