using Common.Enums;
using System.Globalization;

namespace Common.Utilities
{
    /// <summary>
    /// CMS Extentions
    /// </summary>
    public static class CmsEx
    {
        /// <summary>
        /// گرفتن زبان جاری سیستم به صورت اینام
        /// </summary>
        /// <returns>CmsLanguage - Type of enum</returns>
        public static CmsLanguage GetCurrentLanguage()
        {
            var value = CultureInfo.CurrentCulture.Name;
            if (value == "en-US")
                return CmsLanguage.English;
            else if(value == "fa-IR")
                return CmsLanguage.Persian;
            else
                return CmsLanguage.Persian;
        }
    }
}
