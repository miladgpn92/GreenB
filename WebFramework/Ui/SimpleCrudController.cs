using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class SimpleCrudController<TDto, TSelectDto, TEntity, TKey> : BaseController
  where TDto : SimpleBaseDto<TDto, TEntity, TKey>, new()
  where TSelectDto : SimpleBaseDto<TSelectDto, TEntity, TKey>, new()
  where TEntity : SimpleBaseEntity<TKey>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public SimpleCrudController(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public IActionResult Create()
        {
            return View("Edit", new TDto());
        }

        public async Task<IActionResult> Edit(int Id, CancellationToken cancellationToken)
        {
            var res = Mapper.Map<TDto>(await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(Id), cancellationToken));
            return View("Edit", res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrudEdit(TDto dto, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (dto.Id.ToString() == "0")
                {
                    var model = dto.ToEntity(Mapper);

                    await Repository.AddAsync(model, cancellationToken);

                    var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

                    TempData["message"] = new string[] { $"ذخیره شد", "sussess" };
                    return RedirectToAction("Index");
                }
                else
                {
                    var model = await Repository.GetByIdAsync(cancellationToken, dto.Id);

                    model = dto.ToEntity(Mapper, model);

                    await Repository.UpdateAsync(model, cancellationToken);

                    TempData["message"] = new string[] { "ویرایش شد", "warning" };
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View("Edit", dto);
            }
        }

       
    }

    public class SimpleCrudController<TDto, TSelectDto, TEntity> : SimpleCrudController<TDto, TSelectDto, TEntity, int>
        where TDto : SimpleBaseDto<TDto, TEntity, int>, new()
        where TSelectDto : SimpleBaseDto<TSelectDto, TEntity, int>, new()
        where TEntity : SimpleBaseEntity<int>, new()
    {
        public SimpleCrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }

    public class SimpleCrudController<TDto, TEntity> : SimpleCrudController<TDto, TDto, TEntity, int>
        where TDto : SimpleBaseDto<TDto, TEntity, int>, new()
        where TEntity : SimpleBaseEntity<int>, new()
    {
        public SimpleCrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }
    }
}
