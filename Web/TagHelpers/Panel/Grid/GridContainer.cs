using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;

namespace Web.TagHelpers.Panel.Grid
{
    [HtmlTargetElement("dt:grid-container", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class GridContainer : TagHelper
    {
        private readonly IStringLocalizer<Global> _Localizer;

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }


        public int? Page { get; set; }
        public int Total { get; set; }

        public int? FilterCount { get; set; }

        public string? PageName { get; set; }

        public string? PageTitle { get; set; }

        public GridContainer(IStringLocalizer<ResourceLibrary.Panel.Common.Global.Global> Localizer)
        {
            _Localizer = Localizer;
        }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "div";
         

            if (Page == 1 && Total == 0 && FilterCount == 0)
            {

#pragma warning disable CS8604 // Possible null reference argument.
                string NoDataSubmited = @$"
<div class="" flex items-center justify-center flex-col mt-10"">
    <svg width=""48""
         height=""48""
         viewBox=""0 0 48 48""
         fill=""none""
         xmlns=""http://www.w3.org/2000/svg"">
        <path d=""M18 26H30H18ZM24 20V32V20ZM6 34V14C6 12.9391 6.42143 11.9217 7.17157 11.1716C7.92172 10.4214 8.93913 10 10 10H22L26 14H38C39.0609 14 40.0783 14.4214 40.8284 15.1716C41.5786 15.9217 42 16.9391 42 18V34C42 35.0609 41.5786 36.0783 40.8284 36.8284C40.0783 37.5786 39.0609 38 38 38H10C8.93913 38 7.92172 37.5786 7.17157 36.8284C6.42143 36.0783 6 35.0609 6 34Z""
              stroke=""#9CA3AF""
              stroke-width=""2""
              stroke-linecap=""round""
              stroke-linejoin=""round"" />
    </svg>
    <p class=""font-normal text-sm leading-5 mb-2 mt-6 text-gray-900"">
       {_Localizer["NothingPosted"]}
    </p>
    <span class=""font-normal text-sm leading-5 mb-8 text-gray-500"">
     {_Localizer["CreateFirst", PageTitle]}
</span>
    <a href=""/dtcms/{PageName}/upsert""
       class=""text-base text-blue-600 leading-6 border-dashed p-1 border-b-[1px] border-b-blue-600"">
         {_Localizer["Add"]}
    </a>
</div>";
#pragma warning restore CS8604 // Possible null reference argument.

                output.Content.AppendHtml(NoDataSubmited);
            }

            else if (Page == 1 && Total == 0 && FilterCount != 0)
            {
                string NoDataFoundAfterSubmit = @$"
<div class="" flex items-center justify-center flex-col mt-10"">
    <svg width=""48""
         height=""48""
         viewBox=""0 0 48 48""
         fill=""none""
         xmlns=""http://www.w3.org/2000/svg"">
        <path d=""M18 26H30H18ZM24 20V32V20ZM6 34V14C6 12.9391 6.42143 11.9217 7.17157 11.1716C7.92172 10.4214 8.93913 10 10 10H22L26 14H38C39.0609 14 40.0783 14.4214 40.8284 15.1716C41.5786 15.9217 42 16.9391 42 18V34C42 35.0609 41.5786 36.0783 40.8284 36.8284C40.0783 37.5786 39.0609 38 38 38H10C8.93913 38 7.92172 37.5786 7.17157 36.8284C6.42143 36.0783 6 35.0609 6 34Z""
              stroke=""#9CA3AF""
              stroke-width=""2""
              stroke-linecap=""round""
              stroke-linejoin=""round"" />
    </svg>
    <p class=""font-normal text-sm leading-5 mb-2 mt-6 text-gray-900"">
        {_Localizer["NothingFound"]}
    </p>
  
</div>";

                output.Content.AppendHtml(NoDataFoundAfterSubmit);
            }
            else
            {
                output.Content.AppendHtml((await output.GetChildContentAsync()));
            }






        }

    }
}
