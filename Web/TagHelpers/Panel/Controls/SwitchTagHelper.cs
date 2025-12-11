using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Web.TagHelpers.Panel.Controls
{
    [HtmlTargetElement("dt:switch", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SwitchTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public ModelExpression AspFor { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public string On { get; set; } = "فعال";
        public string Off { get; set; } = "غیر فعال";

        private readonly IHtmlGenerator _generator;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SwitchTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator generator)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _generator = generator;
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            var uniqName = AspFor?.Name.Replace('.', '_');


            if (ViewContext == null)
                throw new InvalidOperationException("ViewContext cannot be null");

            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);

            // Set the tag name and add the base classes
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "cursor-pointer border border-gray-300 rounded-md border-solid px-2");

            // Create the label element
            var label = new TagBuilder("label");
            label.AddCssClass("mt-3 mb-2 flex items-center justify-between ml-[64px] mr-[8px] cursor-pointer");
            label.Attributes.Add("for", uniqName);

            // Create the title and description container
            var textContainer = new TagBuilder("div");

            if (!string.IsNullOrEmpty(Title))
            {
                var titleElement = new TagBuilder("h4");
                titleElement.AddCssClass("font-bold text-sm text-black mb-2 cursor-pointer");
                titleElement.InnerHtml.Append(Title);
                textContainer.InnerHtml.AppendHtml(titleElement);
            }

            if (!string.IsNullOrEmpty(Desc))
            {
                var descElement = new TagBuilder("p");
                descElement.AddCssClass("font-light text-xs text-black max-w-[190px] cursor-pointer");
                descElement.InnerHtml.Append(Desc);
                textContainer.InnerHtml.AppendHtml(descElement);
            }

            // Create the toggle container
            var toggleContainer = new TagBuilder("div");
            toggleContainer.Attributes.Add("id", "toggles");

            // Create the checkbox input
            var checkboxInput = _generator.GenerateCheckBox(
                ViewContext,
                AspFor.ModelExplorer,
                AspFor.Name,
                null,
                new { @class = "ios-toggle" });

            // Create the toggle label
            var toggleLabel = new TagBuilder("label");
            toggleLabel.Attributes.Add("for", uniqName);
            toggleLabel.AddCssClass("checkbox-label");
            toggleLabel.Attributes.Add("data-off", Off);
            toggleLabel.Attributes.Add("data-on", On);

            // Combine all elements
            toggleContainer.InnerHtml.AppendHtml(checkboxInput);
            toggleContainer.InnerHtml.AppendHtml(toggleLabel);

            label.InnerHtml.AppendHtml(textContainer);
            label.InnerHtml.AppendHtml(toggleContainer);

            output.Content.SetHtmlContent(label);
        }

    }
}
