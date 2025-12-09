using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Web.TagHelpers.Common
{

    [HtmlTargetElement("dt:text")]
    public class TextTagHelper:TagHelper
    {
        public bool RemoveHtml { get; set; } = false;

        public int? Length { get; set; } = 60;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await output.GetChildContentAsync();
            var contentHtml = content.GetContent();
            string cleanedString = contentHtml.Replace("\n", "").Replace("\r", "");
            cleanedString = cleanedString.Trim();
            if (RemoveHtml)
            {
                Regex reg = new Regex("<[^>]+>|&nbsp;+|&zwnj;");
                var maches = reg.Matches(cleanedString);
                foreach (Match item in maches)
                {
                    cleanedString = cleanedString.Replace(item.Value, " ");
                }
            }

            if (cleanedString.Length <= Length)
            {
                
            }
            else
            {
                if(Length != null)
                    cleanedString = cleanedString.Substring(0, Length.Value) + "...";
            }


            output.TagName = null;
            output.Content.SetContent(cleanedString);
        }
    }
}
