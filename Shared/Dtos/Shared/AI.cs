using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels.Dtos.Shared
{
    // مدل درخواست به AI
    public class AIRequestModel
    {
        public List<AIMessage> Messages { get; set; }
        public double? Temperature { get; set; }
        public int? MaxTokens { get; set; }
        public string UserId { get; set; }
        public string PageSource { get; set; } // صفحه منبع درخواست (مقاله، محصول، و غیره)
    }

    // ساختار پیام‌های AI
    public class AIMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    // تاریخچه گفتگوها
    public class AIConversationHistoryDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

         public string ChatGuid { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<AIMessage> Messages { get; set; }
        public string Status { get; set; } = "active";
        public string PageSource { get; set; } // صفحه منبع درخواست
    }



    public class AIModelResponse
    {
        public string Object { get; set; }
        public List<AIModelInfo> Data { get; set; }
    }

    public class AIModelInfo
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string OwnedBy { get; set; }
        public int MinTier { get; set; }
        public AIPricing Pricing { get; set; }
        public string Mode { get; set; }
    }

    public class AIPricing
    {
        public decimal Input { get; set; }
        public decimal CachedInput { get; set; }
        public decimal Output { get; set; }
    }
}
