using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;

namespace Web.TagHelpers.Panel.Controls
{

    [HtmlTargetElement("dt:gender", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GenderTaghelper : TagHelper
    {

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        private readonly IHtmlGenerator _generator;


        public ModelExpression? AspFor { get; set; }

        private readonly IStringLocalizer<Global> GlobalLocalizer;


        public string? MaleTitle { get; set; }

        public string? FemaleTitle { get; set; }

        public GenderTaghelper(IHtmlGenerator generator, IStringLocalizer<ResourceLibrary.Panel.Common.Global.Global> _GlobalLocalizer)
        {
            GlobalLocalizer = _GlobalLocalizer;
            _generator = generator;

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Attributes.Add("class", "relative z-0 inline-flex shadow-sm rounded-md");
            output.Attributes.Add("id", "gender__container");

            string _MaleTitle = string.IsNullOrEmpty(MaleTitle) ? GlobalLocalizer["Male"] : MaleTitle;

            string _FemaleTitle = string.IsNullOrEmpty(FemaleTitle) ? GlobalLocalizer["Female"] : FemaleTitle;

            using (var writer = new StringWriter())
            {
                writer.Write($@"<div id=""genderMale"" onclick=""setGenderValue(1,this)""  class=""bg-white  cursor-pointer w-[56px] relative inline-flex items-center justify-center   py-2 rounded-r-md ltr:rounded-l-md ltr:rounded-r-[0px] border border-gray-300   text-sm font-medium text-gray-500   focus:z-10 focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"">  
{_MaleTitle}
</div>
  <div id=""genderFemale"" onclick=""setGenderValue(2,this)"" class=""bg-white cursor-pointer w-[56px] relative inline-flex items-center  justify-center py-2 rounded-l-md ltr:rounded-r-md ltr:rounded-l-[0px] border border-gray-300   text-sm font-medium text-gray-500  focus:z-10 focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"">
    {_FemaleTitle}
</div>");

                var hiddenValue = _generator.GenerateHidden(ViewContext,
            AspFor.ModelExplorer,
            AspFor.Name,
            AspFor.Model,
            false,
            new { @id = "GenderSelector" });

                hiddenValue.WriteTo(writer, NullHtmlEncoder.Default);

                output.Content.SetHtmlContent(writer.ToString());

            }





        }
    }
}
