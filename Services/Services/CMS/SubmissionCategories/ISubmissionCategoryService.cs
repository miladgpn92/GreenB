using Common;
using SharedModels;
using SharedModels.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.SubmissionCategories
{
    public interface ISubmissionCategoryService
    {
        Task<ResponseModel<List<SubmissionCategoryDto>>> GetListAsync(PageListModel model, CancellationToken cancellationToken);

        Task<ResponseModel<SubmissionCategoryDto>> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ResponseModel<int>> CreateAsync(SubmissionCategoryCreateDto model, CancellationToken cancellationToken);

        Task<ResponseModel<bool>> UpdateAsync(int id, SubmissionCategoryUpdateDto model, CancellationToken cancellationToken);

        Task<ResponseModel<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
