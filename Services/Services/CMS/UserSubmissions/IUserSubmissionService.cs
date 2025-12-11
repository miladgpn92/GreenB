using Common;
using SharedModels;
using SharedModels.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.UserSubmissions
{
    public interface IUserSubmissionService
    {
        Task<ResponseModel<List<UserSubmissionDto>>> GetListAsync(PageListModel model, int? categoryId, string phone, CancellationToken cancellationToken);

        Task<ResponseModel<UserSubmissionDto>> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ResponseModel<int>> CreateAsync(UserSubmissionCreateDto model, CancellationToken cancellationToken);

        Task<ResponseModel<bool>> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
