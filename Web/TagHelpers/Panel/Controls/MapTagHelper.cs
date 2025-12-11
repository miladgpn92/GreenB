using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;

namespace Web.TagHelpers.Panel.Controls
{
    [HtmlTargetElement("dt:map", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class MapTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;
        private readonly IHtmlGenerator _generator;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        [HtmlAttributeName(name: "lat")]
        public ModelExpression? AspForLatitude { get; set; }

        [HtmlAttributeName(name: "lng")]
        public ModelExpression? AspForLongitude { get; set; } // Added a public setter here.

        public MapTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator generator)
        {
            _generator = generator;
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            using (var writer = new StringWriter())
            {
                writer.Write(@"<div>");
                writer.Write(@$"<div data-lat='{AspForLatitude?.Name.Replace('.', '_')}' data-long='{AspForLongitude?.Name.Replace('.', '_')}' class='map h-[500px] w-full'></div>");

                if (AspForLatitude != null)
                {
                    var textboxLatitude = _generator.GenerateTextBox(ViewContext,
                        AspForLatitude.ModelExplorer,
                        AspForLatitude.Name,
                        AspForLatitude.Model,
                        null,
                       new { @class = AspForLatitude?.Name.Replace('.', '_') + " "+ "opacity-0 h-0 p-0 m-0" });

                    textboxLatitude.WriteTo(writer, NullHtmlEncoder.Default);
                }

                if (AspForLongitude != null)
                {
                    var textboxLongitude = _generator.GenerateTextBox(ViewContext,
                        AspForLongitude.ModelExplorer,
                        AspForLongitude.Name,
                        AspForLongitude.Model,
                        null,
                         new { @class = AspForLongitude?.Name.Replace('.', '_') + " " + "opacity-0 h-0 p-0 m-0" });

                    textboxLongitude.WriteTo(writer, NullHtmlEncoder.Default);
                }

                writer.Write(@"</div>");

                output.Content.SetHtmlContent(writer.ToString());
            }
        }
    }
}
