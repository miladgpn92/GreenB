using Common;
using Common.Utilities;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entitiesAssembly = typeof(IEntity).Assembly;
            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly);
            modelBuilder.AddSequentialGuidForIdConvention();
            modelBuilder.AddPluralizingTableNameConvention();
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            this.AutomationBaseEntityValue();

            _cleanString();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.AutomationBaseEntityValue();

            _cleanString();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.AutomationBaseEntityValue();

            _cleanString();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.AutomationBaseEntityValue();

            _cleanString();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var propName = property.Name;
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }

        /// <summary>
        /// Automation Save some values of <see cref="BaseEntity"/> entity
        /// </summary>
        private void AutomationBaseEntityValue()
        {
            foreach (var entry in this.ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added when entry.Entity.CreateDate == null
                        || entry.Entity.CreatorUserId == null
                        || string.IsNullOrWhiteSpace(entry.Entity.CreatorIP):
                        entry.Entity.CreateDate = DateTime.Now;

                        entry.Entity.CreatorUserId = _httpContextAccessor.HttpContext.User.Identity.GetUserIdInt();

                        entry.Entity.CreatorIP = _httpContextAccessor.HttpContext.Connection.GetRemoteIp();
                        break;

                    case EntityState.Modified:
                        entry.Entity.CreateDate = DateTime.Now;

                        entry.Entity.CreatorUserId = _httpContextAccessor.HttpContext.User.Identity.GetUserIdInt();

                        entry.Entity.CreatorIP = _httpContextAccessor.HttpContext.Connection.GetRemoteIp();
                        break;
                }
            }


            foreach (var entry in this.ChangeTracker.Entries<BaseWithSeoEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added when entry.Entity.CreateDate == null
                        || entry.Entity.CreatorUserId == null
                        || string.IsNullOrWhiteSpace(entry.Entity.CreatorIP):
                        entry.Entity.CreateDate = DateTime.Now;

                        entry.Entity.CreatorUserId = _httpContextAccessor.HttpContext.User.Identity.GetUserIdInt();

                        entry.Entity.CreatorIP = _httpContextAccessor.HttpContext.Connection.GetRemoteIp();
                        break;

                    case EntityState.Modified:
                        entry.Entity.CreateDate = DateTime.Now;

                        entry.Entity.CreatorUserId = _httpContextAccessor.HttpContext.User.Identity.GetUserIdInt();

                        entry.Entity.CreatorIP = _httpContextAccessor.HttpContext.Connection.GetRemoteIp();
                        break;
                }
            }
        }
    }
}