using Common.Consts;
using Microsoft.AspNetCore.Mvc;
using ResourceLibrary.Resources.ErrorMsg;
using ResourceLibrary.Resources.Usermanager;
using System.ComponentModel.DataAnnotations;
using Web.Pages.DTCMS.Components.DeleteSingle;

namespace Web.Pages.DTCMS.Components.ChangePassword
{
    public class ChangePasswordViewComponent:ViewComponent
    {
        public ChangePasswordViewComponent()
        {
            
        }
        public IViewComponentResult Invoke(int UserId)
        {


            ChangePasswordModel model = new ChangePasswordModel()
            {
              UserId = UserId

            };

            return View("/Pages/DTCMS/Components/ChangePassword/Index.cshtml", model);
        }

  
    }

    public class ChangePasswordModel
    {
        [Display(Name = "Password", ResourceType = typeof(UsermanagerRes))]
        [Required(ErrorMessage = ErrMsg.RequierdMsg)]
        [StringLength(100, MinimumLength = 6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(ErrorMsg))]
        [DataType(DataType.Password)]
        public  string? Password { get; set; }

        public int UserId { get; set; }
    }
}
