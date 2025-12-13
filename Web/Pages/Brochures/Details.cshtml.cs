using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Services.CMS.BrochureFiles;
using SharedModels.Dtos;

namespace Web.Pages.Brochures
{
    [AllowAnonymous]
    public class DetailsModel : PageModel
    {
        private readonly IBrochureFileService _brochureFileService;

        public DetailsModel(IBrochureFileService brochureFileService)
        {
            _brochureFileService = brochureFileService;
        }

        public BrochureFileDto? Brochure { get; set; }

        public async Task<IActionResult> OnGet(string slug, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            ResponseModel<BrochureFileDto> response;

            if (int.TryParse(slug, out var brochureId))
            {
                response = await _brochureFileService.GetByIdAsync(brochureId, cancellationToken);
            }
            else
            {
                response = await _brochureFileService.GetBySlugAsync(slug, cancellationToken);
            }

            if (!response.IsSuccess || response.Model == null)
                return NotFound();

            Brochure = response.Model;
            ViewData["Title"] = $"بروشور {Brochure.Title}";

            return Page();
        }
    }
}
