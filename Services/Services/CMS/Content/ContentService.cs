using Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using SharedModels.Dtos.Shared;
using SharedModels.Dtos.Shared.SharedModels.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services.CMS.Content
{
    public class ContentService : IScopedDependency, IContentService
    {
        private readonly IRepository<SiteSetting> _siteSettingRepository;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "ContentData";

        public ContentService(IRepository<SiteSetting> siteSettingRepository, IMemoryCache memoryCache)
        {
            _siteSettingRepository = siteSettingRepository;
            _memoryCache = memoryCache;
        }

        public List<ContentPage> GetAllPages()
        {
            if (!_memoryCache.TryGetValue(CacheKey, out List<ContentPage> pages))
            {
                var siteSetting = _siteSettingRepository.TableNoTracking.FirstOrDefault();
                if (siteSetting == null || string.IsNullOrEmpty(siteSetting.ContentData))
                {
                    pages = new List<ContentPage>();
                }
                else
                {
                    pages = JsonSerializer.Deserialize<List<ContentPage>>(siteSetting.ContentData, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                // Cache the result for 30 minutes
                _memoryCache.Set(CacheKey, pages, TimeSpan.FromMinutes(30));
            }

            return pages ?? new List<ContentPage>();
        }

        public ContentItem GetById(int id)
        {
            var pages = GetAllPages();
            return pages.SelectMany(p => p.Content).FirstOrDefault(c => c.Id == id);
        }

        public List<ContentItem> GetByPage(string pageName)
        {
            var pages = GetAllPages();
            var page = pages.FirstOrDefault(p => p.Page.Equals(pageName, StringComparison.OrdinalIgnoreCase));
            return page?.Content ?? new List<ContentItem>();
        }

        public void UpdateValue(int id, string newValue)
        {
            var pages = GetAllPages();
            var item = pages.SelectMany(p => p.Content).FirstOrDefault(c => c.Id == id);
            
            if (item != null)
            {
                item.Value = newValue;
                SaveChanges(pages);
            }
        }

        public void UpdateContent(List<ContentPage> pages)
        {
            SaveChanges(pages);
        }

        private void SaveChanges(List<ContentPage> pages)
        {
            var siteSetting = _siteSettingRepository.Table.FirstOrDefault();
            if (siteSetting == null)
            {
                siteSetting = new SiteSetting();
                _siteSettingRepository.Add(siteSetting);
            }

            siteSetting.ContentData = JsonSerializer.Serialize(pages, new JsonSerializerOptions { WriteIndented = true });
            _siteSettingRepository.Update(siteSetting);
            
            // Invalidate cache
            _memoryCache.Remove(CacheKey);
        }
    }
}
