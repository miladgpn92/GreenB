using Common;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Web.TagHelpers.Common
{

    [HtmlTargetElement("dt:base-url")]
    public class BaseUrlTagHelper:TagHelper
    {
        private readonly ProjectSettings _settings;

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }

        public BaseUrlTagHelper(IOptions<ProjectSettings> settings)
        {
            _settings = settings.Value;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Attributes.Add("class", ClassName);
            output.Content.AppendHtml(_settings.ProjectSetting.BaseUrl);
        }
    }
}
