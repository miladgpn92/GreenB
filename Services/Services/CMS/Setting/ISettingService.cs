using Common;
using Common.Enums;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CMS.Setting
{
    public interface ISettingService
    {
        ResponseModel<SettingSelectDto> GetSetting();
        ResponseModel SetPublicSetting(PublicSetting publicSetting);
        ResponseModel SetAddressCallSetting(AddressCallSetting addressCallSetting);
        ResponseModel SetSEOSetting(string SeoList);


       

    }
}
