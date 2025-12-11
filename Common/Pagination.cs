using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DariaCMS.Common
{


    public class SearchPager
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string Search { get; set; }
    }

    public interface IPaginationInfo
    {
        int PageNumber { get; }
        int PageSize { get; }
    }

    public class Pageres : IPaginationInfo
    {
        
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

    public static class PaginationExtensions
    {
        public static void Normalize(this Pageres pager, int defaultPageSize = 10, int maxPageSize = 100)
        {
            if (pager == null)
                return;

            if (pager.PageSize <= 0 || pager.PageSize > maxPageSize)
                pager.PageSize = defaultPageSize;
            if (pager.PageNumber <= 0)
                pager.PageNumber = 1;
        }
    }
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(
            this IQueryable<T> source,
            IPaginationInfo pagination)
        {
            return source
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize);
        }
    }
}
