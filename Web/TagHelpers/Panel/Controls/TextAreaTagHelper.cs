using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using Web.TagHelpers.Enums;

namespace Web.TagHelpers.Panel.Controls
{
    [HtmlTargetElement("dt:text-area", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TextAreaTagHelper : TagHelper
    {
        private readonly IHtmlHelper _htmlHelper;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public ModelExpression AspFor { get; set; }

        public TextAlign? TextAlign { get; set; }


        public string? Placeholder { get; set; }
        public string? TargetClass { get; set; }

        public string? HasSlug { get; set; }

        public bool HasCounter { get; set; } = false;

        public string? CounterTarget { get; set; }
        private readonly IHtmlGenerator _generator;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TextAreaTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator generator)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            _generator = generator;
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            if (AspFor.Name.StartsWith("SEO"))
            {
                AspFor = (ModelExpression)AspFor.Model;
            }


            var isRTL = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            ((IViewContextAware)_htmlHelper).Contextualize(ViewContext);
            using (var writer = new StringWriter())
            {
                writer.Write(@"<div class=""dt__input__text"">");

                var labelClasses = "block text-sm font-medium text-gray-700";
                if (isRTL)
                {
                    labelClasses += " text-right";
                }
                else
                {
                    labelClasses += " text-left";
                }
                var label = _generator.GenerateLabel(
                                ViewContext,
                                AspFor?.ModelExplorer,
                                AspFor?.Name, null,
                                new { @class = labelClasses });

                label.WriteTo(writer, NullHtmlEncoder.Default);

                writer.Write(@"<div class=""mt-1"">");

                var textboxClasses = "appearance-none block w-full h-28 px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm w-full text-right ltr:text-left";

                if (TextAlign != null)
                {
                    if (TextAlign == Enums.TextAlign.Center)
                    {
                        textboxClasses += " text-center";
                    }
                    if (TextAlign == Enums.TextAlign.Right)
                    {
                        textboxClasses += " text-right";
                    }
                    if (TextAlign == Enums.TextAlign.Left)
                    {
                        textboxClasses += " text-left";
                    }
                }
                else
                {
                    if (isRTL == true)
                    {
                        textboxClasses += " text-right";
                    }
                    else
                    {
                        textboxClasses += " text-left";
                    }
                }


                var textbox = _generator.GenerateTextArea(ViewContext,
                         AspFor?.ModelExplorer,
                         AspFor?.Name,
                         0, 0, new { @class = textboxClasses, placeholder = Placeholder });
                textbox.Attributes.Add("data-target-class", TargetClass);
                textbox.Attributes.Add("data-has-slug", HasSlug);
                if (HasCounter)
                {
                    textbox.Attributes.Add("data-has-counter", HasCounter.ToString());
                    textbox.Attributes.Add("data-counter-target", CounterTarget);
                }
                textbox.WriteTo(writer, NullHtmlEncoder.Default);



                writer.Write(@"</div>");



                string ValidationContainerClasses = "validation__container";
                if (isRTL)
                {
                    ValidationContainerClasses += " text-right";
                }
                else
                {
                    ValidationContainerClasses += " text-left";
                }


                writer.Write(@$"<div class='{ValidationContainerClasses}'>");


                var validationClasses = "text-xs text-red-600 font-light mt-2.5";
                if (isRTL)
                {
                    validationClasses += " text-right";
                }
                else
                {
                    validationClasses += " text-left";
                }

                var validationMsg = _generator.GenerateValidationMessage(
                                        ViewContext,
                                        AspFor?.ModelExplorer,
                                        AspFor?.Name,
                                        null,
                                        ViewContext?.ValidationMessageElement,
                                        new { @class = validationClasses });

                validationMsg.WriteTo(writer, NullHtmlEncoder.Default);

                writer.Write(@"</div>");

                writer.Write(@"</div>");

                output.Content.SetHtmlContent(writer.ToString());


            }
        }

    }
}
