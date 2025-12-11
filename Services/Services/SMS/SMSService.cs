using Azure;
using Common;
using Common.Consts;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SMSService : ISMSService, IScopedDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public SMSService(IHttpClientFactory httpClientFactory )
        {
            _httpClientFactory = httpClientFactory;
          
        }

      

        public async Task<ResponseModel<string>> IncreseCharge(string Url, int Amount)
        {
       

            string url = $"https://sms.dariatech.com/api/v1/Payment/CreatePayment?SiteUrl={Url}&Amount={Amount}";

            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the URL
                    HttpResponseMessage response = await client.PostAsync(url, null);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        //var res =await _GlobalSettingRepository.TableNoTracking.FirstOrDefaultAsync();
                        //res.SMSCredit += Amount;
                        // _GlobalSettingRepository.Update(res);
                        return new ResponseModel<string>(true, responseContent);

                    }
                    else
                    {
                        return new ResponseModel<string>(false);
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel<string>(false, ex.Message);
                }
            }
    
        }

  
        public async Task<ResponseModel> SendSMSAsync(string UserToken, string Url, string to, string text)
        {
            Url = "https://localhost:7279";
            string url = $"https://sms.dariatech.com/api/v1/Payment/SendSMS?UserToken={UserToken}&CustomerUrl={Url}&Phonenumber={to}&Text={text}";

            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the URL
                    HttpResponseMessage response = await client.PostAsync(url, null);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                       
                        //var res = await _GlobalSettingRepository.TableNoTracking.FirstOrDefaultAsync();
                        //var count= SMSUtility.CalculateSMSCount(text);
                        //res.SMSCredit -= count*SiteConsts.SMSPricePerCharecter;
                       
                        //_GlobalSettingRepository.Update(res);
                        return new ResponseModel(true);

                    }
                    else
                    {
                        return new ResponseModel(false);
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel(false, ex.Message);
                }
            }
        }

        public async Task<ResponseModel> ValidatePayment(string Url, int Amount, int id)
        {

            string url = $"https://sms.dariatech.com/api/v1/Payment/Verify?id={id}&CustomerUrl={Url}&Amount={Amount}";
            // Create an HttpClient instance
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a GET request to the URL
                    HttpResponseMessage response = await client.PostAsync(url, null);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {

                        //var res = await _GlobalSettingRepository.TableNoTracking.FirstOrDefaultAsync();
                        //res.SMSCredit += Amount;
                        //_GlobalSettingRepository.Update(res);
                        return new ResponseModel(true);
                     
                    }
                    else
                    {
                        return new ResponseModel(false);
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseModel(false,ex.Message);
                }
            }

        }
    }
}