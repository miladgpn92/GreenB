using Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PayPingService : IPayPingService, IScopedDependency
    {
        private readonly ProjectSettings _siteSetting;

        public PayPingService(IOptionsSnapshot<ProjectSettings> settings)
        {
            _siteSetting = settings.Value;
        }

        public async Task<ResponseModel> CreateRequest(int amount, string clientRefId = "", string payerIdentity = "", string payerName = "", string description = "")
        {
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            HttpClient httpClient = new HttpClient(clientHandler)
            {
                Timeout = TimeSpan.FromSeconds(20),
            };
            //Bmds1_7nhGovUMipuZXr3Ylv4jn06a0YO2Snuxg1yk4
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _siteSetting.PayPing.PayPingBearerToken);

            RequestPayClass ReqModel = new RequestPayClass
            {
                amount = amount,
                description = description,
                payerName = payerName,
                payerIdentity = payerIdentity,
                returnUrl = _siteSetting.PayPing.PayPingReturnUrl,
                clientRefId = clientRefId
            };

            var res = await httpClient.PostAsync("https://api.payping.ir/v2/pay", new StringContent(JsonConvert.SerializeObject(ReqModel), Encoding.UTF8, "application/json"));

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = await res.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<CodeClass>(responseContent);

                var address = "https://api.payping.ir/v2/pay/gotoipg/" + user.code;
                return new ResponseModel { IsSuccess = true, Description = address };
            }
            else
            {
                return new ResponseModel { IsSuccess = false, Description = "خطایی رخ داده است" };
            }
        }

        /// <summary>
        /// تایید کردن پرداخت
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> VerifyRequest(int amount, string refId)
        {
            //اگه  رف آیدی  زیر 100 بود یعنی مشکلی تو پردازش بوده 
            if (int.TryParse(refId, out _) && int.Parse(refId) < 50)
                return new ResponseModel { IsSuccess = false, Description = WrongTrantactionVerifyCodeToString(int.Parse(refId)) };

            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            HttpClient httpClient = new HttpClient(clientHandler)
            {
                Timeout = TimeSpan.FromSeconds(20),
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Bmds1_7nhGovUMipuZXr3Ylv4jn06a0YO2Snuxg1yk4");

            VerifyRequestModel ReqModel = new VerifyRequestModel
            {
                amount = amount,
                refId = refId
            };

            var res = await httpClient.PostAsync("https://api.payping.ir/v2/pay/verify", new StringContent(JsonConvert.SerializeObject(ReqModel), Encoding.UTF8, "application/json"));

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
                return new ResponseModel { IsSuccess = true, Description = "پرداخت با موفقیت انجام شد" };
            else
                return new ResponseModel { IsSuccess = false, Description = "خطایی رخ داده است" };
        }

        public string WrongTrantactionVerifyCodeToString(int refId)
        {
            return refId switch
            {
                1 => "تراكنش توسط شما لغو شد",
                2 => "رمز کارت اشتباه است",
                3 => "cvv2 یا تاریخ انقضای کارت وارد نشده است",
                4 => "موجودی کارت کافی نیست",
                5 => "تاریخ انقضای کارت گذشته است و یا اشتباه وارد شده",
                6 => "کارت شما مسدود شده است",
                7 => "تراکنش مورد نظر توسط درگاه یافت نشد",
                8 => "بانک صادر کننده کارت شما مجوز انجام تراکنش را صادر نکرده است",
                9 => "مبلغ تراکنش مشکل دارد",
                10 => "شماره کارت اشتباه است",
                11 => "ارتباط با درگاه برقرار نشد، مجددا تلاش کنید",
                12 => "خطای داخلی بانک رخ داده است",
                15 => "این تراکنش قبلا تایید شده است",
                18 => "کاربر پذیرنده تایید نشده است",
                19 => "هویت پذیرنده کامل نشده است و نمی تواند در مجموع بیشتر از ۵۰ هزار تومان دریافتی داشته باشد",
                25 => "سرویس موقتا از دسترس خارج است، لطفا بعدا مجددا تلاش نمایید",
                26 => "کد پرداخت پیدا نشد",
                27 => "پذیرنده مجاز به تراکنش با این مبلغ نمی باشد",
                28 => "لطفا از قطع بودن فیلتر شکن خود مطمئن شوید",
                29 => "ارتباط با درگاه برقرار نشد",
                31 => "امکان تایید پرداخت قبل از ورود به درگاه بانک وجود ندارد",
                38 => "آدرس سایت پذیرنده نا معتبر است",
                39 => "پرداخت ناموفق، مبلغ به حساب پرداخت کننده برگشت داده خواهد شد",
                44 => "RefId نامعتبر است",
                46 => "توکن ساخت پرداخت با توکن تایید پرداخت مغایرت دارد",
                47 => "مبلغ تراکنش مغایرت دارد",
                48 => "پرداخت از سمت شاپرک تایید نهایی نشده است",
                49 => "ترمینال فعال یافت نشد، لطفا مجددا تلاش کنید",
                _ => "خطایی رخ داده است",
            };
        }

        #region payment class
        class RequestPayClass
        {
            public int amount { get; set; }
            public string payerIdentity { get; set; }
            public string payerName { get; set; }
            public string description { get; set; }
            public string returnUrl { get; set; }
            public string clientRefId { get; set; }
        }

        class CodeClass
        {
            public string code { get; set; }
        }
        private class VerifyRequestModel
        {
            public string refId { get; set; }
            public int amount { get; set; }
        }

        #endregion
    }
}
