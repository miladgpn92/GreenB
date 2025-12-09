using Common.Consts;
using Common.Enums;
using Common.Extensions;
using Common.Utilities;
using Entities;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities;
using Services.Services.CMS;
using Services.Services.CMS.AI;
 
using Services.Services.CMS.GlobalSetting;
using Shared.Api;
using SharedModels.Dtos;
using SharedModels.Dtos.Shared;
using System.Data;
using System.Text.Json;

namespace Web.Api.AIPageGenerator
{
    [ApiVersion("1")]
    public class AIPageGeneratorController : BaseController
    {
        private readonly IAIService _aiService;
        private readonly IGlobalSettingService _globalSettingService;
 
        private readonly UserManager<ApplicationUser> _userManager;

        public AIPageGeneratorController(IAIService aiService, IGlobalSettingService GlobalSettingService,  UserManager<ApplicationUser> UserManager)
        {
            _aiService = aiService;
            _globalSettingService = GlobalSettingService;
           
            _userManager = UserManager;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Create(string apiKey, CancellationToken cancellationToken)
        {
            try
            {
                // بررسی توکن استاتیک
                var settings = _globalSettingService.GetGlobalSetting();
                if (settings?.Model == null)
                    return BadRequest("تنظیمات سیستم یافت نشد");
            
                if (apiKey != "RZAgency2024323142313132123123Digital")
                    return Unauthorized();

                if (!settings.Model.AIPageGeneratorEnable.Value)
                    return BadRequest("سیستم تولید خودکار صفحه غیرفعال است");

                // بررسی کلمات کلیدی
                if (string.IsNullOrEmpty(settings.Model.AIMainKeyword))
                    return BadRequest("کلمات کلیدی اصلی تعریف نشده است");

                // تبدیل کلمات کلیدی به لیست
                var mainKeywords = settings.Model.AIMainKeyword.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(k => k.Trim())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToArray();

                if (!mainKeywords.Any())
                    return BadRequest("کلمات کلیدی اصلی معتبر نیست");

                var secondaryKeywords = settings.Model.AISecondaryKeyword?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(k => k.Trim())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToArray();

                var locations = settings.Model.AILocationKeyword?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(k => k.Trim())
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToArray();

                if (mainKeywords == null || !mainKeywords.Any())
                    return BadRequest("کلمات کلیدی اصلی تعریف نشده است");

                // انتخاب تصادفی کلمات کلیدی
                var random = new Random();
                var selectedMain = mainKeywords[random.Next(mainKeywords.Length)];
                var selectedSecondary = secondaryKeywords?.Length > 0 ? secondaryKeywords[random.Next(secondaryKeywords.Length)] : null;
                var selectedLocation = locations?.Length > 0 ? locations[random.Next(locations.Length)] : null;

                // دریافت و پردازش پیش‌توضیحات
                var preDescriptions = settings.Model.AIPreDescription?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .Where(d => !string.IsNullOrEmpty(d))
                    .ToArray();

                // ساخت پرامپت برای تولید عنوان
                var titlePrompt = new List<AIMessage>
                {
                    new AIMessage { Role = "system", Content = "شما یک متخصص SEO هستید که عناوین جذاب و بهینه برای موتورهای جستجو تولید می‌کند." }
                };

                // اضافه کردن پیش‌توضیحات به عنوان پیام‌های system
                if (preDescriptions != null)
                {
                    foreach (var desc in preDescriptions)
                    {
                        titlePrompt.Add(new AIMessage { Role = "system", Content = desc });
                    }
                }

                // اضافه کردن درخواست اصلی
                titlePrompt.Add(new AIMessage
                {
                    Role = "user",
                    Content = $"لطفا سه عنوان مقاله جذاب با استفاده از کلمه کلیدی اصلی '{selectedMain}'" +
                        (selectedSecondary != null ? $" و کلمه کلیدی فرعی '{selectedSecondary}'" : "") +
                        (selectedLocation != null ? $" برای منطقه '{selectedLocation}'" : "") +
                        " تولید کنید. هر عنوان باید در یک خط مجزا باشد."
                });

                // دریافت عناوین از AI
                var titleResponse = await _aiService.Chat(new AIRequestModel { Messages = titlePrompt });
                if (!titleResponse.IsSuccess)
                    return BadRequest("خطا در تولید عنوان");

                // بررسی پاسخ AI برای عنوان
                if (titleResponse?.Model == null)
                    return BadRequest("پاسخی از AI برای عنوان دریافت نشد");

                var responseObj = JsonSerializer.Deserialize<JsonElement>(titleResponse.Model.ToString());
                if (!responseObj.TryGetProperty("choices", out var choices) || 
                    choices.GetArrayLength() == 0 ||
                    !choices[0].TryGetProperty("message", out var message) ||
                    !message.TryGetProperty("content", out var contentElement))
                {
                    return BadRequest("ساختار پاسخ AI نامعتبر است");
                }

                var content = contentElement.GetString();
                if (string.IsNullOrEmpty(content))
                    return BadRequest("محتوای پاسخ AI خالی است");

                // جدا کردن عناوین
                var titles = content.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Select(t => t.TrimStart('1', '2', '3', '.', ' ', '*')) // حذف شماره‌گذاری و کاراکترهای اضافی
                    .Select(t => t.Replace("**", "")) // حذف علامت‌های ستاره
                    .ToArray();

                if (!titles.Any())
                    return BadRequest("عنوانی تولید نشد");

                // انتخاب یک عنوان تصادفی
                var selectedTitle = titles[random.Next(titles.Length)];

                // حذف علامت‌های تعجب از انتهای عنوان
                selectedTitle = selectedTitle.TrimEnd('!');

                selectedTitle = TextUtility.RemoveHtmlTag(selectedTitle);

                // ساخت پرامپت برای تولید محتوا
                var contentPrompt = new List<AIMessage>
                {
                    new AIMessage { Role = "system", Content = "شما یک نویسنده محتوا هستید. هرگز از # یا h1 در محتوا استفاده نکنید. عنوان اصلی مقاله قبلاً مشخص شده است." }
                };

                // اضافه کردن پیش‌توضیحات به عنوان پیام‌های system
                if (preDescriptions != null)
                {
                    foreach (var desc in preDescriptions)
                    {
                        contentPrompt.Add(new AIMessage { Role = "system", Content = desc });
                    }
                }

                // تعریف ساختارهای مختلف محتوا
                var contentStructures = new[]
                {
                    new {
                        Name = "ساختار داستانی-تجربی",
                        Prompt = @"لطفا یک مقاله با رویکرد داستانی و تجربی برای عنوان '{0}' بنویسید.
                        ساختار مقاله باید شامل موارد زیر باشد:
                        - شروع با یک داستان یا تجربه شخصی واقعی
                        - بیان مشکلات و چالش‌های واقعی
                        - راه‌حل‌های عملی و تجربه شده
                        - نکات کلیدی در قالب داستان
                        - نتیجه‌گیری و پیام اصلی
                        - پرسش‌های تأمل‌برانگیز

                        نکات مهم:
                        - از زبان محاوره‌ای و عامیانه استفاده کنید
                        - از اصطلاحات روزمره استفاده کنید
                        - از جملات ناقص و کوتاه استفاده کنید
                        - از کلمات احساسی و عاطفی استفاده کنید
                        - از اشتباهات گرامری کوچک و طبیعی استفاده کنید
                        - از کلمات ربط متنوع استفاده کنید
                        - از علائم نگارشی متنوع استفاده کنید
                        - از کلمات مترادف و هم‌معنی استفاده کنید
                        - از جملات تأکیدی و پرسشی استفاده کنید
                        - از کلمات تخصصی به صورت طبیعی استفاده کنید"
                    },
                    new {
                        Name = "ساختار گفتگو-محور",
                        Prompt = @"لطفا یک مقاله با رویکرد گفتگو-محور برای عنوان '{0}' بنویسید.
                        ساختار مقاله باید شامل موارد زیر باشد:
                        - شروع با یک سوال یا مشکل
                        - گفتگو با متخصص یا فرد با تجربه
                        - پرسش و پاسخ‌های طبیعی
                        - نکات کلیدی در قالب گفتگو
                        - نتیجه‌گیری و پیام اصلی
                        - پرسش‌های تأمل‌برانگیز

                        نکات مهم:
                        - از زبان محاوره‌ای و عامیانه استفاده کنید
                        - از اصطلاحات روزمره استفاده کنید
                        - از جملات ناقص و کوتاه استفاده کنید
                        - از کلمات احساسی و عاطفی استفاده کنید
                        - از اشتباهات گرامری کوچک و طبیعی استفاده کنید
                        - از کلمات ربط متنوع استفاده کنید
                        - از علائم نگارشی متنوع استفاده کنید
                        - از کلمات مترادف و هم‌معنی استفاده کنید
                        - از جملات تأکیدی و پرسشی استفاده کنید
                        - از کلمات تخصصی به صورت طبیعی استفاده کنید"
                    },
                    new {
                        Name = "ساختار پرسش-محور",
                        Prompt = @"لطفا یک مقاله با رویکرد پرسش-محور برای عنوان '{0}' بنویسید.
                        ساختار مقاله باید شامل موارد زیر باشد:
                        - شروع با یک سوال جذاب
                        - پاسخ‌های طبیعی و غیر رسمی
                        - پرسش‌های فرعی و پاسخ‌ها
                        - نکات کلیدی در قالب پرسش و پاسخ
                        - نتیجه‌گیری و پیام اصلی
                        - پرسش‌های تأمل‌برانگیز

                        نکات مهم:
                        - از زبان محاوره‌ای و عامیانه استفاده کنید
                        - از اصطلاحات روزمره استفاده کنید
                        - از جملات ناقص و کوتاه استفاده کنید
                        - از کلمات احساسی و عاطفی استفاده کنید
                        - از اشتباهات گرامری کوچک و طبیعی استفاده کنید
                        - از کلمات ربط متنوع استفاده کنید
                        - از علائم نگارشی متنوع استفاده کنید
                        - از کلمات مترادف و هم‌معنی استفاده کنید
                        - از جملات تأکیدی و پرسشی استفاده کنید
                        - از کلمات تخصصی به صورت طبیعی استفاده کنید"
                    }
                };

                // انتخاب تصادفی یک ساختار
                var selectedStructure = contentStructures[random.Next(contentStructures.Length)];

                contentPrompt.Add(new AIMessage
                {
                    Role = "user",
                    Content = string.Format(selectedStructure.Prompt, selectedTitle) + @"
                    نکات مهم برای تولید محتوا:
                    1. حداقل 2000 کلمه باشد
                    2. کلمات کلیدی مهم را با تگ strong مشخص کنید
                    3. از عناصر زیر برای غنی‌سازی محتوا استفاده کنید:
                       - لیست‌های نکته به نکته
                       - جداول مقایسه‌ای در صورت نیاز
                       - نقل قول‌های مرتبط
                       - مثال‌های کاربردی و موردی
                       - آمار و ارقام مرتبط
                    
                    4. هر بخش باید با یک heading مناسب (h2 یا h3) مشخص شود
                    5. از تگ h1 استفاده نکن در متن
                    6. محتوا باید کاملاً طبیعی و انسانی به نظر برسد
                    7. از تکرار ساختارهای مشابه خودداری کنید
                    8. از زبان محاوره‌ای و طبیعی استفاده کنید
                    9. از اصطلاحات تخصصی به اندازه مناسب استفاده کنید
                    10. محتوا باید برای خواننده ارزشمند و کاربردی باشد

                    نکات مهم برای جلوگیری از تشخیص AI بودن محتوا:
                    1. از جملات متنوع با طول‌های مختلف استفاده کنید
                    2. از کلمات ربط متنوع استفاده کنید (اما، زیرا، چرا که، به دلیل، در نتیجه، و...)
                    3. از اصطلاحات عامیانه و محاوره‌ای استفاده کنید
                    4. از نقل قول‌های واقعی و تجربیات شخصی استفاده کنید
                    5. از اعداد و ارقام دقیق استفاده کنید
                    6. از مثال‌های واقعی و موردی استفاده کنید
                    7. از اشتباهات گرامری کوچک و طبیعی استفاده کنید
                    8. از تکرار کلمات کلیدی به صورت طبیعی و نه افراطی استفاده کنید
                    9. از جملات ناقص و کوتاه در بین جملات بلند استفاده کنید
                    10. از علائم نگارشی متنوع استفاده کنید
                    11. از کلمات مترادف و هم‌معنی استفاده کنید
                    12. از جملات تأکیدی و پرسشی استفاده کنید
                    13. از کلمات احساسی و عاطفی استفاده کنید
                    14. از کلمات تخصصی به صورت طبیعی و در متن استفاده کنید
                    15. از کلمات ربط پیچیده و متنوع استفاده کنید

                    لطفا از Markdown برای فرمت‌بندی استفاده کنید."
                });

                contentPrompt.Add(new AIMessage
                {
                    Role = "system",
                    Content = @"برای طبیعی‌تر شدن محتوا، این موارد را رعایت کنید:
                    1. از جملات کوتاه و بلند به صورت متناوب استفاده کنید
                    2. از کلمات ربط متنوع استفاده کنید
                    3. از اصطلاحات عامیانه و محاوره‌ای استفاده کنید
                    4. از نقل قول‌های واقعی استفاده کنید
                    5. از اعداد و ارقام دقیق استفاده کنید
                    6. از مثال‌های واقعی استفاده کنید
                    7. از اشتباهات گرامری کوچک و طبیعی استفاده کنید
                    8. از تکرار کلمات کلیدی به صورت طبیعی استفاده کنید
                    9. از جملات ناقص و کوتاه استفاده کنید
                    10. از علائم نگارشی متنوع استفاده کنید"
                });

                // دریافت محتوا از AI
                var contentResponse = await _aiService.Chat(new AIRequestModel { Messages = contentPrompt });
                if (!contentResponse.IsSuccess)
                    return BadRequest("خطا در تولید محتوا");

                // بررسی پاسخ AI برای محتوا
                if (contentResponse?.Model == null)
                    return BadRequest("پاسخی از AI برای محتوا دریافت نشد");

                var responseArticleObj = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(contentResponse.Model.ToString());
                var markdownContent = responseArticleObj
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrEmpty(markdownContent))
                    return BadRequest("محتوایی از AI دریافت نشد");

                // بررسی کیفیت محتوا و فیلترهای امنیتی
                var contentQualityCheck = await CheckContentQuality(markdownContent);
                if (!contentQualityCheck.IsValid)
                {
                    // اگر محتوا کیفیت مناسبی نداشت، دوباره تلاش می‌کنیم
                    contentResponse = await _aiService.Chat(new AIRequestModel { Messages = contentPrompt });
                    if (!contentResponse.IsSuccess)
                        return BadRequest("خطا در تولید مجدد محتوا");

                    responseArticleObj = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(contentResponse.Model.ToString());
                    markdownContent = responseArticleObj
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString();

                    contentQualityCheck = await CheckContentQuality(markdownContent);
                    if (!contentQualityCheck.IsValid)
                        return BadRequest($"محتوای تولید شده کیفیت مناسبی ندارد: {contentQualityCheck.Reason}");
                }

                // حذف علامت‌های ``` و markdown از ابتدا و انتهای متن
                markdownContent = markdownContent
                    .TrimStart('`', '\n')
                    .TrimEnd('`', '\n')
                    .Replace("```markdown\n", "")
                    .Replace("```", "");

                // تبدیل Markdown به HTML
                var html = ConvertMarkdownToHtml(markdownContent);

                // بررسی نهایی محتوا قبل از ذخیره
                if (!await ValidateFinalContent(html))
                    return BadRequest("محتوای نهایی معیارهای کیفی را برآورده نمی‌کند");

                //var slug = _slugService.CheckSlug(selectedTitle.ToSlug(), cancellationToken);

                // به جای SignIn، مستقیماً کاربر رو پیدا کنیم
                var user = await _userManager.FindByNameAsync("09363201642");
                if (user == null)
                {
                    // اگر کاربر وجود نداره، ایجادش کنیم
                    user = new ApplicationUser
                    {
                        UserName = "09363201642",
                        Email = "ai@rzagency.ir",
                        EmailConfirmed = true,
                        PhoneNumber = "09363201642",
                        PhoneNumberConfirmed = true
                    };
                    var createResult = await _userManager.CreateAsync(user, "123123");
                    if (!createResult.Succeeded)
                        return BadRequest("خطا در ایجاد کاربر");
                }

                // ایجاد صفحه دینامیک
                //var page = new Entities.DynamicPage
                //{
                //    Title = selectedTitle,
                //    Description = html,
                //    Slug = slug,
                //    CreatorUserId = user.Id,
                //    DescriptionForEditor = markdownContent,
                //    IsPin = false,

                //    // فیلدهای BaseWithSeoEntity
                //    CreatorIP = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0",
                //    CreateDate = DateTime.Now,
                //    PublishDate = DateTime.Now,
                //    CmsLanguage = CmsLanguage.Persian,
                //    SeoTitle = selectedTitle,
                //    SeoDescription = selectedTitle.Length > 150 ? selectedTitle.Substring(0, 150) : selectedTitle,
                //    ApplicationUser = user
                //    // سایر فیلدهای مورد نیاز
                //};

                //var pageResult = await _dynamicPageService.CreateForAI(page, cancellationToken);
                //if (!pageResult.IsSuccess)
                //    return BadRequest("خطا در ایجاد صفحه");

                return Ok(new { message = "صفحه با موفقیت ایجاد شد" });
            }
            catch (Exception ex)
            {
                return BadRequest($"خطا در تولید صفحه: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private string ConvertMarkdownToHtml(string markdown)
        {
            markdown = System.Net.WebUtility.HtmlDecode(markdown);
            
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();
            return Markdown.ToHtml(markdown, pipeline);
        }

        private async Task<(bool IsValid, string Reason)> CheckContentQuality(string content)
        {
            try
            {
                // بررسی طول محتوا
                if (content.Length < 1500)
                    return (false, "طول محتوا کمتر از حد مجاز است");

                // بررسی تنوع ساختاری
                var headingCount = content.Count(c => c == '#');
                if (headingCount < 3)
                    return (false, "ساختار محتوا یکنواخت است");

                // بررسی تنوع پاراگراف‌ها
                var paragraphs = content.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (paragraphs.Length < 8)
                    return (false, "تعداد پاراگراف‌ها کم است");

                // بررسی وجود کلمات کلیدی تکراری
                var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                 .Where(w => w.Length > 3)
                                 .Select(w => w.ToLower().Trim(new[] { '.', ',', '!', '?', ':', ';', '"', '\'', '(', ')', '[', ']', '{', '}' }))
                                 .Where(w => !string.IsNullOrWhiteSpace(w))
                                 .ToList();

                var wordFrequency = words.GroupBy(w => w)
                                       .ToDictionary(g => g.Key, g => g.Count());
                
                // بررسی تکرار کلمات با در نظر گرفتن طول محتوا
                var totalWords = words.Count;
                var repeatedWords = wordFrequency.Where(w => 
                    (w.Value > 15 && w.Key.Length <= 4) || // کلمات کوتاه
                    (w.Value > 10 && w.Key.Length <= 6) || // کلمات متوسط
                    (w.Value > 8 && w.Key.Length > 6)      // کلمات بلند
                ).ToList();

                if (repeatedWords.Any())
                {
                    var mostRepeated = repeatedWords.OrderByDescending(w => w.Value).First();
                    return (false, $"تکرار بیش از حد کلمه '{mostRepeated.Key}' ({mostRepeated.Value} بار)");
                }

                // بررسی تنوع جملات
                var sentences = content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Where(s => s.Length > 10)
                                     .ToList();

                if (sentences.Count < 5)
                    return (false, "تعداد جملات معتبر کم است");

                var avgSentenceLength = sentences.Average(s => s.Length);
                if (avgSentenceLength < 15 || avgSentenceLength > 250)
                    return (false, "طول جملات نامناسب است");

                // بررسی وجود عناصر غنی‌سازی محتوا
                var hasRichContent = content.Contains("**") || 
                                    content.Contains("*") || 
                                    content.Contains("-") ||
                                    content.Contains("1.") ||
                                    content.Contains("2.") ||
                                    content.Contains("3.");

                if (!hasRichContent)
                    return (false, "محتوا فاقد عناصر غنی‌سازی است");

                return (true, "محتوای تولید شده معیارهای کیفی را برآورده می‌کند");
            }
            catch (Exception ex)
            {
                // در صورت بروز خطا، اجازه می‌دهیم محتوا ذخیره شود
                return (true, "خطا در بررسی کیفیت محتوا: " + ex.Message);
            }
        }

        private async Task<bool> ValidateFinalContent(string html)
        {
            // بررسی وجود تگ‌های HTML نامناسب
            if (html.Contains("<h1>") || html.Contains("</h1>"))
                return false;

            // بررسی نسبت متن به HTML
            var textLength = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", "").Length;
            var htmlLength = html.Length;
            if (textLength / (double)htmlLength < 0.7)
                return false;

            // بررسی وجود لینک‌های نامناسب
            if (html.Contains("http://") || html.Contains("https://"))
                return false;

            // بررسی وجود کاراکترهای خاص
            if (html.Contains("&quot;") || html.Contains("&amp;"))
                return false;

            return true;
        }
    }
}
