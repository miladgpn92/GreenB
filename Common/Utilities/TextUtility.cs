using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Utilities
{
    /// <summary>
    /// عملیات های مرتبط متن
    /// </summary>
   public static class TextUtility
    {
        /// <summary>
        /// ایجاد محدودیت کاراکتری بر روی تکست
        /// </summary>
        public static string TextLimitLength(string Text, int maxLength)
        {
            if (Text != null)
            {
                if (Text.Length <= maxLength)
                {
                    return Text;
                }
                else
                {
                    return Text.Substring(0, maxLength) + "...";
                }
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// حذف تگ های html از متن
        /// </summary>
        public static string RemoveHtmlTag(string HtmlCode)
        {
            if (HtmlCode != null)
            {
                Regex reg = new Regex("<[^>]+>|&nbsp;+|&zwnj;");
                var maches = reg.Matches(HtmlCode);
                foreach (Match item in maches)
                {
                    HtmlCode = HtmlCode.Replace(item.Value, " ");
                }
                return HtmlCode;
            }
            else
            {
                return " ";
            }

        }



        public static string TextLimit(this string input, int maxLength, bool removeHtml = false)
        {
            input = input.Trim();   
         
            if (string.IsNullOrEmpty(input) || maxLength <= 0)
                return input;

            if (removeHtml)
            {
                // Remove HTML tags using regex
                input = Regex.Replace(input, "<[^>]+>|&nbsp;+|&zwnj;", string.Empty);
            }

            if (input.Length <= maxLength)
            {
                return input;
            }
            else
            {
                return input.Substring(0, maxLength);
            }
        }

    }
}
