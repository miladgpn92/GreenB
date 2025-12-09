using Common.Enums;
using Common;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS.GlobalSetting
{
    public interface IGlobalSettingService
    {
        ResponseModel<GetGlobalSettingDto> GetGlobalSetting();

        ResponseModel SetEmailSetting(EmailSetting emailSetting);
        ResponseModel SetSocialSetting(SocialSetting socialSetting);

        ResponseModel SetInjectCodeSetting(InjectSetting injectSetting);

        ResponseModel SetAISetting(AISetting aISetting);

        ResponseModel SetAIPageGenerator(AIPageGeneratorDto model);


        ResponseModel<bool> ChangeSMSServiceState();
    }
}
