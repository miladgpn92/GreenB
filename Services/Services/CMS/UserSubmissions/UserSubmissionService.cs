using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Services;
using Services.Services.CMS.Setting;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.UserSubmissions
{
    public class UserSubmissionService : IScopedDependency, IUserSubmissionService
    {
        private readonly IRepository<UserSubmission> _userSubmissionRepository;
        private readonly IMapper _mapper;
        private readonly ISMSService _smsService;
        private readonly ISettingService _settingService;
        private readonly ProjectSettings _projectSettings;

        public UserSubmissionService(
            IRepository<UserSubmission> userSubmissionRepository,
            IMapper mapper,
            ISMSService smsService,
            ISettingService settingService,
            IOptionsSnapshot<ProjectSettings> projectSettings)
        {
            _userSubmissionRepository = userSubmissionRepository;
            _mapper = mapper;
            _smsService = smsService;
            _settingService = settingService;
            _projectSettings = projectSettings.Value;
        }

        public async Task<ResponseModel<List<UserSubmissionDto>>> GetListAsync(PageListModel model, int? categoryId, string phone, CancellationToken cancellationToken)
        {
            model ??= new PageListModel();
            if (model.arg.PageSize <= 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber <= 0)
                model.arg.PageNumber = 1;

            var query = _userSubmissionRepository.TableNoTracking
                .Include(u => u.SubmissionCategory)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(u => u.SubmissionCategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var phoneTerm = phone.Trim();
                query = query.Where(u => u.Phone.Contains(phoneTerm));
            }

            query = query.OrderByDescending(u => u.CreatedAt);

            var result = await query
                .Paginate(model.arg)
                .ProjectTo<UserSubmissionDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new ResponseModel<List<UserSubmissionDto>>(true, result);
        }

        public async Task<ResponseModel<UserSubmissionDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var item = await _userSubmissionRepository.TableNoTracking
                .Where(u => u.Id == id)
                .ProjectTo<UserSubmissionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return new ResponseModel<UserSubmissionDto>(false, null, "درخواست مورد نظر یافت نشد");

            return new ResponseModel<UserSubmissionDto>(true, item);
        }

        public async Task<ResponseModel<int>> CreateAsync(UserSubmissionCreateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return new ResponseModel<int>(false, 0, "اطلاعات ورودی نامعتبر است");

            var entity = new UserSubmission
            {
                Phone = model.Phone?.Trim(),
                FirstName = model.FirstName?.Trim(),
                LastName = model.LastName?.Trim(),
                SubmissionCategoryId = model.SubmissionCategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            await _userSubmissionRepository.AddAsync(entity, cancellationToken);

            var smsMessage = _settingService.GetSetting()?.Model?.SMSText;
            var smsConfig = _projectSettings?.ProjectSetting;
            if (!string.IsNullOrWhiteSpace(smsMessage) &&
                !string.IsNullOrWhiteSpace(entity.Phone) &&
                smsConfig != null)
            {
                var smsResult = await _smsService.SendSMSAsync(
                    smsConfig.SMSToken,
                    smsConfig.BaseUrl,
                    entity.Phone,
                    smsMessage);

                if (!smsResult.IsSuccess)
                    return new ResponseModel<int>(true, entity.Id, "ثبت درخواست انجام شد اما ارسال پیامک با خطا مواجه شد.");
            }

            return new ResponseModel<int>(true, entity.Id, "درخواست با موفقیت ثبت شد");
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return new ResponseModel<bool>(false, false, "شناسه نامعتبر است");

            var entity = await _userSubmissionRepository.Table
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            if (entity == null)
                return new ResponseModel<bool>(false, false, "درخواست مورد نظر یافت نشد");

            await _userSubmissionRepository.DeleteAsync(entity, cancellationToken);

            return new ResponseModel<bool>(true, true, "درخواست حذف شد");
        }
    }
}
