using Common;
using SharedModels;
using SharedModels.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.BrochureFiles
{
    public interface IBrochureFileService
    {
        Task<ResponseModel<List<BrochureFileDto>>> GetListAsync(PageListModel model, CancellationToken cancellationToken);

        Task<ResponseModel<BrochureFileDto>> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ResponseModel<BrochureFileDto>> GetBySlugAsync(string slug, CancellationToken cancellationToken);

        Task<ResponseModel<int>> CreateAsync(BrochureFileCreateDto model, CancellationToken cancellationToken);

        Task<ResponseModel<bool>> UpdateAsync(int id, BrochureFileUpdateDto model, CancellationToken cancellationToken);

        Task<ResponseModel<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
