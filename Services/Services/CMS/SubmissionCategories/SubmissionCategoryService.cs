using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.SubmissionCategories
{
    public class SubmissionCategoryService : IScopedDependency, ISubmissionCategoryService
    {
        private readonly IRepository<SubmissionCategory> _categoryRepository;
        private readonly IMapper _mapper;

        public SubmissionCategoryService(IRepository<SubmissionCategory> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<ResponseModel<List<SubmissionCategoryDto>>> GetListAsync(PageListModel model, CancellationToken cancellationToken)
        {
            model ??= new PageListModel();
            if (model.arg.PageSize <= 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber <= 0)
                model.arg.PageNumber = 1;

            var query = _categoryRepository.TableNoTracking
                .OrderByDescending(c => c.CreatedAt);

            var categories = await query
                .Paginate(model.arg)
                .ProjectTo<SubmissionCategoryDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new ResponseModel<List<SubmissionCategoryDto>>(true, categories);
        }

        public async Task<ResponseModel<SubmissionCategoryDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.TableNoTracking
                .Where(c => c.Id == id)
                .ProjectTo<SubmissionCategoryDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (category == null)
                return new ResponseModel<SubmissionCategoryDto>(false, null, "دسته‌بندی یافت نشد");

            return new ResponseModel<SubmissionCategoryDto>(true, category);
        }

        public async Task<ResponseModel<int>> CreateAsync(SubmissionCategoryCreateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return new ResponseModel<int>(false, 0, "اطلاعات ورودی نامعتبر است");

            var entity = new SubmissionCategory
            {
                Title = model.Title?.Trim(),
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            await _categoryRepository.AddAsync(entity, cancellationToken);

            return new ResponseModel<int>(true, entity.Id, "دسته‌بندی با موفقیت ایجاد شد");
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, SubmissionCategoryUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null || id <= 0)
                return new ResponseModel<bool>(false, false, "اطلاعات ورودی نامعتبر است");

            var category = await _categoryRepository.Table
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (category == null)
                return new ResponseModel<bool>(false, false, "دسته‌بندی یافت نشد");

            category.Title = model.Title?.Trim();
            category.UpdatedAt = DateTime.Now;

            await _categoryRepository.UpdateAsync(category, cancellationToken);

            return new ResponseModel<bool>(true, true, "دسته‌بندی با موفقیت ویرایش شد");
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return new ResponseModel<bool>(false, false, "شناسه نامعتبر است");

            var category = await _categoryRepository.Table
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (category == null)
                return new ResponseModel<bool>(false, false, "دسته‌بندی یافت نشد");

            await _categoryRepository.DeleteAsync(category, cancellationToken);

            return new ResponseModel<bool>(true, true, "دسته‌بندی حذف شد");
        }
    }
}
