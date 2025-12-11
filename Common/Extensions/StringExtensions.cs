using System.Linq;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            // حذف کاراکترهای خاص و جایگزینی با خط تیره
            text = text.Replace(":", "-")
                      .Replace(".", "-")
                      .Replace("!", "-")
                      .Replace("?", "-")
                      .Replace("،", "-")
                      .Replace(",", "-")
                      .Replace("؛", "-")
                      .Replace(";", "-")
                      .Replace("_", "-")
                      .Replace(" ", "-")
                      .Replace("--", "-");

            // حذف کاراکترهای غیرمجاز
            text = new string(text.Where(c => 
                char.IsLetterOrDigit(c) || 
                c == 'آ' || c == 'ا' || c == 'ب' || c == 'پ' || c == 'ت' || 
                c == 'ث' || c == 'ج' || c == 'چ' || c == 'ح' || c == 'خ' ||
                c == 'د' || c == 'ذ' || c == 'ر' || c == 'ز' || c == 'ژ' ||
                c == 'س' || c == 'ش' || c == 'ص' || c == 'ض' || c == 'ط' ||
                c == 'ظ' || c == 'ع' || c == 'غ' || c == 'ف' || c == 'ق' ||
                c == 'ک' || c == 'گ' || c == 'ل' || c == 'م' || c == 'ن' ||
                c == 'و' || c == 'ه' || c == 'ی' || c == 'ئ' || c == '-'
            ).ToArray());

            // حذف خط تیره‌های تکراری
            while (text.Contains("--"))
            {
                text = text.Replace("--", "-");
            }

            // حذف خط تیره از ابتدا و انتها
            return text.Trim('-');
        }
    }
} 