using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Panel.Common.Global;
using Web.Pages.DTCMS.Components.CategoryMaker;

namespace Web.Pages.DTCMS.Components.Greeting
{
    public class GreetingViewComponent:ViewComponent
    {
        public GreetingViewComponent(IStringLocalizer<Global> globalLocalizer)
        {
            GlobalLocalizer = globalLocalizer;
        }

        public IStringLocalizer<Global> GlobalLocalizer { get; }

        public IViewComponentResult Invoke()
        {

            int hour = DateTime.Now.Hour;

            string GreetingText;

            if (hour >= 5 && hour < 12)
            {
                GreetingText = GlobalLocalizer["GoodMorning"];
            }
            else if (hour >= 12 && hour < 15)
            {
                GreetingText = GlobalLocalizer["GoodAfternoon"];
            }
            else if (hour >= 15 && hour < 18)
            {
                GreetingText = GlobalLocalizer["GoodEvening"];
            }
            else
            {
                GreetingText = GlobalLocalizer["GoodNight"];
            }

            return View("/Pages/DTCMS/Components/Greeting/Index.cshtml", GreetingText);
        }
    }
}
