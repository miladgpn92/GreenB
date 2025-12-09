using Common;
using Common.Consts;
using Common.Enums;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ResourceLibrary;
using ResourceLibrary.Resources.Auth;
using Services;
using Services.Services;
using Services.Services.CMS.ActiveSession;
using Services.Services.CMS.Auth;
using Services.Services.Email;
using Shared.Api;
using SharedModels;
using SharedModels.Api;
using SharedModels.Dtos;

namespace Web.Api
{

    [ApiVersion("1")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IActiveSessionService _activeSessionService;
        private readonly IJwtService _jwtService;
        public AuthController(IAuthService authService, IJwtService jwtService, IActiveSessionService activeSessionService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _activeSessionService = activeSessionService;

        }



        /// <summary>
        /// Creates roles and adds an admin user to the role.
        /// </summary>
        /// <returns>
        /// Returns an OK response if successful.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("[action]")]
        public async Task<ActionResult> CreateRolesProject(CancellationToken cancellationToken)
        {

            var res = await _authService.CreateRolesProject();
            if (res.IsSuccess)
                return Ok(res.Description);
            else
                return BadRequest(res.Description);
        }




        /// <summary>
        /// Generates a JWT token for the user based on the provided username and password.
        /// </summary>
        /// <param name="tokenRequest">The token request containing the username and password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A JsonResult containing the generated JWT token.
        /// </returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Login([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {

            try
            {
                var res = await _authService.Login(tokenRequest, cancellationToken);

                if (res.IsSuccess)
                {
                    var jwt = await _jwtService.GenerateAsync(res.Model);

                    //This code adds an active session for the user with the given identity and the current HTTP context.
                    _activeSessionService.AddActiveSession(res.Model.Id, HttpContext, jwt.access_token);
                    return new JsonResult(jwt);
                }
                else
                {
                    return BadRequest(res.Message);
                }

            }
            catch
            {
                return BadRequest();
            }
        }



        /// <summary>
        /// Verifies the account with the given username and verification code.
        /// </summary>
        /// <param name="model">The account confirmation data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The JWT token if the account is verified.</returns>
        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> AccountVerification([FromBody] AccountConfirmationDto model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var res = await _authService.AccountVerification(model, cancellationToken);
                if (res.IsSuccess)
                {
                    var jwt = await _jwtService.GenerateAsync(res.Model);

                    //This code adds an active session for the user with the given identity and the current HTTP context.
                    _activeSessionService.AddActiveSession(res.Model.Id, HttpContext, jwt.access_token);
                    return new JsonResult(jwt);
                }
                else
                {
                    return BadRequest(res.Message);
                }
            }
            return BadRequest();
        }


        /// <summary>
        /// Sends a validation code to the user. it use for Register and OTP Authentication
        /// </summary>
        /// <param name="model">The authentication data transfer object.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult> SendCode([FromForm] SharedModels.Dtos.AuthDto model, CancellationToken cancellationToken)
        {

            var res=await _authService.SendCode(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Description);
            else
                return BadRequest(res.Description);

        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult> Register([FromForm] SharedModels.Dtos.AuthDto model, CancellationToken cancellationToken)
        {

            var res= await _authService.Register(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Description);
            else
                return BadRequest(res.Description);
        }

        /// <summary>
        /// Refreshes the token.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The refreshed token.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenDto model, CancellationToken cancellationToken)
        {

            var res = await _authService.RefreshToken(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }





        /// <summary>
        /// Sets the password for the specified user.
        /// </summary>
        /// <param name="model">The model containing the new password.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [Authorize(Roles = "admin,user")]
        [HttpPost("[action]")]
        public async Task<ActionResult> SetPassword([FromBody] SetPassword model, CancellationToken cancellationToken)
        {
            var user = User.Identity.GetUserIdInt();
            var res=await _authService.SetPassword(model, user, cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Description);
            else
                return BadRequest(res.Description);
        }


        /// <summary>
        /// Checks if the user is authenticated and returns an OK result.
        /// </summary>
        /// <returns>OK result if the user is authenticated.</returns>
        [Authorize]
        [HttpGet("[action]")]
        public ActionResult IsAuthenticated()
        {
            return Ok();
        }

    }
}
