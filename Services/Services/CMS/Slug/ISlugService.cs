using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.CMS
{
    public interface ISlugService<T> where T : class, IEntity
    {
        string CheckSlug(string slug, CancellationToken cancellationToken);
    }
}
