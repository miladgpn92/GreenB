using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SharedModels.Dtos
{
    public class ProfilePictureUploadRequest
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
