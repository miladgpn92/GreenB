using Common;
using SharedModels.Dtos.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace Services.Services.CMS.AI
{
    public interface IAIService
    {
        Task<ResponseModel<object>> Chat(AIRequestModel request);

        // دریافت اعتبار حساب
        Task<ResponseModel<object>> GetCredit();

        // ذخیره گفتگو با هوش مصنوعی برای تاریخچه
        Task<ResponseModel<bool>> SaveConversation(AIConversationHistoryDto conversation , CancellationToken cancellationToken);

        // دریافت تاریخچه گفتگوهای کاربر
        Task<ResponseModel<List<AIConversationHistoryDto>>> GetUserConversations(int userId);

        // متد جدید برای دریافت لیست مدل‌ها
        Task<ResponseModel<AIModelResponse>> GetModels();
    }
}
