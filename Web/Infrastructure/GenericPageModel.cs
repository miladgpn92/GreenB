using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using DariaCMS.Common;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedModels;
using SharedModels.Api;

namespace Web.Infrastructure
{
    [Authorize]
    public class GenericPageModel<TSelectDto, TEntity> : PageModel
        where TSelectDto : BaseWithSeoDto<TSelectDto, TEntity, int>, new()
        where TEntity : BaseWithSeoEntity<int>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public GenericPageModel(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }


        [BindProperty]
        public List<TSelectDto> Items { get; set; } = new List<TSelectDto>();

        [BindProperty]
        public PageListModel PageFilter { get; set; } = new();

 

        public virtual async Task<IActionResult> OnGetAsync(PageListModel model, string filter, CancellationToken cancellationToken)
        {
            if(filter != null)
            {
                // sample: filter=[{"field":"title","value":"e","operator":"Contains"}]
                var FilterRaw = JsonConvert.DeserializeObject<List<FilterCriteria>>(filter);
                if (FilterRaw != null && FilterRaw.Count > 0)
                {
                    model.filters = FilterRaw;
                }
            }
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
                    list = Common.Utilities.SearchUtility.SortData(list, model.sortField, model.ascending);
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


            PageFilter = model;

            var res = await list.Paginate(model.arg).ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            Items = res;

            return Page();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int[] id, CancellationToken cancellationToken)
        {
            foreach (var item in id)
            {
                var model = await Repository.GetByIdAsync(cancellationToken, item);
                if (model != null)
                    await Repository.DeleteAsync(model, cancellationToken);
            }
            return RedirectToPage("./Index");
        }

    }

    public class GenericBasePageModel<TSelectDto, TEntity> : PageModel
      where TSelectDto : BaseDto<TSelectDto, TEntity, int>, new()
      where TEntity : BaseEntity<int>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public GenericBasePageModel(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }


        [BindProperty]
        public List<TSelectDto> Items { get; set; } = new List<TSelectDto>();

        [BindProperty]
        public PageListModel PageFilter { get; set; } = new();



        public virtual async Task<IActionResult> OnGetAsync(PageListModel model, string filter, CancellationToken cancellationToken)
        {
            if (filter != null)
            {
                // sample: filter=[{"field":"title","value":"e","operator":"Contains"}]
                var FilterRaw = JsonConvert.DeserializeObject<List<FilterCriteria>>(filter);
                if (FilterRaw != null && FilterRaw.Count > 0)
                {
                    model.filters = FilterRaw;
                }
            }
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
                    list = Common.Utilities.SearchUtility.SortData(list, model.sortField, model.ascending);
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


            PageFilter = model;

            var res = await list.Paginate(model.arg).ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            Items = res;

            return Page();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int[] id, CancellationToken cancellationToken)
        {
            foreach (var item in id)
            {
                var model = await Repository.GetByIdAsync(cancellationToken, item);
                if (model != null)
                    await Repository.DeleteAsync(model, cancellationToken);
            }
            return RedirectToPage("./Index");
        }

    }

    public class GenericSimplePageModel<TSelectDto, TEntity> : PageModel
    where TSelectDto : SimpleBaseDto<TSelectDto, TEntity, int>, new()
    where TEntity : SimpleBaseEntity<int>, new()
    {
        protected readonly IRepository<TEntity> Repository;
        protected readonly IMapper Mapper;

        public GenericSimplePageModel(IRepository<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }


        [BindProperty]
        public List<TSelectDto> Items { get; set; } = new List<TSelectDto>();

        [BindProperty]
        public PageListModel PageFilter { get; set; } = new();



        public virtual async Task<IActionResult> OnGetAsync(PageListModel model, string filter, CancellationToken cancellationToken)
        {
            if (filter != null)
            {
                // sample: filter=[{"field":"title","value":"e","operator":"Contains"}]
                var FilterRaw = JsonConvert.DeserializeObject<List<FilterCriteria>>(filter);
                if (FilterRaw != null && FilterRaw.Count > 0)
                {
                    model.filters = FilterRaw;
                }
            }
            if (model.arg.PageSize > 100)
                model.arg.PageSize = 100;
            if (model.arg.PageSize == 0)
                model.arg.PageSize = 10;
            if (model.arg.PageNumber == 0)
                model.arg.PageNumber = 1;
            var list = Repository.TableNoTracking;


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
                    list = Common.Utilities.SearchUtility.SortData(list, model.sortField, model.ascending);
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


            PageFilter = model;

            var res = await list.Paginate(model.arg).ProjectTo<TSelectDto>(Mapper.ConfigurationProvider).ToListAsync(cancellationToken);

            Items = res;

            return Page();

        }

        public async Task<IActionResult> OnPostDeleteAsync(int[] id, CancellationToken cancellationToken)
        {
            foreach (var item in id)
            {
                var model = await Repository.GetByIdAsync(cancellationToken, item);
                if (model != null)
                    await Repository.DeleteAsync(model, cancellationToken);
            }
            return RedirectToPage("./Index");
        }

    }
}
