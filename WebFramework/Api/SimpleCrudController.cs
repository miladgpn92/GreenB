using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Enums;
using Common.Utilities;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Api
{
    [ApiVersion("1")]
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




        [HttpGet("PagedList")]
        public virtual async Task<ActionResult<List<TSelectDto>>> List([FromQuery] Pageres arg ,[FromQuery] string query, [FromQuery] List<string> searchFields, CancellationToken cancellationToken)
        {
            if (arg.PageSize > 100)
                arg.PageSize = 100;
            if (arg.PageSize == 0)
                arg.PageSize = 10;
            if (arg.PageNumber == 0)
                arg.PageNumber = 1;
            var list = Repository.TableNoTracking;

          


            var res = await list.OrderBy(a => a.Id).Paginate(arg).ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            return Ok(res);
        }


        [HttpGet("{id}")]
        public virtual async Task<ApiResult<TSelectDto>> Get(TKey id, CancellationToken cancellationToken)
        {
            var dto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }

        [HttpPost]
        public virtual async Task<ApiResult<TSelectDto>> Create(TDto dto, CancellationToken cancellationToken)
        {

            var model = dto.ToEntity(Mapper);

            await Repository.AddAsync(model, cancellationToken);

            var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }

        [HttpPut]
        public virtual async Task<ApiResult<TSelectDto>> Update(TKey id, TDto dto, CancellationToken cancellationToken)
        {
            var model = await Repository.GetByIdAsync(cancellationToken, id);

            model = dto.ToEntity(Mapper, model);

            await Repository.UpdateAsync(model, cancellationToken);

            var resultDto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id.Equals(model.Id), cancellationToken);

            return resultDto;
        }

        [HttpDelete]
        public virtual async Task<ApiResult> Delete(List<TKey> ids, CancellationToken cancellationToken)
        {
            foreach (var id in ids)
            {
                var model = await Repository.GetByIdAsync(cancellationToken, id);

                await Repository.DeleteAsync(model, cancellationToken);
            }


            return Ok();
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
