using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Enums;
using Common.Utilities;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using SharedModels.Api;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Common.Utilities.SearchUtility;
namespace Shared.Api
{
    [ApiVersion("1")]
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

       

        [HttpGet("{id}")]
        public virtual async Task<ApiResult<TSelectDto>> Get(TKey id, CancellationToken cancellationToken)
        {
            var dto = await Repository.TableNoTracking.ProjectTo<TSelectDto>(Mapper.ConfigurationProvider)
                 .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto == null)
                return NotFound();

            return dto;
        }

        [HttpPost("PagedList")]
        public virtual async Task<ActionResult<List<TSelectDto>>> List([FromBody] PageListModel model, CancellationToken cancellationToken)
        {
            if (model.arg.PageSize > 100)
                model.arg.PageSize = 100;
            if (model.arg.PageSize == 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber == 0)
                model.arg.PageNumber = 1;
            var list = Repository.TableNoTracking.Where(a => a.CmsLanguage == CmsEx.GetCurrentLanguage());
            


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

                    return BadRequest("Something went wrong in Filter");
                }



            }
            if (!string.IsNullOrEmpty(model.sortField))
            {

                try
                {
                    list = SortData(list, model.sortField, model.ascending);
                }
                catch (System.Exception)
                {

                    return BadRequest("Something went wrong in Sorting");
                }

            }
            else
            {
                list = list.OrderByDescending(a => a.Id);
            }


            var res = await list.Paginate(model.arg).ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            return Ok(res);
        }

        [HttpPost]
        public virtual async Task<ApiResult<TSelectDto>> Create(TDto dto, CancellationToken cancellationToken)
        {
            var model = dto.ToEntity(Mapper);
            model.CreatorUserId = User.Identity.GetUserIdInt();
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
                if (model != null)
                    await Repository.DeleteAsync(model, cancellationToken);
            }


            return Ok();
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
}
