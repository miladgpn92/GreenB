//using AutoMapper;
//using Data.Repositories;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using Microsoft.AspNetCore.Mvc;
//using Services.Services.CMS;
//using AutoMapper.QueryableExtensions;
//using Microsoft.EntityFrameworkCore;
//using Entities;
//using Shared.Api;

//namespace Web.Infrastructure
//{
//    [Authorize]
//    public class GenericUpsertPageModel<TSelectDto, TEntity> : PageModel
//        where TSelectDto : BaseWithSeoDto<TSelectDto, TEntity, int>, new()
//        where TEntity : BaseWithSeoEntity<int>, new()
//    {
//        private readonly IRepository<TEntity> _repository;
//        private readonly ISlugService<TEntity> _slugService;
//        private readonly Func<TEntity, bool> _shouldCheckSlug; // Added condition delegate

//        public GenericUpsertPageModel(IRepository<TEntity> repository, ISlugService<TEntity> slugService, IMapper mapper, Func<TEntity, bool> shouldCheckSlug)
//        {
//            _repository = repository;
//            _slugService = slugService;
//            Mapper = mapper;
//            _shouldCheckSlug = shouldCheckSlug; // Store the condition delegate
//        }

//        [BindProperty]
//        public TSelectDto Items { get; set; } = default!;

//        [BindProperty]
//        public bool IsEdit { get; set; }
//        public IMapper Mapper { get; }

//        public async Task OnGetAsync(int? id, CancellationToken cancellationToken)
//        {
//            if (id.HasValue)
//            {
//                IsEdit = true;

//                Items = await _repository.TableNoTracking
//                    .ProjectTo<TEntity>(Mapper.ConfigurationProvider)
//                    .SingleOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
//            }
//            else
//            {
//                IsEdit = false;
//            }
//        }

//        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Page();
//            }

            

//            if (IsEdit)
//            {
//                var model = await _repository.GetByIdAsync(cancellationToken, Items.Id);

//                model = Items.ToEntity(Mapper, model);

//                await _repository.UpdateAsync(model, cancellationToken);
//            }
//            else
//            {
//                var model = Items.ToEntity(Mapper);
//                await _repository.AddAsync(model, cancellationToken);
//            }

//            return RedirectToPage("./Index");
//        }
//    }
//}
