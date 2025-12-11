using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace Web.TagHelpers.Common
{
    [HtmlTargetElement("dt:image",TagStructure =TagStructure.WithoutEndTag)]
    public class ImageTaghelper:TagHelper
    {
        public required string Src { get; set; }

        public string? Class { get; set; }


     
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder ImageTag = new TagBuilder("img");

            if (!string.IsNullOrEmpty(Src))
            {
                ImageTag.Attributes["src"] = Src;
            }
            else
            {
                ImageTag.Attributes["src"] = "/DTCMS/assets/images/no-pic/No-Pic-1024.svg";
            }

            ImageTag.Attributes["class"] = Class;

            output.TagName = null;
            output.Content.SetHtmlContent(ImageTag);
        }

    }

  
}
