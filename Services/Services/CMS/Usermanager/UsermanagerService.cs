using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using ResourceLibrary.Resources.Usermanager;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS
{
    public class UsermanagerService : IScopedDependency, IUsermanagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper mapper;


        private readonly IStringLocalizer<UsermanagerRes> _Localizer;
        public UsermanagerService(UserManager<ApplicationUser> userManager, IMapper Mapper, IStringLocalizer<UsermanagerRes> localizer)
        {
            _userManager = userManager;
            mapper = Mapper;
            _Localizer = localizer;


        }



        public async Task<ResponseModel> CreateAsync(UserDto user)
        {
            ApplicationUser currentuser = null;
            if(user.PhoneNumber != null) {
                currentuser = await _userManager.Users.Where(a =>a.PhoneNumber == user.PhoneNumber).FirstOrDefaultAsync();
            }else if(user.Email != null)
            {
               var  EmailUser = await _userManager.Users.Where(a => a.Email == user.Email).FirstOrDefaultAsync();
                if (EmailUser != null)
                    currentuser = null;
            }
           
            if (currentuser != null)
            {
                var msg = _Localizer["UserExist"];

                return new ResponseModel(false, msg.Value);
            }
            else
            {

                if(user.Password == null)
                {
                    user.Password = user.PhoneNumber.Substring(user.PhoneNumber.Length - 6);
                }
                var userres = mapper.Map<ApplicationUser>(user);
                userres.IsActive = true;
                if (userres != null)
                {
                    if (userres.PhoneNumber != null)
                    {
                        userres.PhoneNumberConfirmed = true;
                        userres.UserName = userres.PhoneNumber;
                    }
                    else if (userres.Email != null)
                    {
                        userres.EmailConfirmed = true;
                        userres.Email = userres.Email;
                    }

                }
                userres.IsRegisterComplete = true;  
                var state = await _userManager.CreateAsync(userres);
                if (state.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userres, user.UserRole.ToString());
                    if (user.Password != null)
                    {
                     var resAddPass=  await _userManager.AddPasswordAsync(userres, user.Password);
                    }
                    return new ResponseModel(true, "");
                }
                else
                {
                    var msg = _Localizer["UserExist"];

                    return new ResponseModel(false, msg.Value);
                }
            }

        }

        public async Task<ResponseModel> DeleteAsync(List<int> ids)
        {

            try
            {
                foreach (var id in ids)
                {
                    var user = await _userManager.Users.Where(a => a.Id == id).FirstOrDefaultAsync();
                    if (user != null)

                    {
                        await _userManager.DeleteAsync(user);

                    }
                }
                return new ResponseModel(true, "");

            }
            catch 
            {

                return new ResponseModel(false, null);
            }

        }

        public async Task<ResponseModel<List<UserSelectDto>>> GetListAsync(PageListModel model, CancellationToken cancellationToken)
        {

            if (model.arg.PageSize > 100)
                model.arg.PageSize = 100;
            if (model.arg.PageSize == 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber == 0)
                model.arg.PageNumber = 1;
            var list = _userManager.Users;


            if (model.filters != null && model.filters.Count > 0)
            {

                try
                {
                    var filters = new List<FilterCriteria>();
                    foreach (var item in model.filters)
                    {
                        filters.Add(new FilterCriteria
                        {
                            Field = item.Field,
                            Value = item.Value,
                            Operator = item.Operator,
                        });
                    }

                    list = list.DynamicFilter(model.filters);
                }
                catch (System.Exception)
                {

                    return new ResponseModel<List<UserSelectDto>>(false, null, "Something went wrong in Filter");
                }



            }
            if (!string.IsNullOrEmpty(model.sortField))
            {

                try
                {
                    list = SearchUtility.SortData(list, model.sortField, model.ascending);
                }
                catch (System.Exception)
                {
                    return new ResponseModel<List<UserSelectDto>>(false, null, "Something went wrong in Sorting");
                }

            }
            else
            {
                list = list.OrderByDescending(a => a.Id);
            }
            var res = await list.Paginate(model.arg).ProjectTo<UserSelectDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
            return new ResponseModel<List<UserSelectDto>>(true, res);

        }

        public async Task<ResponseModel<UserSelectDto>> GetByIdAsync(int id)
        {
            var user = await _userManager.Users.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                var msg = _Localizer["UserNotFound"];
                return new ResponseModel<UserSelectDto>(false, null, msg);
            }
            else
            {
                var userres = mapper.Map<UserSelectDto>(user);

                return new ResponseModel<UserSelectDto>(true, userres);
            }
        }

        public async Task<ResponseModel> UpdateAsync(int id, UserUpdateDto user)
        {
            var currentuser = await _userManager.Users.FirstOrDefaultAsync(a => a.Id == id);
            if (currentuser == null)
            {
                var msg = _Localizer["UserNotFound"];
                return new ResponseModel(false, msg.Value);
            }
            else
            {
                if (currentuser.UserRole != user.UserRole)
                {
                    // Remove the user from the current role
                    await _userManager.RemoveFromRoleAsync(currentuser, currentuser.UserRole.ToString());
                    // Add the user to the new role
                    var result = await _userManager.AddToRoleAsync(currentuser, user.UserRole.ToString());
                }


                currentuser.Name = user.Name;
                currentuser.Email = user.Email;
                currentuser.Family = user.Family;
                currentuser.PicUrl = user.PicUrl;
                currentuser.PhoneNumber = user.PhoneNumber;
                currentuser.UserRole = user.UserRole;
                currentuser.Gender = user.Gender;


                if (currentuser.PhoneNumber != null)
                {
                    currentuser.UserName = currentuser.PhoneNumber;
                }
                else if (currentuser.Email != null)
                {
                    currentuser.Email = currentuser.Email;
                }

                var state = await _userManager.UpdateAsync(currentuser);



                if (state.Succeeded)
                {


                    return new ResponseModel(true, "");
                }
                else
                {
                    return new ResponseModel(false, null);
                }
            }
        }


        public async Task<ResponseModel> ChangeUserStateAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                var msg = _Localizer["UserNotFound"];
                return new ResponseModel(false, msg.Value);
            }


            // Toggle the LockoutEnabled property
            user.LockoutEnabled = !user.LockoutEnabled;

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                // User is currently locked out, set LockoutEnd to a past date to activate
                user.LockoutEnd = DateTimeOffset.UtcNow.AddDays(-1);
            }
            else
            {
                // User is currently active, set LockoutEnd to a future date to deactivate
                user.LockoutEnd = null;
            }

            // Update the user
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return new ResponseModel(true, "");
            }
            else
            {
                // Handle error if updating user fails
                return new ResponseModel(false, result.Errors.FirstOrDefault()?.Description);
            }
        }

        public async Task<ResponseModel> ChangePassword(int id, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                var msg = _Localizer["UserNotFound"];
                return new ResponseModel(false, msg.Value);
            }


            // Change the user password
            await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.AddPasswordAsync(user, password);
            if (result.Succeeded)
            {
                return new ResponseModel(true, "");
            }
            else
            {
                // Handle error if change password fails
                return new ResponseModel(false, result.Errors.FirstOrDefault()?.Description);
            }

        }
    }
}
