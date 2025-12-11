using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class SMSUtility
    {
        public static int CalculateSMSCount(string text)
        {
            int smsCount = 0;
            int length = text.Length;

            if (IsTextPersian(text))
            {
                int firstPageLength = 70;
                int secondPageLength = 67;
                int remainingPageLength = 64;

                smsCount += length / firstPageLength; // تعداد صفحات صحیح
                int remainingChars = length % firstPageLength; // تعداد کاراکتر باقی‌مانده

                if (remainingChars > 0)
                {
                    smsCount++; // یک صفحه بیشتر برای کاراکترهای باقی‌مانده

                    if (remainingChars <= secondPageLength)
                    {
                        return smsCount; // صفحه دوم کافی است
                    }

                    remainingChars -= secondPageLength; // کاراکترهای باقی‌مانده بعد از صفحه دوم

                    int remainingPages = remainingChars / remainingPageLength; // تعداد صفحات باقی‌مانده
                    smsCount += remainingPages; // اضافه کردن تعداد صفحات باقی‌مانده به تعداد پیامک‌ها
                    remainingChars %= remainingPageLength; // کاراکترهای باقی‌مانده بعد از صفحات باقی‌مانده

                    if (remainingChars > 0)
                    {
                        smsCount++; // یک صفحه بیشتر برای کاراکترهای باقی‌مانده
                    }
                }
            }
            else // متن انگلیسی
            {
                int firstPageLength = 160;
                int secondPageLength = 146;
                int thirdPageLength = 153;
                int remainingPageLength = 153;

                smsCount += length / firstPageLength; // تعداد صفحات صحیح
                int remainingChars = length % firstPageLength; // تعداد کاراکتر باقی‌مانده

                if (remainingChars > 0)
                {
                    smsCount++; // یک صفحه بیشتر برای کاراکترهای باقی‌مانده

                    if (remainingChars <= secondPageLength)
                    {
                        return smsCount; // صفحه دوم کافی است
                    }

                    remainingChars -= secondPageLength; // کاراکترهای باقی‌مانده بعد از صفحه دوم

                    if (remainingChars <= thirdPageLength)
                    {
                        return smsCount + 1; // صفحه سوم کافی است و به صفحه بعدی نمی‌رسد
                    }

                    remainingChars -= thirdPageLength; // کاراکترهای باقی‌مانده بعد از صفحه سوم

                    int remainingPages = remainingChars / remainingPageLength; // تعداد صفحات باقی‌مانده
                    smsCount += remainingPages; // اضافه کردن تعداد صفحات باقی‌مانده به تعداد پیامک‌ها
                    remainingChars %= remainingPageLength; // کاراکترهای باقی‌مانده بعد از صفحات باقی‌مانده

                    if (remainingChars > 0)
                    {
                        smsCount++; // یک صفحه بیشتر برای کاراکترهای باقی‌مانده
                    }
                }
            }

            return smsCount;
        }

        public static bool IsTextPersian(string text)
        {
            // یک منطق ساده برای تشخیص متن فارسی
            // می‌توانید الگوریتم‌های تشخیص زبان پیچیده‌تری را نیز استفاده کنید
            foreach (char c in text)
            {
                if (c >= 0x600 && c <= 0x6FF) // حروف عربی و فارسی
                {
                    return true;
                }
            }
            return false;
        }
    }
}
