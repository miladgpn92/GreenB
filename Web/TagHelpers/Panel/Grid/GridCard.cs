using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using UAParser;

namespace Web.TagHelpers.Panel.Grid
{
    [HtmlTargetElement("dt:grid-card", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GridCard : TagHelper
    {

        /// <summary>
        /// Card has checkbox or not
        /// </summary>
        [HtmlAttributeName(name: "has-checkbox")]
        public bool HasCheckBox { get; set; } = true;

        /// <summary>
        /// Id of item that generate
        /// </summary>
        [HtmlAttributeName(name: "item-id")]
        public object? ItemId { get; set; }

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "div";
            string itemClass = "bg-white flex justify-between items-center shadow-sm py-4 px-2 rounded-md mb-4 " + (String.IsNullOrEmpty(ClassName) ? "" : ClassName);
            output.Attributes.Add("class", itemClass);
            
            if(HasCheckBox)
            {
                TagBuilder CheckBoxInput = new TagBuilder("input");
                CheckBoxInput.Attributes.Add("type", "checkbox");

                if(ItemId != null)
                {
                    CheckBoxInput.Attributes.Add("value", ItemId.ToString());

                }

                CheckBoxInput.Attributes.Add("class", "rounded border-gray-300 w-4 h-4 mx-2 grid__box");

                output.Content.AppendHtml(CheckBoxInput);
            }
            

            output.Content.AppendHtml((await output.GetChildContentAsync()));
      



        }

    }
}
