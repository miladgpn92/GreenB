using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAParser;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Services.CMS.ActiveSession
{
    public class ActiveSessionService : IScopedDependency, IActiveSessionService
    {
        private readonly IRepository<Entities.User.ActiveSession> _repositoryActiveSession;

        public IMapper Mapper { get; }

        public ActiveSessionService(IRepository<Entities.User.ActiveSession> repositoryActiveSession, IMapper Mapper)
        {
            _repositoryActiveSession = repositoryActiveSession;
            this.Mapper = Mapper;
        }
        public  ResponseModel AddActiveSession(int UserId, HttpContext context, string Jwt)
        {
            var Browser = GetBrowserType(context);
            var Device = GetDeviceType(context);
            var OS = GetOSType(context);
            string userAgent = context.Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);

            string DName = c.OS.Family + "-" + c.OS.Major;
            if (!string.IsNullOrEmpty(c.OS.Minor))
                DName += "." + c.OS.Minor;

            try
            {

                _repositoryActiveSession.Add(new Entities.User.ActiveSession()
                {
                    OSType = OS,    
                    DeviceName = DName,
                    CreatorUserId = UserId,
                    BrowserType = Browser,
                    DeviceType = Device,
                    LoginDate = DateTime.Now,
                    CreatorIP = context.Connection.RemoteIpAddress?.ToString(),
                    Token = EncryptUtility.Encrypt(Jwt)

                }); ;

                return new ResponseModel(true);
            }
            catch (Exception)
            {

                return new ResponseModel(false);
            }
        }

        public async Task<ResponseModel<List<ActiveSessionDto>>> GetSession(int UserId, HttpContext context, CancellationToken cancellationToken)
        {
            var res = await _repositoryActiveSession.TableNoTracking.Where(a => a.CreatorUserId == UserId).ToListAsync(cancellationToken);
            string currentToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            List<ActiveSessionDto> result = new List<ActiveSessionDto>();
            foreach (var item in res)
            {
                bool iSCurrent=false;
                if(currentToken == EncryptUtility.Decrypt(item.Token))
                {
                    iSCurrent = true;
                }

                result.Add(new ActiveSessionDto
                {
                    BrowserType = item.BrowserType,
                    DeviceName = item.DeviceName,
                    CreatorIP = item.CreatorIP,
                    CreatorUserId = item.CreatorUserId,
                    DeviceType = item.DeviceType,
                    Id = item.Id,
                    LoginDate = item.LoginDate,
                    OSType = item.OSType,
                    IsCurrent = iSCurrent
                });
            }
            return new ResponseModel<List<ActiveSessionDto>>(true, result);
        }

        public  ResponseModel RemoveActiveSession(int SessionId)
        {
            try
            {
                var find = _repositoryActiveSession.TableNoTracking.Where(a => a.Id == SessionId).FirstOrDefault();
                _repositoryActiveSession.Delete(find);
                return new ResponseModel(true); 
            }
            catch (Exception)
            {

                return new ResponseModel(false);
            }
         
        }


        private BrowserType GetBrowserType(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            BrowserType matchingBrowser = BrowserType.Unknown; // Initialize with Unknown

            foreach (BrowserType browser in Enum.GetValues(typeof(BrowserType)))
            {
                if (browser.ToString().ToLower().Equals(c.UA.Family.ToLower(), StringComparison.OrdinalIgnoreCase))
                {
                    matchingBrowser = browser;
                    break;
                }
            }
            return matchingBrowser;

        }

        private OSType GetOSType(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"].ToString();
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            OSType matchingOS = OSType.Unknown; // Initialize with Unknown

            foreach (OSType os in Enum.GetValues(typeof(OSType)))
            {
                if (os.ToString().ToLower().Equals(c.OS.Family.ToLower(), StringComparison.OrdinalIgnoreCase))
                {
                    matchingOS = os;
                    break;
                }
            }

            return matchingOS;


        }

        private DeviceType GetDeviceType(HttpContext context)
        {
            string userAgent = context.Request.Headers["User-Agent"].ToString();

            if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.Tablet;
            }

            if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.Mobile;
            }
            else if (userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.Tablet;
            }
            else if (userAgent.Contains("TV", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.TV;
            }
            else if (userAgent.Contains("Wearable", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.Wearable;
            }
            else if (userAgent.Contains("EReader", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.EReader;
            }
            else if (userAgent.Contains("Console", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.Console;
            }
            else if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase) ||
                     userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase) ||
                     userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
            {
                return DeviceType.Desktop;
            }
            else
            {
                return DeviceType.Unknown;
            }
        }
    }
}
