using Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
 
using Services.Sitemap;
using System.Globalization;
using System.Net;

namespace Web.Controllers
{
    public class SitemapController : Controller
    {
 
        private readonly ProjectSettings _settings;
        private readonly SitemapBuilder _siteMapBuilder;

        public SitemapController(
       
            IOptions<ProjectSettings> settings)
        {
   
            _settings = settings.Value;
            _siteMapBuilder = new SitemapBuilder();
        }

        [Route("sitemap.xml")]
        [ResponseCache(Duration = 3600)] // Cache for 1 hour
        public async Task<IActionResult> SitemapAsync(CancellationToken cancellationToken)
        {
            var baseUrl = _settings.ProjectSetting.BaseUrl.TrimEnd('/');

            // Add static pages
            AddUrlWithDefaults(BuildUrl(baseUrl, ""));


          
           

            

            var memoryStream = new MemoryStream();
            _siteMapBuilder.WriteSitemap(memoryStream);

            // Convert stream to byte array before disposing
            byte[] bytes = memoryStream.ToArray();

            return File(bytes, "text/xml");
        }

        private async Task ProcessItemsAsync<T>(
          Func<T, string> urlBuilder,
          Task<ResponseModel<List<T>>> dataTask,
          string listUrl = null,
          CancellationToken cancellationToken = default)
        {
            var result = await dataTask;
            if (!result.IsSuccess) return;

            if (!string.IsNullOrEmpty(listUrl))
            {
                AddUrlWithDefaults(listUrl);
            }

            // Check if T has a CreateDate property
            var createDateProperty = typeof(T).GetProperty("CreateDate");

            foreach (var item in result.Model)
            {
                DateTime? createDate = null;
                DateTime? utcDate = null;
                // If CreateDate property exists, get its value
                if (createDateProperty != null && createDateProperty.PropertyType == typeof(DateTime))
                {
                    createDate = (DateTime?)createDateProperty.GetValue(item);
                    if(createDate != null )
                    {
                        // Convert Persian date to Gregorian date
                        DateTime gregorianDate = PersianDateConverter.ToGregorianDateTime(
                            createDate.Value.Year, createDate.Value.Month, createDate.Value.Day,
                            createDate.Value.Hour, createDate.Value.Minute, createDate.Value.Second
                        );
                        utcDate = gregorianDate.ToUniversalTime(); // Convert to UTC
                    }
              
                }

     



                AddUrlWithDefaults(urlBuilder(item), utcDate);
            }
        }

        private void AddUrlWithDefaults(string url, DateTime? modified = null)
        {
            // Convert Persian date to Gregorian date
            DateTime TodaygregorianDate = PersianDateConverter.ToGregorianDateTime(
            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second
            );
            DateTime TodayutcDate = TodaygregorianDate.ToUniversalTime(); // Convert to UTC


            _siteMapBuilder.AddUrl(
                url.ToLowerInvariant(),
                modified: modified ?? TodayutcDate,
                changeFrequency: modified == null ? ChangeFrequency.Weekly : null,
                priority: modified == null ? 1.0 : 0.9
            );
        }

        private string BuildUrl(string baseUrl, string path)
        {
            var fullUrl = new Uri(new Uri(baseUrl), path).ToString(); // Use ToString() instead of AbsoluteUri
            return fullUrl;
        }

        // Helper class for Persian-to-Gregorian date conversion
        public static class PersianDateConverter
        {
            private static readonly PersianCalendar _persianCalendar = new PersianCalendar();

            public static DateTime ToGregorianDateTime(int year, int month, int day, int hour, int minute, int second)
            {
                return _persianCalendar.ToDateTime(year, month, day, hour, minute, second, 0);
            }
        }
    }
}