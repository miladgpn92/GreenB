using Common;
using Common.Enums;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ResourceLibrary;
using ResourceLibrary.Resources.Auth;
using Services.Services.CMS.ActiveSession;
using Services.Services.Email;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Services.Services.CMS.Auth
{
    public class AuthService : IScopedDependency, IAuthService
    {
       
        private readonly IStringLocalizer<AuthRes> _authLocalizer;

        /// <summary>
        /// Injection
        /// </summary>
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISMSService _sMSService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ProjectSettings _projectsetting;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepository<AspNetUserJwtRefreshToken> _JwtTableRepository;
        private readonly IActiveSessionService _activeSessionService;
        private readonly IHostEnvironment _environment;

  

        public IStringLocalizer<SharedResource> _SharedLocalizer { get; }
        public IRepository<SiteSetting> _Settingrepository { get; }

        public AuthService(IStringLocalizer<SharedResource> sharedLocalizer, IStringLocalizer<AuthRes> authLocalizer, IJwtService jwtService, UserManager<ApplicationUser> userManager, ISMSService sMSService,
            IUserRepository userRepository, IRepository<SiteSetting> Settingrepository, IEmailService emailService, IOptionsSnapshot<ProjectSettings> settings, RoleManager<ApplicationRole> roleManager, IRepository<AspNetUserJwtRefreshToken> jwtTableRepository , IActiveSessionService activeSessionService, IHostEnvironment environment)
        {
            _userRepository = userRepository;
            _Settingrepository = Settingrepository;
            _SharedLocalizer = sharedLocalizer;
            _authLocalizer = authLocalizer;
            _jwtService = jwtService;
            _userManager = userManager;
            _sMSService = sMSService;
            _emailService = emailService;
            _projectsetting = settings.Value;
            _roleManager = roleManager;
            _JwtTableRepository = jwtTableRepository;
            _activeSessionService = activeSessionService;
            _environment = environment;
         
        }

        public async Task<ResponseModel<ApplicationUser>> AccountVerification(AccountConfirmationDto model, CancellationToken cancellationToken)
        {
            ApplicationUser user = null;

            user = await _userRepository.Table.FirstOrDefaultAsync(a => a.UserName == model.UserName, cancellationToken: cancellationToken);

            if (user == null)
                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["UserNotFound"].Value);


            if (user.ValidationCode != null && user.ValidationCode.Equals(model.VerificationCode))
            {

                user.ValidationCode = null;
                user.PhoneNumberConfirmed = true;
                user.IsActive = true;
                user.LastLoginDate = DateTime.Now;

                await _userRepository.UpdateAsync(user, cancellationToken);

                return new ResponseModel<ApplicationUser>(true, user);

            }
            else
                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["NotValidData"].Value);

        }

        public async Task<ResponseModel> CreateRolesProject()
        {
            var roles = default(UsersRole).ToEnumModel(showname: false);

            foreach (var role in roles)
            {
                ApplicationRole applicationRole = new ApplicationRole()
                {
                    Name = role.Title,
                };

                await _roleManager.CreateAsync(applicationRole);
            }
            try
            {
                ///admin
                ApplicationUser admin = new ApplicationUser()
                {
                    UserName = "09111275582",
                    PhoneNumber = "09111275582",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    UserRole = UsersRole.admin,
                    IsRegisterComplete = true,
                };
                var admin1 = _userManager.CreateAsync(admin, "!@#QWE123qwe").Result;
                await _userManager.AddToRoleAsync(admin, "admin");
 

                ///
                return new ResponseModel(true);
            }
            catch (Exception)
            {
                return new ResponseModel(false);
            }
        }

        public async Task<ResponseModel<AccessToken>> RefreshToken(RefreshTokenDto model, CancellationToken cancellationToken)
        {
            var FindedRefresh = await _JwtTableRepository.Table.FirstOrDefaultAsync(m => m.JwtRefreshToken == model.RefreshToken);
            if (FindedRefresh == null)
                return new ResponseModel<AccessToken>(false, null);

            var FindedUser = await _userManager.FindByIdAsync(FindedRefresh.UserId.ToString());
            if (FindedUser == null)
                return new ResponseModel<AccessToken>(false, null);

            if (FindedRefresh.SecurityStamp != FindedUser.SecurityStamp)
            {
                await _JwtTableRepository.DeleteRangeAsync(_JwtTableRepository.Table.Where(m => m.UserId == FindedRefresh.UserId), cancellationToken);
                return new ResponseModel<AccessToken>(false, null);
            }

            Guid guid = Guid.NewGuid();
            FindedRefresh.JwtRefreshToken = guid;
            FindedRefresh.CreateDate = DateTime.Now;
            await _JwtTableRepository.UpdateAsync(FindedRefresh, cancellationToken);

            var jwt = await _jwtService.GenerateAsync(FindedUser);
            jwt.refresh_token = guid.ToString();

            return new ResponseModel<AccessToken>(true, jwt);

        }

        public async Task<ResponseModel> Register(SharedModels.Dtos.AuthDto model, CancellationToken cancellationToken)
        {

            //Check if email authentication is enabled and phone number authentication is disabled
            if (_projectsetting.ProjectSetting.IsEmailAuthEnable == true && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == false)
            {
                //Check if the email SMTP URL, port, username, and password are all set
                //if (_GlobalSetting?.EmailSMTPUrl == null || _GlobalSetting.EmailSMTPPort == 0 || _GlobalSetting.EmailUsername == null || _GlobalSetting.EmailPassword == null)
                //{
                //    //If not, return an error message
                //    return new ResponseModel(false, _authLocalizer["UnableSendMail"].Value);

                //}
            }

            //Check if email authentication is disabled and phone number authentication is enabled
            if (_projectsetting.ProjectSetting.IsEmailAuthEnable == false && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == true)
            {
                //Check if SMS credit is 0
                //if (_GlobalSetting?.SMSCredit == 0)
                //{
                //    //Return an error message
                //    return new ResponseModel(false, _authLocalizer["UnableSendSMS"].Value);

                //}
            }


            //Check if email authentication and phone number authentication are both disabled
            if (_projectsetting.ProjectSetting.IsEmailAuthEnable == false && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == false)
            {
                //If both are disabled, return a bad request
                return new ResponseModel(false, _authLocalizer["InfraProblem"].Value);

            }




            //Check if the username contains an "@" symbol and if email authentication is disabled
            if (model.UserName.Contains("@") && _projectsetting.ProjectSetting.IsEmailAuthEnable == false)
            {
                //Return a bad request if the username contains an "@" symbol and email authentication is disabled
                return new ResponseModel(false, _authLocalizer["InCorrectEmail"].Value);

            }



            //Check if the username length is 11 characters and starts with 09
            if (model.UserName.Length == 11 && model.UserName.StartsWith("09"))
            {
                //Check if the project setting is set to enable phone number authentication
                if (_projectsetting.ProjectSetting.IsPhonenumberAuthEnable == false)
                {
                    //Return an error message if the phone number authentication is not enabled

                    return new ResponseModel(false, _authLocalizer["InCorrectPhonenumber"].Value);

                }
            }




            ApplicationUser user = null;
            user = await _userRepository.Table.FirstOrDefaultAsync(a => a.UserName == model.UserName, cancellationToken: cancellationToken);
            //Generate Code
            string code = GenerateCode.AuthenticationCode(model.UserName, 4);

            //Create User if null
            if (user == null)
            {
                // Register user with email
                if (model.UserName.Contains('@'))
                {
                    user = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        Email = model.UserName,
                        EmailConfirmed = false,
                    };
                }
                //Register user with phonenumber
                else
                {
                    user = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        PhoneNumber = model.UserName,

                        PhoneNumberConfirmed = false,
                    };
                }
                user.UserRole = UsersRole.user;
                try
                {
                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, UsersRole.user.ToString());
                }
                catch
                {

                    throw;
                }

            }
            if (user.IsRegisterComplete == true)
            {
                return new ResponseModel(false, _authLocalizer["AlreadyRegister"].Value);

            }





            //Check if the user has sent a validation code before
            if (user.LastSendValidationCode != null)
            {
                //Check if 90 seconds have passed since the last validation code was sent
                //and if the site is not in debug mode
                if (user.LastSendValidationCode.Value.AddSeconds(90) > DateTime.Now)
                    return new ResponseModel(false, _authLocalizer["EarlyOTPRequest"].Value);



            }
            user.ValidationCode = code;
            user.LastSendValidationCode = DateTime.Now;
            await _userManager.UpdateAsync(user);
            // code = _projectsetting.AppNameForAutoFillSms + "\nکد تایید شما " + code + " میباشد \n" + model.Token;
            if (!model.UserName.Contains("0999"))
            {
                if (model.UserName.Contains('@') && _projectsetting.ProjectSetting.IsEmailAuthEnable == true)
                {
                    var body = EmailUtility.GetTemplate("SendCode.html");
                    body = body.Replace("{{code}}", code);
                    body = body.Replace("{{site-name}}", _Settingrepository.TableNoTracking.FirstOrDefault()?.SiteTitle);
                    body = body.Replace("{{contactus-url}}", _projectsetting.ProjectSetting.BaseUrl + "/contact");
                    List<string> To = new List<string>();
                    To.Add(model.UserName);
                    //await _emailService.SendEmailAsync(new MailData(To, _authLocalizer["AccountActivation"].Value, body, _GlobalSetting?.EmailUsername, _Settingrepository.TableNoTracking.FirstOrDefault()?.SiteTitle, true, _GlobalSetting?.EmailSMTPUrl, _GlobalSetting?.EmailSMTPPort, _GlobalSetting?.EmailUsername, _GlobalSetting?.EmailPassword, false), cancellationToken);


                }
                if (model.UserName.Length == 11 && model.UserName.StartsWith("09") && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == true)
                {
                    if (!_environment.IsDevelopment())
                    {
                        await _sMSService.SendSMSAsync("3209c1dc-1cae-4823-b24b-7c41fc470019", "https://localhost:7279", model.UserName, code);
                    }
                }
            }

            return new ResponseModel(true);
        }

        public async Task<ResponseModel> SendCode(SharedModels.Dtos.AuthDto model, CancellationToken cancellationToken)
        {
            //Check if email authentication is enabled and phone number authentication is disabled
            if (_projectsetting.ProjectSetting.IsEmailAuthEnable == true && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == false)
            {
                //Check if the email SMTP URL, port, username, and password are all set
                //if (_GlobalSetting?.EmailSMTPUrl == null || _GlobalSetting.EmailSMTPPort == 0 || _GlobalSetting.EmailUsername == null || _GlobalSetting.EmailPassword == null)
                //{
                //    //If not, return an error message
                //    return new ResponseModel(false, _authLocalizer["UnableSendMail"].Value);

                //}
            }

            //Check if email authentication is disabled and phone number authentication is enabled
            if (_projectsetting.ProjectSetting.IsEmailAuthEnable == false && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == true)
            {
                //Check if SMS credit is 0
                //if (_GlobalSetting?.SMSCredit == 0)
                //{
                //    //Return an error message
                //    return new ResponseModel(false, _authLocalizer["UnableSendSMS"].Value);

                //}
            }


            //Check if email authentication and phone number authentication are both disabled
            if (_projectsetting.ProjectSetting.IsEmailAuthEnable == false && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == false)
            {
                //If both are disabled, return a bad request
                return new ResponseModel(false, _authLocalizer["InfraProblem"].Value);

            }




            //Check if the username contains an "@" symbol and if email authentication is disabled
            if (model.UserName.Contains("@") && _projectsetting.ProjectSetting.IsEmailAuthEnable == false)
            {
                //Return a bad request if the username contains an "@" symbol and email authentication is disabled
                return new ResponseModel(false, _authLocalizer["InCorrectEmail"].Value);

            }



            //Check if the username length is 11 characters and starts with 09
            if (model.UserName.Length == 11 && model.UserName.StartsWith("09"))
            {
                //Check if the project setting is set to enable phone number authentication
                if (_projectsetting.ProjectSetting.IsPhonenumberAuthEnable == false)
                {
                    //Return an error message if the phone number authentication is not enabled
                    return new ResponseModel(false, _authLocalizer["InCorrectPhonenumber"].Value);

                }
            }




            ApplicationUser user = null;
            user = await _userRepository.Table.FirstOrDefaultAsync(a => a.UserName == model.UserName, cancellationToken: cancellationToken);
            //Generate Code
            string code = GenerateCode.AuthenticationCode(model.UserName, 4);

         
                //Create User if null
                if (user == null)
                {


                return new ResponseModel(false, "کاربری یافت نشد");

            }
                else
                {

                    //Check if the user is active
                    if (user.IsActive == false)
                    {
                        return new ResponseModel(false, _authLocalizer["InActiveAccount"].Value);
                    }


                }
          






            //Check if the user has sent a validation code before
            if (user.LastSendValidationCode != null)
            {
                //Check if 90 seconds have passed since the last validation code was sent
                //and if the site is not in debug mode
                if (user.LastSendValidationCode.Value.AddSeconds(90) > DateTime.Now)
                    return new ResponseModel(false, _authLocalizer["EarlyOTPRequest"].Value);
            }

            if (!_environment.IsDevelopment())
            {
                user.ValidationCode = code;
            }
            else
            {
                user.ValidationCode = "1234";
            }

            
            user.LastSendValidationCode = DateTime.Now;
            await _userManager.UpdateAsync(user);
            // code = _projectsetting.AppNameForAutoFillSms + "\nکد تایید شما " + code + " میباشد \n" + model.Token;
            if (!model.UserName.Contains("0999"))
            {
                if (model.UserName.Contains('@') && _projectsetting.ProjectSetting.IsEmailAuthEnable == true)
                {
                    var body = EmailUtility.GetTemplate("SendCode.html");
                    body = body.Replace("{{code}}", code);
                    body = body.Replace("{{site-name}}", _Settingrepository.TableNoTracking.FirstOrDefault()?.SiteTitle);
                    body = body.Replace("{{contactus-url}}", _projectsetting.ProjectSetting.BaseUrl + "/contact");
                    List<string> To = new List<string>();
                    To.Add(model.UserName);
                    //await _emailService.SendEmailAsync(new MailData(To, _authLocalizer["AccountActivation"].Value, body, _GlobalSetting?.EmailUsername, _Settingrepository.TableNoTracking.FirstOrDefault()?.SiteTitle, true, _GlobalSetting?.EmailSMTPUrl, _GlobalSetting?.EmailSMTPPort, _GlobalSetting?.EmailUsername, _GlobalSetting?.EmailPassword, false), cancellationToken);


                }
                if (model.UserName.Length == 11 && model.UserName.StartsWith("09") && _projectsetting.ProjectSetting.IsPhonenumberAuthEnable == true)
                {
                    if (!_environment.IsDevelopment())
                    {
                        await _sMSService.SendSMSAsync("3209c1dc-1cae-4823-b24b-7c41fc470019", "https://localhost:7279", model.UserName, code);
                    }
                }
            }

            return new ResponseModel(true);

        }

        public async Task<ResponseModel> SetPassword(SetPassword model, int UserId, CancellationToken cancellationToken)
        {

            var findUser = await _userManager.FindByIdAsync(UserId.ToString());
            if (findUser == null)
                return new ResponseModel(false);
            else
            {
                try
                {

                    // Remove the user's password.
                    await _userManager.RemovePasswordAsync(findUser);

                    await _userManager.AddPasswordAsync(findUser, model.Password);
                    return new ResponseModel(true);
                }
                catch (Exception)
                {

                    return new ResponseModel(false);
                }

            }
        }

        public async Task<ResponseModel<ApplicationUser>> Login(TokenRequest tokenRequest, CancellationToken cancellationToken)
        {

            var user = await _userManager.FindByNameAsync(tokenRequest.username);

            if (user == null)

                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["UserNotFound"].Value);

            if (tokenRequest.UserRole == null)
                tokenRequest.UserRole = user.UserRole;

            if (user.IsActive == false)
                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["InActiveAccount"].Value);


            if (tokenRequest.UserRole != user.UserRole)
                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["DenyForRole"].Value);


            var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.password);
            if (!isPasswordValid)
                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["InCorrectUserOrPass"].Value);


            if (user.PhoneNumberConfirmed == false && user.EmailConfirmed == false)
                return new ResponseModel<ApplicationUser>(false, null, _authLocalizer["NotActiveAccount"].Value);

            user.LastLoginDate = DateTime.Now;
            await _userRepository.UpdateAsync(user, cancellationToken);

            return new ResponseModel<ApplicationUser>(true, user);

        }

        public async Task<ResponseModel> CompleteRegister(UserExtraData model, int UserId, CancellationToken cancellationToken)
        {
            ApplicationUser user = null;

            user = await _userRepository.Table.FirstOrDefaultAsync(a => a.Id == UserId, cancellationToken: cancellationToken);
            if (user == null)
                return new ResponseModel(false, _authLocalizer["UserNotFound"].Value);

            try
            {
                user.IsRegisterComplete = true;
                await _userRepository.UpdateAsync(user, cancellationToken);

  

                return new ResponseModel(true);
            }
            catch (Exception E)
            {

                return new ResponseModel(false, E.InnerException.Message);
            }

        }
    }
}
