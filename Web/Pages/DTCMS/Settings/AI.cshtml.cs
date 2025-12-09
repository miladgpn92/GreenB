using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;
using Org.BouncyCastle.Utilities;
using Services.Services.CMS.AI;
using Services.Services.CMS.GlobalSetting;
using SharedModels.Dtos;

namespace Web.Pages.DTCMS.Settings
{
    public class AIModel : PageModel
    {
        private readonly IAIService _aIService;

        public AIModel(IGlobalSettingService service, IMapper mapper , IAIService AIService)
        {
            Service = service;
            Mapper = mapper;
            _aIService = AIService;
        }

      
        public IEnumerable<SelectListItem> ModelsList { get; set; }

 

        public IGlobalSettingService Service { get; }
        public IMapper Mapper { get; }

        [BindProperty]
        public AISetting? Items { get; set; } = new();

        public async Task OnGet(int? id, CancellationToken cancellationToken)
        {

            await LoadModels();
            var rs = Service.GetGlobalSetting();
            if (rs.IsSuccess)
            {
                Items = Mapper.Map<AISetting>(rs.Model);
            }
        }

        public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await LoadModels();
                return Page();
            }


            Service.SetAISetting(Items);
            return RedirectToPage("./Index");
        }


        private async Task LoadModels()
        {
            // ایجاد گروه‌ها یکبار
            var chatGPTGroup = new SelectListGroup { Name = "ChatGPT" };
            var alibabaGroup = new SelectListGroup { Name = "Alibaba" };
            var anthropicGroup = new SelectListGroup { Name = "Anthropic" };
      
            var deepseekGroup = new SelectListGroup { Name = "Deepseek" };
            var googleGroup = new SelectListGroup { Name = "Google" };
            var metaGroup = new SelectListGroup { Name = "Meta" };
            var xaiGroup = new SelectListGroup { Name = "XAI" };
            // مستقیماً لیست را به ModelsList اختصاص می‌دهیم
            ModelsList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "gpt-4o-mini",
                    Value = "gpt-4o-mini",
                    Group = chatGPTGroup,
                    Selected = Items?.AIModel == "gpt-4o-mini"
                },
                new SelectListItem
                {
                    Text = "gpt-4o",
                    Value = "gpt-4o",
                    Group = chatGPTGroup,
                    Selected = Items?.AIModel == "gpt-4o"
                },
                new SelectListItem
                {
                    Text = "gpt-3.5-turbo",
                    Value = "gpt-3.5-turbo",
                    Group = chatGPTGroup,
                    Selected = Items?.AIModel == "gpt-3.5-turbo"
                },
                new SelectListItem
                {
                    Text = "QWQ",
                    Value = "qwq-32b",
                    Group = alibabaGroup,
                    Selected = Items?.AIModel == "qwq-32b"
                }
                ,
                new SelectListItem
                {
                    Text = "Claude 3.5 Haiku",
                    Value = "anthropic.claude-3-5-haiku-20241022-v1:0",
                    Group = anthropicGroup,
                    Selected = Items?.AIModel == "anthropic.claude-3-5-haiku-20241022-v1:0"
                }
                 
                ,
                new SelectListItem
                {
                    Text = "Deepseek Chat",
                    Value = "deepseek-chat",
                    Group = deepseekGroup,
                    Selected = Items?.AIModel == "deepseek-chat"
                },
                new SelectListItem
                {
                    Text = "Deepseek Reasoner",
                    Value = "deepseek-reasoner",
                    Group = deepseekGroup,
                    Selected = Items?.AIModel == "deepseek-reasoner"
                },
                 new SelectListItem
                {
                    Text = "Gemini 2 - flash",
                    Value = "gemini-2.0-flash",
                    Group = googleGroup,
                    Selected = Items?.AIModel == "gemini-2.0-flash"
                },
                 new SelectListItem
                {
                    Text = "Gemini 2 - flash lite",
                    Value = "gemini-2.0-flash-lite",
                    Group = googleGroup,
                    Selected = Items?.AIModel == "gemini-2.0-flash-lite"
                },
                 new SelectListItem
                {
                    Text = "Gemini 2 - flash exp",
                    Value = "gemini-2.0-flash-exp",
                    Group = googleGroup,
                    Selected = Items?.AIModel == "gemini-2.0-flash-exp"
                },
                 new SelectListItem
                {
                    Text = "Gemini 1.5 - flash ",
                    Value = "gemini-1.5-flash",
                    Group = googleGroup,
                    Selected = Items?.AIModel == "gemini-1.5-flash"
                }
                 ,
                 new SelectListItem
                {
                    Text = "Llama 4 ",
                    Value = "llama-4-scout-17b-16e-instruct",
                    Group = metaGroup,
                    Selected = Items?.AIModel == "llama-4-scout-17b-16e-instruct"
                },
                 new SelectListItem
                {
                    Text = "Llama 3 ",
                    Value = "meta.llama3-1-8b-instruct-v1:0",
                    Group = metaGroup,
                    Selected = Items?.AIModel == "meta.llama3-1-8b-instruct-v1:0"
                }
                 ,
                 new SelectListItem
                {
                    Text = "Grok 3 beta ",
                    Value = "grok-3-beta",
                    Group = xaiGroup,
                    Selected = Items?.AIModel == "grok-3-beta"
                }
                  ,
                 new SelectListItem
                {
                    Text = "Grok 3 mini beta ",
                    Value = "grok-3-mini-beta",
                    Group = xaiGroup,
                    Selected = Items?.AIModel == "grok-3-mini-beta"
                }
            };
        }
    }
}
