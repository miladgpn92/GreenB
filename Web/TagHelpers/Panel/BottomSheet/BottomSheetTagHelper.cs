using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace Web.TagHelpers.Panel.BottomSheet
{
    /// <summary>
    /// Trigger with data-sheet-control="nima" data-html-append="<h1>hi</h1>"
    /// </summary>


    [HtmlTargetElement("dt:bottom-sheet", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BottomSheetTagHelper : TagHelper
    {


        /// <summary>
        /// add custom id to item
        /// </summary>
        [HtmlAttributeName(name: "id")]
        public string? Id { get; set; }


        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "min-height")]
        public int? MinHeight { get; set; } = 50;

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "disable-drag")]
        public bool? DisableDrag { get; set; } = false;


        [HtmlAttributeName(name: "title")]
        public string? Title { get; set; }

        [HtmlAttributeName(name: "Level")]
        public int? Level { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "div";
            output.Attributes.Add("class", "sheet z-30 "+ ClassName);
            output.Attributes.Add("id", Id);
            output.Attributes.Add("data-min-height", MinHeight);
            output.Attributes.Add("data-not-dragable", DisableDrag?.ToString().ToLower());
            output.Attributes.Add("aria-hidden", "true");
            output.Attributes.Add("role", "dialog");
            output.Content.AppendHtml(" <div class=\"overlay bg-gray-800 opacity-75 w-full\"></div>");

            TagBuilder Container = new TagBuilder("div");
            Container.AddCssClass("contents max-w-[390px] w-[390px] relative");

            if (string.IsNullOrEmpty(Title))
            {
                Container.InnerHtml.AppendHtml(" <header class=\"controls bg-white\"><div class=\"draggable-area w-full flex justify-center\"><div class=\"draggable-thumb w-12\"></div></div></header>");
            }
            else
            {
                string BackIcon;
                if (Level == 0 || Level == 1 || Level == null)
                {
                    BackIcon = "<span  data-sheet-dismiss class=\"icon-X text-xl text-gray-900\"></span> ";
                }
                else
                {
                    BackIcon = "<span  data-sheet-dismiss class=\"icon-Chevron-left text-xl text-gray-900 inline-block ltr:rotate-180\"></span>";
                }
                Container.InnerHtml.AppendHtml($" <div class=\"inline-flex items-center bg-white justify-between w-full px-4 pt-5\"> <span class=\"text-gray-900 text-sm font-normal \"> {Title}</span> <div class=\"cursor-pointer\"> {BackIcon} </div></div>");
            }



            TagBuilder ConentBody =new TagBuilder("main");
            ConentBody.AddCssClass("body bg-white p-4 ");
            ConentBody.InnerHtml.AppendHtml("<div id=\"extraContent\"></div>");
            ConentBody.InnerHtml.AppendHtml(await output.GetChildContentAsync());
 

            Container.InnerHtml.AppendHtml(ConentBody);

            output.Content.AppendHtml(Container);

         
        }
    }
}
