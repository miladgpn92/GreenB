using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Extensions;
using Data.Repositories;
using DariaCMS.Common;
using Entities;
using Microsoft.EntityFrameworkCore;
using Services.Services.CMS;
using SharedModels;
using SharedModels.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS.BrochureFiles
{
    public class BrochureFileService : IScopedDependency, IBrochureFileService
    {
        private readonly IRepository<BrochureFile> _brochureRepository;
        private readonly IMapper _mapper;
        private readonly ISlugService<BrochureFile> _slugService;

        public BrochureFileService(IRepository<BrochureFile> brochureRepository, IMapper mapper, ISlugService<BrochureFile> slugService)
        {
            _brochureRepository = brochureRepository;
            _mapper = mapper;
            _slugService = slugService;
        }

        public async Task<ResponseModel<List<BrochureFileDto>>> GetListAsync(PageListModel model, CancellationToken cancellationToken)
        {
            model ??= new PageListModel();
            if (model.arg.PageSize <= 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber <= 0)
                model.arg.PageNumber = 1;

            var query = _brochureRepository.TableNoTracking
                .OrderByDescending(b => b.CreatedAt);

            var brochures = await query
                .Paginate(model.arg)
                .ProjectTo<BrochureFileDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return new ResponseModel<List<BrochureFileDto>>(true, brochures);
        }

        public async Task<ResponseModel<BrochureFileDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var brochure = await _brochureRepository.TableNoTracking
                .Where(b => b.Id == id)
                .ProjectTo<BrochureFileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (brochure == null)
                return new ResponseModel<BrochureFileDto>(false, null, "فایل بروشور یافت نشد");

            return new ResponseModel<BrochureFileDto>(true, brochure);
        }

        public async Task<ResponseModel<BrochureFileDto>> GetBySlugAsync(string slug, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return new ResponseModel<BrochureFileDto>(false, null, "اسلاگ نامعتبر است");

            var brochure = await _brochureRepository.TableNoTracking
                .Where(b => b.Slug == slug.Trim())
                .ProjectTo<BrochureFileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (brochure == null)
                return new ResponseModel<BrochureFileDto>(false, null, "فایل بروشور یافت نشد");

            return new ResponseModel<BrochureFileDto>(true, brochure);
        }

        public async Task<ResponseModel<int>> CreateAsync(BrochureFileCreateDto model, CancellationToken cancellationToken)
        {
            if (model == null)
                return new ResponseModel<int>(false, 0, "اطلاعات ورودی نامعتبر است");

            var baseSlug = string.IsNullOrWhiteSpace(model.Slug)
                ? model.Title?.ToSlug()
                : model.Slug.ToSlug();

            var uniqueSlug = _slugService.CheckSlug(baseSlug, cancellationToken);

            var entity = new BrochureFile
            {
                Title = model.Title?.Trim(),
                PdfFileUrl = model.PdfFileUrl?.Trim(),
                Slug = uniqueSlug,
                CreatedAt = DateTime.Now,
                UpdatedAt = null
            };

            await _brochureRepository.AddAsync(entity, cancellationToken);

            return new ResponseModel<int>(true, entity.Id, "فایل بروشور با موفقیت ایجاد شد");
        }

        public async Task<ResponseModel<bool>> UpdateAsync(int id, BrochureFileUpdateDto model, CancellationToken cancellationToken)
        {
            if (model == null || id <= 0)
                return new ResponseModel<bool>(false, false, "اطلاعات ورودی نامعتبر است");

            var brochure = await _brochureRepository.Table
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (brochure == null)
                return new ResponseModel<bool>(false, false, "فایل بروشور یافت نشد");

            brochure.Title = model.Title?.Trim();
            brochure.PdfFileUrl = model.PdfFileUrl?.Trim();
            var baseSlug = string.IsNullOrWhiteSpace(model.Slug)
                ? model.Title?.ToSlug()
                : model.Slug.ToSlug();

            if (!string.Equals(baseSlug, brochure.Slug, StringComparison.OrdinalIgnoreCase))
            {
                brochure.Slug = _slugService.CheckSlug(baseSlug, cancellationToken);
            }
            brochure.UpdatedAt = DateTime.Now;

            await _brochureRepository.UpdateAsync(brochure, cancellationToken);

            return new ResponseModel<bool>(true, true, "فایل بروشور با موفقیت ویرایش شد");
        }

        public async Task<ResponseModel<bool>> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return new ResponseModel<bool>(false, false, "شناسه نامعتبر است");

            var brochure = await _brochureRepository.Table
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (brochure == null)
                return new ResponseModel<bool>(false, false, "فایل بروشور یافت نشد");

            await _brochureRepository.DeleteAsync(brochure, cancellationToken);

            return new ResponseModel<bool>(true, true, "فایل بروشور حذف شد");
        }
    }
}
