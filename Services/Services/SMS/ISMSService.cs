using System;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Http;

namespace Services.Services
{
    public interface ISMSService
    {
        Task<ResponseModel> SendSMSAsync(string UserToken,string Url,string to, string text);

        Task<ResponseModel<string>> IncreseCharge(string Url, int Amount);

        Task<ResponseModel> ValidatePayment(string Url, int Amount , int id);
    }
}
