using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class PaginationValidation
    {
        /// <summary>
        /// Validates and adjusts pagination parameters.
        /// </summary>
        public static (int Page, int PageSize) Validate(int page, int pageSize)
        {
            const int maxPageSize = 100;
            const int defaultPageSize = 10;
            const int defaultPage = 1;

            return (
                Page: page > 0 ? page : defaultPage,
                PageSize: pageSize > 0 ? (pageSize > maxPageSize ? maxPageSize : pageSize) : defaultPageSize
            );
        }
    }
}
