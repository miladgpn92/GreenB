using SharedModels.Dtos.Shared;
using SharedModels.Dtos.Shared.SharedModels.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS.Content
{
    public interface IContentService
    {
        List<ContentPage> GetAllPages();
        ContentItem GetById(int id);
        List<ContentItem> GetByPage(string pageName);
        void UpdateValue(int id, string newValue);
        void UpdateContent(List<ContentPage> pages);
    }
}
