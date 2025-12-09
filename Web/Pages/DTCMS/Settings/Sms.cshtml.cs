using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ResourceLibrary.Resources.ErrorMsg;
using Services.Services;
using Services.Services.CMS.GlobalSetting;
using System.ComponentModel.DataAnnotations;

namespace Web.Pages.DTCMS.Settings
{
    public class SmsModel : PageModel
    {
        private readonly IGlobalSettingService _globalSettingService;
        private readonly ISMSService _sMSService;
        private readonly ProjectSettings _settings;

        public SmsModel(IGlobalSettingService globalSettingService, ISMSService sMSService, IOptions<ProjectSettings> settings)
        {
            _globalSettingService = globalSettingService;
            _sMSService = sMSService;
          _settings = settings.Value;
        }

        [BindProperty]
        [Display(Name = "CreditAmount", ResourceType = typeof(ResourceLibrary.Panel.Admin.Settings.Settings))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequierdMsg", ErrorMessageResourceType = typeof(ErrorMsg))]
        public int CreditAmount { get; set; }


        [BindProperty]
        public double SMSCredit { get; set; }
        public async Task OnGetAsync(int? id , int? amount)
        {
       
           if(id != null && amount != null)
            {
               var validateRes= await _sMSService.ValidatePayment(_settings.ProjectSetting.BaseUrl, amount.Value, id.Value);
               if(validateRes.IsSuccess)
                {
                    var res = _globalSettingService.GetGlobalSetting();
                    if (res.IsSuccess)
                    {
                        SMSCredit = res.Model.SMSCredit;
                    }
                }
                else
                {
                    var res = _globalSettingService.GetGlobalSetting();
                    if (res.IsSuccess)
                    {
                        SMSCredit = res.Model.SMSCredit;
                    }
                }
            }
            else
            {
                var res = _globalSettingService.GetGlobalSetting();
                if (res.IsSuccess)
                {
                    SMSCredit = res.Model.SMSCredit;
                }
            }
            
          
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res= await _sMSService.IncreseCharge(_settings.ProjectSetting.BaseUrl, CreditAmount);
            if (res.IsSuccess)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                ExternalApiResponse apiResponse = JsonConvert.DeserializeObject<ExternalApiResponse>(res.Model);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                return new RedirectResult(apiResponse.data);
            }
            else
            {
                return Page();
            }
          
        }
    }
}
