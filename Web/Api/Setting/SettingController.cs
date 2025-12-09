using Common;
using Common.Consts;
using Common.Enums;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services.Services;
using Services.Services.CMS.GlobalSetting;
using Services.Services.CMS.Setting;
using Shared.Api;
using SharedModels.Dtos;
using System.Data;

namespace Web.Api.Setting
{
    [ApiVersion("1")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class SettingController : BaseController
    {
        private readonly ISettingService _settingService;

        public IGlobalSettingService _GlobalSettingService { get; }

        private readonly ProjectSettings _projectsetting;

        private readonly ISMSService _sMSService;

        public SettingController(ISMSService sMSService, ISettingService settingService, IGlobalSettingService globalSettingService, IOptionsSnapshot<ProjectSettings> settings)
        {
            _projectsetting = settings.Value;
            _settingService = settingService;
            _GlobalSettingService = globalSettingService;
            _sMSService = sMSService;
        }



        /// <summary>
        /// Gets the setting for the specified language.
        /// </summary>
        /// <param name="lang">The language.</param>
        /// <returns>The setting for the specified language.</returns>
        [HttpGet("[action]")]
        public ActionResult GetSetting(CmsLanguage lang)
        {
            var res = _settingService.GetSetting();
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }



        /// <summary>
        /// Gets the global setting.
        /// </summary>
        /// <returns>The global setting.</returns>

        [HttpGet("[action]")]
        public ActionResult GetGlobalSetting()
        {
            var res = _GlobalSettingService.GetGlobalSetting();
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }



        /// <summary>
        /// Sets the public setting.
        /// </summary>
        /// <param name="publicSetting">The public setting.</param>
        /// <param name="lang">The language.</param>
        /// <returns>
        /// Returns an action result.
        /// </returns>
        [HttpPost("[action]")]
        public ActionResult SetPublicSetting(PublicSetting publicSetting, CmsLanguage lang)
        {
            var res = _settingService.SetPublicSetting(publicSetting);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        /// <summary>
        /// Sets the address call setting.
        /// </summary>
        /// <param name="addressCallSetting">The address call setting.</param>
        /// <param name="lang">The language.</param>
        /// <returns>The action result.</returns>
        [HttpPost("[action]")]
        public ActionResult SetAddressCallSetting(AddressCallSetting addressCallSetting, CmsLanguage lang)
        {
            var res = _settingService.SetAddressCallSetting(addressCallSetting);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        /// <summary>
        /// Sets the SEO setting for the specified language.
        /// </summary>
        /// <param name="list">The list of SEO settings.</param>
        /// <param name="lang">The language.</param>
        /// <returns>The result of the operation.</returns>

        [HttpPost("[action]")]
        public ActionResult SetSEOSetting(string list, CmsLanguage lang)
        {
            var res = _settingService.SetSEOSetting(list);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        /// <summary>
        /// Sets the email setting.
        /// </summary>
        /// <param name="emailSetting">The email setting.</param>
        /// <returns>
        /// Returns an action result.
        /// </returns>
        [HttpPost("[action]")]
        public ActionResult SetEmailSetting(EmailSetting emailSetting)
        {
            var res = _GlobalSettingService.SetEmailSetting(emailSetting);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }




        /// <summary>
        /// Sets the social setting.
        /// </summary>
        /// <param name="socialSetting">The social setting.</param>
        /// <returns>
        /// Returns an action result based on the success of the operation.
        /// </returns>
        [HttpPost("[action]")]
        public ActionResult SetSocialSetting(SocialSetting socialSetting)
        {
            var res = _GlobalSettingService.SetSocialSetting(socialSetting);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        /// <summary>
        /// Sets the inject code setting.
        /// </summary>
        /// <param name="injectSetting">The inject setting.</param>
        /// <returns>The result of the action.</returns>
        [HttpPost("[action]")]
        public ActionResult SetInjectCodeSetting(InjectSetting injectSetting)
        {
            var res = _GlobalSettingService.SetInjectCodeSetting(injectSetting);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }




        /// <summary>
        /// Sends an SMS to the specified phone number.
        /// </summary>
        /// <param name="model">The model containing the phone number and text of the SMS.</param>
        /// <returns>
        /// Returns an OkResult if the SMS was sent successfully, or a BadRequestResult if the SMS failed to send.
        /// </returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> SendSMS(SendSMSDto model)
        {
            var res = await _sMSService.SendSMSAsync(_projectsetting.ProjectSetting.SMSToken, _projectsetting.ProjectSetting.BaseUrl, model.Phonenumber, model.Text);
            if (res.IsSuccess)
                return Ok(res);
            else
                return BadRequest(res);
        }



        /// <summary>
        /// IncreseSMSCharge increases the SMS charge by the given amount.
        /// </summary>
        /// <param name="Amount">The amount to increase the SMS charge by.</param>
        /// <returns>
        /// Returns an Ok response with the updated SMS charge if successful, otherwise a BadRequest response with the error message.
        /// </returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> IncreseSMSCharge(int Amount)
        {
            var res = await _sMSService.IncreseCharge(_projectsetting.ProjectSetting.BaseUrl, Amount);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Model);
        }



        /// <summary>
        /// Validates the SMS charge for the given amount and id.
        /// </summary>
        /// <returns>
        /// Returns an OK response if the validation is successful, otherwise returns a BadRequest response with the error description.
        /// </returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> ValidateSMSCharge(int Amount, int id)
        {
            var res = await _sMSService.ValidatePayment(_projectsetting.ProjectSetting.BaseUrl, Amount, id);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        [HttpGet("[action]")]
        public IActionResult ChangeSMSServiceState()
        {
            var res =  _GlobalSettingService.ChangeSMSServiceState();
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Model);
        }

    }
}
