using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;

namespace Web.TagHelpers.Panel.Controls
{

    [HtmlTargetElement("dt:seo-trigger", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SEOTriggerTagHelper : TagHelper
    {
        private readonly IStringLocalizer<Global> GlobalLocalizer;

        public SEOTriggerTagHelper(IStringLocalizer<ResourceLibrary.Panel.Common.Global.Global> _GlobalLocalizer)
        {
            GlobalLocalizer = _GlobalLocalizer;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "div";
            string itemClass = "h-[76px] mt-10 mb-10 seo flex items-center justify-between bg-gray-100 p-4 rounded-md cursor-pointer";
            output.Attributes.Add("class", itemClass);
            output.Attributes.Add("data-sheet-control", "seo__bottomsheet");

            output.Content.AppendHtml(@$" <div class="" flex items-center"">
                <div class=""icon"">
                    <svg width=""40""
                         height=""40""
                         viewBox=""0 0 40 40""
                         fill=""none""
                         xmlns=""http://www.w3.org/2000/svg"">
                        <g clip-path=""url(#clip0_1558_4549)"">
                            <path d=""M39.5751 20.4503C39.5751 19.1337 39.4584 17.8837 39.2584 16.667H20.425V24.1837H31.2084C30.725 26.6503 29.3084 28.7337 27.2084 30.1503V35.1503H33.6417C37.4084 31.667 39.5751 26.5337 39.5751 20.4503Z""
                                  fill=""#4285F4"" />
                            <path d=""M20.425 39.9997C25.825 39.9997 30.3416 38.1997 33.6416 35.1497L27.2083 30.1497C25.4083 31.3497 23.125 32.0831 20.425 32.0831C15.2083 32.0831 10.7916 28.5664 9.20829 23.8164H2.57495V28.9664C5.85828 35.4997 12.6083 39.9997 20.425 39.9997Z""
                                  fill=""#34A853"" />
                            <path d=""M9.20838 23.8165C8.79172 22.6165 8.57505 21.3332 8.57505 19.9999C8.57505 18.6665 8.80838 17.3832 9.20838 16.1832V11.0332H2.57505C1.20838 13.7332 0.425049 16.7665 0.425049 19.9999C0.425049 23.2332 1.20838 26.2665 2.57505 28.9665L9.20838 23.8165Z""
                                  fill=""#FBBC05"" />
                            <path d=""M20.425 7.91667C23.375 7.91667 26.0083 8.93334 28.0916 10.9167L33.7916 5.21667C30.3416 1.98333 25.825 0 20.425 0C12.6083 0 5.85828 4.5 2.57495 11.0333L9.20829 16.1833C10.7916 11.4333 15.2083 7.91667 20.425 7.91667Z""
                                  fill=""#EA4335"" />
                        </g>
                        <defs>
                            <clipPath id=""clip0_1558_4549"">
                                <rect width=""40"" height=""40"" fill=""white"" />
                            </clipPath>
                        </defs>
                    </svg>
                </div>
                <div class=""item pr-3 ltr:pr-0 ltr:pl-3"">
                    <h5 class=""font-bold text-sm text-black mb-2"">  {GlobalLocalizer["SEO"]}</h5>
                    <span class=""font-light text-xs text-black"">
                        {GlobalLocalizer["OptimizeGoogleDesc"]}
                    </span>
                </div>
            </div>
            <div class="""">
                <span class=""icon-Chevron-left inline-block	 text-xl text-gray-400 ltr:rotate-180""></span>
            </div>");
          

 




        }
    }
}
