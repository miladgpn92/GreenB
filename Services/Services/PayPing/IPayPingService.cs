using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    /// <summary>
    /// فقط جهت اتصال به درگاه پی پینگ هست و هیچگونه کاری برای بخش مالی انجام نمیشود
    /// </summary>
    public interface IPayPingService
    {
        /// <summary>
        /// اتصال به پی پینگ و گرفتن آدرس پرداخت
        /// </summary>
        /// <param name="amount">به تـومان</param>
        /// <param name="clientRefId"></param>
        /// <param name="payerIdentity">شماره موبایل یا ایمیل پرداخت کننده</param>
        /// <param name="payerName">نام پرداخت کننده</param>
        /// <param name="description"></param>
        /// <returns></returns>
        public Task<ResponseModel> CreateRequest(int amount, string clientRefId = "", string payerIdentity = "", string payerName = "", string description = "");

        /// <summary>
        /// تایید کردن پرداخت
        /// </summary>
        /// <param name="amount">به تـومان</param>
        /// <param name="refId">کدی که برای وریفای از سرور بهمون میدن و موقعی که از درگاه برمیگریم همراه رکوئست هست</param>
        /// <returns></returns>
        public Task<ResponseModel> VerifyRequest(int amount, string refId);

        /// <summary>
        /// اگه تو پرداخت مشکلی بود تبدیل کد خطا به متن 
        /// </summary>
        /// <param name="refId"></param>
        /// <returns></returns>
        public string WrongTrantactionVerifyCodeToString(int refId);
    }
}
