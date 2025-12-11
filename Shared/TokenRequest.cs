using Common.Enums;
using ResourceLibrary.Resources.ErrorMsg;
using System.ComponentModel.DataAnnotations;

namespace SharedModels
{

    /// <summary>
    /// Model for get data from user to generate token
    /// </summary>
    public class TokenRequest
    {

        [Display(Name = "UserName", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(150, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string username { get; set; }



        [Display(Name = "Password", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(150, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "UserRole", ResourceType = typeof(ResourceLibrary.Resources.Auth.AuthDto))]
        public UsersRole? UserRole { get; set; }

        public string refresh_token { get; set; }

    }
}
