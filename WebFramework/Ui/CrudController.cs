using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Enums;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.Api;
using SharedModels.Api;

namespace WebFramework.Ui
{
    public class CrudController<TDto, TSelectDto, TEntity, TKey> : BaseController
         where TDto : BaseDto<TDto, TEntity, TKey>, new()
         where TSelectDto : BaseDto<TSelectDto, TEntity, TKey>, new()
         where TEntity : BaseEntity<TKey>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public CrudController(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public virtual IActionResult Create()
        {
            ViewBag.PageType = PageType.add;
            return View("upsert", new TDto());
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit(int Id, CancellationToken cancellationToken)
        {
            ViewBag.PageType = PageType.edit;
            var resultDto = await Repository.TableNoTracking.ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(Id), cancellationToken);
            return View("upsert", resultDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> CrudEdit(TDto dto, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (dto.Id.ToString() == "0")
                {
                    ViewBag.PageType = PageType.add;
                    var model = dto.ToEntity(Mapper);
                    await Repository.AddAsync(model, cancellationToken);

                    TempData["message"] = new string[] { $"ذخیره شد", "sussess" };
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.PageType = PageType.edit;
                    var model = await Repository.GetByIdAsync(cancellationToken, dto.Id);

                    model = dto.ToEntity(Mapper, model);

                    await Repository.UpdateAsync(model, cancellationToken);

                    TempData["message"] = new string[] { "ویرایش شد", "warning" };
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.PageType = PageType.add;
                return View("upsert", dto);
            }
        }

        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromQuery] TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var res = await Repository.Table.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
                if (res != null)
                    await Repository.DeleteAsync(res, cancellationToken);
            }
            catch
            {
                throw;
            }
            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteRange([FromQuery] TKey[] ids, CancellationToken cancellationToken)
        {
            try
            {
                var entities = Repository.Table.Where(_ => ids.Contains(_.Id));
                await Repository.DeleteRangeAsync(entities, cancellationToken);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class CrudController<TDto, TSelectDto, TEntity> : CrudController<TDto, TSelectDto, TEntity, int>
        where TDto : BaseDto<TDto, TEntity, int>, new()
        where TSelectDto : BaseDto<TSelectDto, TEntity, int>, new()
        where TEntity : BaseEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }

    public class CrudController<TDto, TEntity> : CrudController<TDto, TDto, TEntity, int>
        where TDto : BaseDto<TDto, TEntity, int>, new()
        where TEntity : BaseEntity<int>, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }

    public class CrudWithSeoController<TDto, TSelectDto, TEntity, TKey> : BaseController
     where TDto : BaseWithSeoDto<TDto, TEntity, TKey>, new()
     where TSelectDto : BaseWithSeoDto<TSelectDto, TEntity, TKey>, new()
     where TEntity : BaseWithSeoEntity<TKey>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public CrudWithSeoController(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public virtual IActionResult Create()
        {
            ViewBag.PageType = PageType.add;
            return View("upsert", new TDto());
        }

        [HttpGet]
        public virtual async Task<IActionResult> Edit(int Id, CancellationToken cancellationToken)
        {
            ViewBag.PageType = PageType.edit;
            var resultDto = await Repository.TableNoTracking.ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(Id), cancellationToken);
            return View("upsert", resultDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> CrudEdit(TDto dto, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (dto.Id.ToString() == "0")
                {
                    ViewBag.PageType = PageType.add;
                    var model = dto.ToEntity(Mapper);

                    await Repository.AddAsync(model, cancellationToken);
                    TempData["message"] = new string[] { $"ذخیره شد", "sussess" };
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.PageType = PageType.edit;
                    var model = await Repository.GetByIdAsync(cancellationToken, dto.Id);

                    model = dto.ToEntity(Mapper, model);

                    await Repository.UpdateAsync(model, cancellationToken);

                    TempData["message"] = new string[] { "ویرایش شد", "warning" };
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ViewBag.PageType = PageType.add;
                return View("upsert", dto);
            }
        }


        [HttpDelete]
        public virtual async Task<IActionResult> Delete([FromQuery] TKey id, CancellationToken cancellationToken)
        {
            try
            {
                var res = await Repository.Table.SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
                if (res != null)
                {
                    await Repository.DeleteAsync(res, cancellationToken);
                }
            }
            catch
            {
                throw;
            }
            return Ok();
        }
    }

    public class CrudWithSeoController<TDto, TSelectDto, TEntity> : CrudWithSeoController<TDto, TSelectDto, TEntity, int>
        where TDto : BaseWithSeoDto<TDto, TEntity, int>, new()
        where TSelectDto : BaseWithSeoDto<TSelectDto, TEntity, int>, new()
        where TEntity : BaseWithSeoEntity<int>, new()
    {
        public CrudWithSeoController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }

    public class CrudWithSeoController<TDto, TEntity> : CrudWithSeoController<TDto, TDto, TEntity, int>
        where TDto : BaseWithSeoDto<TDto, TEntity, int>, new()
        where TEntity : BaseWithSeoEntity<int>, new()
    {
        public CrudWithSeoController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}