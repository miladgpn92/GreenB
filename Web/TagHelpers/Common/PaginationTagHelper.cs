
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.RegularExpressions;

namespace Web.TagHelpers.Common
{

    [HtmlTargetElement("dt:pagination")]
    public class PaginationTagHelper : TagHelper
    {

        private IUrlHelperFactory urlHelperFactory;

        public PaginationTagHelper(IUrlHelperFactory helperFactory, IHttpContextAccessor httpContextAccessor)
        {
            urlHelperFactory = helperFactory;
            HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Current Page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int Size { get; set; } = 10;

        /// <summary>
        /// Total
        /// </summary>
        public int? Total { get; set; }

        /// <summary>
        /// add custom class to item
        /// </summary>
        [HtmlAttributeName(name: "class")]
        public string? ClassName { get; set; }





        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {


            // Get the query string value from the HttpContextAccessor
            var QueryString = HttpContextAccessor?.HttpContext?.Request.QueryString.Value;

            // Define a regular expression pattern to match the "arg.PageNumber" and "arg.PageSize" query parameters
            string pattern = @"(&?arg\.PageNumber=\d+&arg\.PageSize=\d+)|(&?arg\.PageSize=\d+&arg\.PageNumber=\d+)";

            // Check if the query string is not empty
            if (!string.IsNullOrEmpty(QueryString))
            {
                // Remove the leading question mark if it exists
                if (QueryString.StartsWith("?"))
                {
                    QueryString = QueryString.Substring(1); // Remove the leading question mark
                }
                // Remove the "arg.PageNumber" and "arg.PageSize" query parameters from the query string using regular expression replacement
                QueryString = Regex.Replace(QueryString, pattern, string.Empty);
            }

#pragma warning disable CS8604 // Possible null reference argument.
            var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
#pragma warning restore CS8604 // Possible null reference argument.
            string url = urlHelper.ActionContext.HttpContext.Request.Path;

            //Logic
            int? Next;
            int? Prev;
            if (Page <= 1)
            {
                Prev = null;
            }
            else
            {
                Prev = Page - 1;
            }

            if (Page == 1 && Total < Size)
            {
                Next = null;
            }
            else
            {
                Next = Page + 1;
            }


            output.TagName = "div";
            string itemClass = "flex justify-between items-center" + (String.IsNullOrEmpty(ClassName) ? "" : ClassName);
            output.Attributes.Add("class", itemClass);

            TagBuilder PaginationForm = new TagBuilder("form");
            PaginationForm.Attributes.Add("method", "get");

            TagBuilder PagingSection = new TagBuilder("div");
            PagingSection.AddCssClass("relative z-0 inline-flex shadow-sm rounded-md ");


            TagBuilder PrevPage = new TagBuilder("a");
            PrevPage.AddCssClass("-ml-px relative inline-flex items-center px-4 py-2 rounded-r-md bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 border border-solid border-gray-300 disabled:cursor-not-allowed ltr:rounded-r-none ltr:rounded-l-md");
            PrevPage.InnerHtml.AppendHtml("<span class=\"sr-only\">Next</span><span class=\"inline-block icon-Chevron-right text-xl font-bold ltr:rotate-180 \"></span>");
            if (Prev == null)
            {
                PrevPage.Attributes.Add("href", "#");
                PrevPage.Attributes.Add("disabled", "disabled");
            }
            else
            {
                PrevPage.Attributes.Add("href", url + "?arg.PageNumber=" + Prev + "&arg.PageSize=" + Size + (QueryString  != null ? "&"+QueryString : ""));
            }

            PagingSection.InnerHtml.AppendHtml(PrevPage);


            TagBuilder PageInput = new TagBuilder("input");
            PageInput.Attributes.Add("type", "number");
            PageInput.Attributes.Add("name", "arg.PageNumber");
            PageInput.Attributes.Add("id", "pageNumber");
            PageInput.Attributes.Add("class", "w-[44px] shadow-sm focus:ring-gray-300 focus:border-gray-300 block sm:text-sm border-gray-300 rounded-none text-center [appearance:textfield] [&::-webkit-outer-spin-button]:appearance-none [&::-webkit-inner-spin-button]:appearance-none");
            PageInput.Attributes.Add("value", Page.ToString());


            PagingSection.InnerHtml.AppendHtml(PageInput);

            TagBuilder PageSizeHiddenInput = new TagBuilder("input");

            PageSizeHiddenInput.Attributes.Add("type", "hidden");
            PageSizeHiddenInput.Attributes.Add("name", "arg.PageSize");
            PageSizeHiddenInput.Attributes.Add("id", "pageSize");
            PageSizeHiddenInput.Attributes.Add("value", Size.ToString());

            PagingSection.InnerHtml.AppendHtml(PageSizeHiddenInput);
        

            if(QueryString != null) {

                var queryStrings = HttpContextAccessor?.HttpContext?.Request.Query;
                if(queryStrings != null)
                {
                    foreach (var query in queryStrings)
                    {

                        TagBuilder QueryStringHiddenInput = new TagBuilder("input");

                        QueryStringHiddenInput.Attributes.Add("type", "hidden");
                        QueryStringHiddenInput.Attributes.Add("name", query.Key);

                        QueryStringHiddenInput.Attributes.Add("value", query.Value);

                        PagingSection.InnerHtml.AppendHtml(QueryStringHiddenInput);

                    }
                }
            

            }




            TagBuilder NextPage = new TagBuilder("a");
            NextPage.AddCssClass("relative inline-flex items-center px-4 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 border border-solid border-gray-300 border-r-0 disabled:cursor-not-allowed ltr:rounded-l-none ltr:rounded-r-md");
            NextPage.InnerHtml.AppendHtml("<span class=\"sr-only\">Previous</span> <span class=\"inline-block icon-Chevron-left text-xl font-bold ltr:rotate-180 \"></span>");
            if (Next == null)
            {
                NextPage.Attributes.Add("href", "#");
                NextPage.Attributes.Add("disabled", "disabled");
            }
            else
            {
                NextPage.Attributes.Add("href", url + "?arg.PageNumber=" + Next + "&arg.PageSize=" + Size + (QueryString != null ? "&"+QueryString : ""));
            }
            PagingSection.InnerHtml.AppendHtml(NextPage);
            PaginationForm.InnerHtml.AppendHtml(PagingSection);
            output.Content.AppendHtml(PaginationForm);

            TagBuilder PagingSize = new TagBuilder("div");
            PagingSize.AddCssClass("w-16");

            TagBuilder PageSizeForm = new TagBuilder("form");
            PageSizeForm.Attributes.Add("method", "get");


            TagBuilder PageNumberHiddenInput = new TagBuilder("input");

            PageNumberHiddenInput.Attributes.Add("type", "hidden");
            PageNumberHiddenInput.Attributes.Add("name", "arg.PageNumber");
            PageNumberHiddenInput.Attributes.Add("id", "pageNumber");
            PageNumberHiddenInput.Attributes.Add("value", Page.ToString());

            PageSizeForm.InnerHtml.AppendHtml(PageNumberHiddenInput);


            TagBuilder SelectPageSize = new TagBuilder("select");

            SelectPageSize.Attributes.Add("name", "arg.PageSize");
            SelectPageSize.Attributes.Add("onchange", "submitPageSize()");
            SelectPageSize.Attributes.Add("size", "1");
            SelectPageSize.Attributes.Add("class", " block w-full h-[46px] w-[66px]  text-base bg-white border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md ltr:text-center");

            SelectPageSize.InnerHtml.AppendHtml($@"
                    <option {(Size == 10 ? "Selected" : "")}>10</option>
                    <option {(Size == 30 ? "Selected" : "")}>30</option>
                    <option {(Size == 50 ? "Selected" : "")}>50</option>
            ");

            PageSizeForm.InnerHtml.AppendHtml(SelectPageSize);




            PagingSize.InnerHtml.AppendHtml(PageSizeForm);

            output.Content.AppendHtml(PagingSize);

        }
    }
}
