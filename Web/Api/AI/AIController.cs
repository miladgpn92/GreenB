using Common;
using Common.Consts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS.AI;
using Shared.Api;
using SharedModels.Dtos.Shared;
using System.Data;
using System.Security.Claims;

namespace Web.Api.AI
{
    [ApiVersion("1")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class AIController : BaseController
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        /// <summary>
        /// گفتگو با هوش مصنوعی
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> Chat([FromBody] AIRequestModel requestModel)
        {
            // اضافه کردن شناسه کاربر به درخواست اگر کاربر لاگین است
            if (User.Identity.IsAuthenticated)
            {
                requestModel.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var result = await _aiService.Chat(requestModel);

            if (result.IsSuccess)
                return Ok(result.Model);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// دریافت اعتبار باقیمانده حساب هوش مصنوعی
        /// </summary>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetCredit()
        {
            var result = await _aiService.GetCredit();

            if (result.IsSuccess)
                return Ok(result.Model);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// ذخیره گفتگو
        /// </summary>
        [HttpPost("[action]")]
        public async Task<IActionResult> SaveConversation([FromBody] AIConversationHistoryDto conversation , CancellationToken cancellationToken)
        {
            // تنظیم شناسه کاربر فعلی اگر کاربر لاگین است
            if (User.Identity.IsAuthenticated)
            {
                conversation.UserId = User.Identity.GetUserIdInt();
            }

            var result = await _aiService.SaveConversation(conversation , cancellationToken);

            if (result.IsSuccess)
                return Ok(result.Model);
            else
                return BadRequest(result.Message);
        }

        /// <summary>
        /// دریافت تاریخچه گفتگوهای کاربر
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetConversations()
        {
            int userId = User.Identity.GetUserIdInt();

            var result = await _aiService.GetUserConversations(userId);

            if (result.IsSuccess)
                return Ok(result.Model);
            else
                return BadRequest(result.Message);
        }


        /// <summary>
        /// لیست مدل ها
        /// </summary>
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetModels()
        {
     

            var result = await _aiService.GetModels();

            if (result.IsSuccess)
                return Ok(result.Model);
            else
                return BadRequest(result.Message);
        }
    }
}
