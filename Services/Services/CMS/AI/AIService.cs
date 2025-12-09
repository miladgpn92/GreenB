using Common;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
 

namespace Services.Services.CMS.AI
{
    public class AIService : IAIService , IScopedDependency
    {
        private readonly IGlobalSettingService _globalSettingService;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository<Entities.AI.AIConversationHistory> _conversationRepository;

        public AIService(
            IGlobalSettingService globalSettingService,
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory,
            IRepository<Entities.AI.AIConversationHistory>  ConvertionRepository)
        {
            _globalSettingService = globalSettingService;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _conversationRepository = ConvertionRepository;
        }

        public async Task<ResponseModel<object>> Chat(AIRequestModel request)
        {
            try
            {
                // دریافت تنظیمات هوش مصنوعی
                var globalSettings = _globalSettingService.GetGlobalSetting();
                if (!globalSettings.IsSuccess)
                    return new ResponseModel<object>(false, null, "تنظیمات هوش مصنوعی یافت نشد");

                string aiToken = globalSettings.Model.AIToken;
                string aiModel = globalSettings.Model.AIModel ?? "gpt-4o-mini";

                // ساخت و ارسال درخواست به API
                var httpClient = _httpClientFactory.CreateClient("AIProvider");
                httpClient.BaseAddress = new Uri("https://api.avalai.ir/v1/");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiToken);

                var openAIRequest = new
                {
                    model = aiModel,
                    messages = request.Messages,
                    temperature = request.Temperature ?? 0.7,
                    max_tokens = request.MaxTokens ?? 2000
                };

                var response = await httpClient.PostAsJsonAsync("chat/completions", openAIRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<object>();
                    return new ResponseModel<object>(true, responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ResponseModel<object>(false, null, errorContent);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<object>(false, null, $"خطا در ارتباط با سرویس هوش مصنوعی: {ex.Message}");
            }
        }

        public async Task<ResponseModel<object>> GetCredit()
        {
            try
            {
                // استفاده از کش برای جلوگیری از درخواست‌های مکرر در بازه زمانی کوتاه
                string cacheKey = "AICredit";
                if (_memoryCache.TryGetValue(cacheKey, out object cachedCredit))
                {
                    return new ResponseModel<object>(true, cachedCredit);
                }

                // دریافت تنظیمات هوش مصنوعی
                var globalSettings = _globalSettingService.GetGlobalSetting();
                if (!globalSettings.IsSuccess)
                    return new ResponseModel<object>(false, null, "تنظیمات هوش مصنوعی یافت نشد");

                string aiToken = globalSettings.Model.AIToken;

                var httpClient = _httpClientFactory.CreateClient("AIProvider");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiToken);

                var response = await httpClient.GetAsync("https://api.avalai.ir/user/credit");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<object>();

                    // ذخیره در کش برای ۵ دقیقه
                    _memoryCache.Set(cacheKey, responseContent, TimeSpan.FromMinutes(5));

                    return new ResponseModel<object>(true, responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ResponseModel<object>(false, null, errorContent);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<object>(false, null, $"خطا در دریافت اعتبار: {ex.Message}");
            }
        }

        public async Task<ResponseModel<bool>> SaveConversation(AIConversationHistoryDto conversation, CancellationToken cancellationToken)
        {
            try
            {
                // بررسی وجود چت با همین GUID
                var existingConversation = await _conversationRepository.TableNoTracking
                    .FirstOrDefaultAsync(c => c.ChatGuid == conversation.ChatGuid);

                var entity = existingConversation ?? new Entities.AI.AIConversationHistory
                {
                    ChatGuid = conversation.ChatGuid,
                    UserId = conversation.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                entity.Title = conversation.Title;
                entity.Messages = JsonSerializer.Serialize(conversation.Messages);
                entity.Status = conversation.Status;
                entity.PageSource = conversation.PageSource;

                if (existingConversation == null)
                {
                    await _conversationRepository.AddAsync(entity, cancellationToken);
                }
                else
                {
                    _conversationRepository.Update(entity);
                }

                return new ResponseModel<bool>(true, true);
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>(false, false, $"خطا در ذخیره گفتگو: {ex.Message}");
            }
        }

        public async Task<ResponseModel<List<AIConversationHistoryDto>>> GetUserConversations(int userId)
        {
            try
            {
                var entities = await _conversationRepository.TableNoTracking
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                // تبدیل انتیتی‌ها به مدل‌ها
                var conversations = entities.Select(e => new AIConversationHistoryDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Title = e.Title,
                    CreatedAt = e.CreatedAt,
                    Messages = JsonSerializer.Deserialize<List<AIMessage>>(e.Messages),
                    Status = e.Status,
                    PageSource = e.PageSource // بازیابی منبع درخواست
                }).ToList();

                return new ResponseModel<List<AIConversationHistoryDto>>(true, conversations);
            }
            catch (Exception ex)
            {
                return new ResponseModel<List<AIConversationHistoryDto>>(false, null, $"خطا در دریافت تاریخچه گفتگوها: {ex.Message}");
            }
        }

        public async Task<ResponseModel<AIModelResponse>> GetModels()
        {
            try
            {
                // دریافت تنظیمات هوش مصنوعی
                var globalSettings = _globalSettingService.GetGlobalSetting();
                if (!globalSettings.IsSuccess)
                    return new ResponseModel<AIModelResponse>(false, null, "تنظیمات هوش مصنوعی یافت نشد");

                string aiToken = globalSettings.Model.AIToken;

                // ساخت و ارسال درخواست به API
                var httpClient = _httpClientFactory.CreateClient("AIProvider");
                httpClient.BaseAddress = new Uri("https://api.avalai.ir/v1/");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", aiToken);

                var response = await httpClient.GetAsync("models");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadFromJsonAsync<AIModelResponse>();
                    return new ResponseModel<AIModelResponse>(true, responseContent);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ResponseModel<AIModelResponse>(false, null, errorContent);
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel<AIModelResponse>(false, null, $"خطا در دریافت لیست مدل‌ها: {ex.Message}");
            }
        }
    }
}
