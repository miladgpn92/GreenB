using AutoMapper;
using Common;
using Common.Consts;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.CMS;
using Shared;
using Shared.Api;
using SharedModels;
using SharedModels.Dtos;
using System.Data;

namespace Web.Api.Usermanager
{
    [ApiVersion("1")]
    [Authorize(Roles = RoleConsts.Admin)]
    [NonController]
    public class UsermanagerController : BaseController
    {
        private readonly IUsermanagerService _usermanagerService;

        public UsermanagerController(IUsermanagerService usermanagerService)
        {
            _usermanagerService = usermanagerService;
        }



        /// <summary>
        /// Creates a new user with the given UserDto object.
        /// </summary>
        /// <param name="dto">The UserDto object containing the user information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns an Ok result if the user was created successfully, otherwise a BadRequest result with the error description.</returns>
        [HttpPost]
        public virtual async Task<IActionResult> Create(UserDto dto, CancellationToken cancellationToken)
        {
            var res = await _usermanagerService.CreateAsync(dto);

            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }




        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="UserId">The id of the user to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpDelete]
        public virtual async Task<IActionResult> Delete(List<int> UserId, CancellationToken cancellationToken)
        {
            var res = await _usermanagerService.DeleteAsync(UserId);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }



        /// <summary>
        /// Gets a user by their Id.
        /// </summary>
        /// <param name="UserId">The Id of the user.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The user with the specified Id.</returns>
        [HttpGet("GetById")]
        public virtual async Task<IActionResult> GetById(int UserId, CancellationToken cancellationToken)
        {
            var res = await _usermanagerService.GetByIdAsync(UserId);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }



        /// <summary>
        /// Get a list of users based on the PageListModel.
        /// </summary>
        /// <param name="model">The PageListModel containing the parameters for the list.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Returns an OkResult with the list of users or a BadRequestResult with an error message.
        /// </returns>
        [HttpPost("PagedList")]
        public virtual async Task<IActionResult> List([FromBody] PageListModel model, CancellationToken cancellationToken)
        {
            var res = await _usermanagerService.GetListAsync(model, cancellationToken);
            if (res.IsSuccess)
                return Ok(res.Model);
            else
                return BadRequest(res.Message);
        }



        /// <summary>
        /// Changes the state of the user.
        /// </summary>
        /// <param name="UserId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPost("ChangeUserState")]
        public virtual async Task<IActionResult> ChangeUserState(int UserId, CancellationToken cancellationToken)
        {
            var res = await _usermanagerService.ChangeUserStateAsync(UserId, cancellationToken);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest();
        }




        /// <summary>
        /// Updates a user with the given Id using the given UserUpdateDto.
        /// </summary>
        /// <param name="Id">The Id of the user to update.</param>
        /// <param name="dto">The UserUpdateDto containing the new user information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A response indicating success or failure.</returns>
        [HttpPut]
        public virtual async Task<IActionResult> Update(int Id, UserUpdateDto dto, CancellationToken cancellationToken)
        {
            var res = await _usermanagerService.UpdateAsync(Id, dto);
            if (res.IsSuccess)
                return Ok();
            else
                return BadRequest(res.Description);
        }
    }
}
