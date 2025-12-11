using Common;
using Entities;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.Auth
{
    public interface IAuthService
    {
        Task<ResponseModel> CreateRolesProject();

        Task<ResponseModel<ApplicationUser>> Login(TokenRequest tokenRequest, CancellationToken cancellationToken);

        Task<ResponseModel<ApplicationUser>> AccountVerification(AccountConfirmationDto model, CancellationToken cancellationToken);

        Task<ResponseModel> SendCode(AuthDto model, CancellationToken cancellationToken);

        Task<ResponseModel> Register(AuthDto model, CancellationToken cancellationToken);

         Task<ResponseModel<AccessToken>>  RefreshToken(RefreshTokenDto model, CancellationToken cancellationToken);

        Task<ResponseModel> SetPassword(SetPassword model,int UserId, CancellationToken cancellationToken);

        Task<ResponseModel> CompleteRegister(UserExtraData model, int UserId, CancellationToken cancellationToken);

    }
}
