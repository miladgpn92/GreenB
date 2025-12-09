using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;

namespace Web.TagHelpers.Panel.Controls
{
    [HtmlTargetElement("dt:file-uploader", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class FileUploaderTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        private readonly IHtmlGenerator _generator;

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }

        private readonly IStringLocalizer<Global> GlobalLocalizer;

        public ModelExpression? AspFor { get; set; }

        public bool IsMultiple { get; set; } = false;

        public FileUploaderTagHelper(IHtmlGenerator generator, IStringLocalizer<ResourceLibrary.Panel.Common.Global.Global> _GlobalLocalizer)
        {
            GlobalLocalizer = _GlobalLocalizer;
            _generator = generator;

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var uniqName = AspFor?.Name.Replace('.', '_');
            output.TagName = "div";

            var UploadHidden = _generator.GenerateHidden(ViewContext, AspFor?.ModelExplorer, AspFor?.Name, AspFor?.Model, true, new { });
            UploadHidden.Attributes.Add("data-defaultvalue-target", $"multi__file__preview__container__{uniqName}");
            UploadHidden.Attributes.Add("class", $"multi__hidden__input__uploader__{uniqName}"); 
            output.Content.AppendHtml(UploadHidden);

            string Multiple = "";
            if (IsMultiple)
                Multiple = "multiple";


#pragma warning disable CS8604 // Possible null reference argument.
            var content = $@"

            <div class=""relative cursor-pointer bg-gray-100 border-[2px] border-gray-200 border-dashed   h-[120px] w-[358px] rounded-md flex justify-center items-center text-center mx-auto flex-col"">
        <input data-accept-single=""{(!IsMultiple).ToString().ToLower()}"" data-doc-target=""multi__hidden__input__uploader__{uniqName}"" type=""file"" accept=""*"" {Multiple} class=""z-[2] opacity-0 w-full h-full absolute""
               oninput=""initFileUploader('multi__file__preview__container__{uniqName}')"" />

        <svg class=""mb-2"" width=""32"" height=""32"" viewBox=""0 0 32 32"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
            <path d=""M3.23075 32C2.31025 32 1.54167 31.6917 0.925 31.075C0.308333 30.4583 0 29.6898 0 28.7693V3.23075C0 2.31025 0.308333 1.54167 0.925 0.925C1.54167 0.308333 2.31025 0 3.23075 0H21.8116C22.2423 0 22.6609 0.0871842 23.0673 0.261551C23.4737 0.435884 23.8218 0.667933 24.1116 0.9577L31.0423 7.88845C31.3321 8.17818 31.5641 8.52627 31.7384 8.9327C31.9128 9.3391 32 9.75768 32 10.1884V28.7693C32 29.6898 31.6917 30.4583 31.075 31.075C30.4583 31.6917 29.6898 32 28.7693 32H3.23075ZM3.23075 30H28.7693C29.1282 30 29.4231 29.8846 29.6539 29.6539C29.8846 29.4231 30 29.1282 30 28.7693V10H23.6154C23.1513 10 22.766 9.8468 22.4596 9.5404C22.1532 9.23397 22 8.8487 22 8.3846V2H3.23075C2.87178 2 2.57692 2.11538 2.34615 2.34615C2.11538 2.57692 2 2.87178 2 3.23075V28.7693C2 29.1282 2.11538 29.4231 2.34615 29.6539C2.57692 29.8846 2.87178 30 3.23075 30ZM24 24C24.2846 24 24.5224 23.9045 24.7134 23.7135C24.9045 23.5225 25 23.2846 25 23C25 22.7154 24.9045 22.4776 24.7134 22.2866C24.5224 22.0955 24.2846 22 24 22H8C7.71537 22 7.47755 22.0955 7.28655 22.2866C7.09552 22.4776 7 22.7154 7 23C7 23.2846 7.09552 23.5225 7.28655 23.7135C7.47755 23.9045 7.71537 24 8 24H24ZM15 10C15.2846 10 15.5224 9.90448 15.7135 9.71345C15.9045 9.52242 16 9.2846 16 9C16 8.71537 15.9045 8.47755 15.7135 8.28655C15.5224 8.09552 15.2846 8 15 8H8C7.71537 8 7.47755 8.09552 7.28655 8.28655C7.09552 8.47755 7 8.71537 7 9C7 9.2846 7.09552 9.52242 7.28655 9.71345C7.47755 9.90448 7.71537 10 8 10H15ZM24 17C24.2846 17 24.5224 16.9045 24.7134 16.7135C24.9045 16.5225 25 16.2846 25 16C25 15.7154 24.9045 15.4775 24.7134 15.2865C24.5224 15.0955 24.2846 15 24 15H8C7.71537 15 7.47755 15.0955 7.28655 15.2865C7.09552 15.4775 7 15.7154 7 16C7 16.2846 7.09552 16.5225 7.28655 16.7135C7.47755 16.9045 7.71537 17 8 17H24Z""
                  fill=""#9CA3AF"" />
        </svg>

        <div class=""font-normal text-sm text-indigo-600""> {GlobalLocalizer["SingleUploaderTitle", AspFor?.Metadata.DisplayName]}   </div>
    </div>
    <div class=""flex flex-wrap justify-end w-[358px] mx-auto my-3 "" id=""multi__file__preview__container__{uniqName}"">
    </div>


";
#pragma warning restore CS8604 // Possible null reference argument.
            output.Content.AppendHtml(content);
        }
    }
}
