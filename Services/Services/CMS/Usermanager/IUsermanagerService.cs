using Common;
using Entities;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS
{
    public interface IUsermanagerService
    {
        Task<ResponseModel> CreateAsync(UserDto user);
        Task<ResponseModel> UpdateAsync(int id, UserUpdateDto user);
        Task<ResponseModel> DeleteAsync(List<int> ids);
        Task<ResponseModel<List<UserSelectDto>>> GetListAsync(PageListModel model, CancellationToken cancellationToken);
        Task<ResponseModel<UserSelectDto>> GetByIdAsync(int id);

        Task<ResponseModel> ChangeUserStateAsync(int id, CancellationToken cancellationToken);

        Task<ResponseModel> ChangePassword(int id,string password, CancellationToken cancellationToken);
    }


}
