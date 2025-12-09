using Common;
using Common.Exceptions;
using Common.Utilities;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository, IScopedDependency
    {
        public UserRepository(ApplicationDbContext dbContext)
            : base(dbContext)
        {
        }


    }
}
