using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Consts
{
    public class SiteConsts
    {
        public static string SiteBaseUploadUrl = "https://localhost:44318/upload/";
        public static bool IsDebug = System.Diagnostics.Debugger.IsAttached ? true : false;
        public static int SMSPricePerCharecter = 99;

    }
}
