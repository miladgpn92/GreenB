using AutoMapper;
using Common;
using Data.Repositories;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS
{
    public class SlugService<T> :ISlugService<T> where T : class, IEntity
    {
        private readonly IMapper _mapper;
        private readonly IRepository<T> _repository;

        public SlugService(IMapper mapper, IRepository<T> repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        

        public string CheckSlug(string slug, CancellationToken cancellationToken)
        {
            string finalSlug = slug;
            int count = 1;

            // Get the PropertyInfo for the Slug property
            PropertyInfo slugProperty = typeof(T).GetProperty("Slug");

            // Create the expression for the lambda argument
            ParameterExpression lambdaArg = Expression.Parameter(typeof(T), "x");
            MemberExpression slugMember = Expression.Property(lambdaArg, slugProperty);
            BinaryExpression compareExpr = Expression.Equal(slugMember, Expression.Constant(slug));
            Expression<Func<T, bool>> lambdaExpr = Expression.Lambda<Func<T, bool>>(compareExpr, lambdaArg);

            // Check if there's already an entity with the same slug
            while (_repository.TableNoTracking.Any(lambdaExpr))
            {
                finalSlug = slug + "-" + count;
                count++;

                // Create the expression for the lambda argument with the updated slug
                ConstantExpression finalSlugExpr = Expression.Constant(finalSlug);
                compareExpr = Expression.Equal(slugMember, finalSlugExpr);
                lambdaExpr = Expression.Lambda<Func<T, bool>>(compareExpr, lambdaArg);
            }

            return finalSlug;
        }


    }
}
