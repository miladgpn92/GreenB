using Microsoft.AspNetCore.Mvc;
using ResourceLibrary.Resources.CategoryMakerDto;
using ResourceLibrary.Resources.ErrorMsg;
using SharedModels.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages.DTCMS.Components.CategoryMaker
{
    public class CategoryMakerViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke(string PageTitle)
        {

           CategoryMakerDto categoryMakerDto = new CategoryMakerDto();
            categoryMakerDto.PageTitle = PageTitle;

            return View("/Pages/DTCMS/Components/CategoryMaker/Index.cshtml", categoryMakerDto);
        }

       
    }
    public class CategoryMakerDto
    {

        [Display(Name = "Title", ResourceType = typeof(CategoryMakerDtoRes))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        [MaxLength(150, ErrorMessageResourceName = "MaxLenMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public string? Title { get; set; }

        public string? PageTitle { get; set; }
    }
}
