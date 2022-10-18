using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HMProductInfoAPI.Models;
using HMProductInfoAPI.Services;

namespace HMProductInfoAPI.Data
{
    public class HMProductDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;
        public HMProductDbContext (DbContextOptions<HMProductDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            this._currentUserService = currentUserService;
        }

        public DbSet<HMProductInfoAPI.Models.Product> Products { get; set; } = default!;

        public override int SaveChanges()
        {
            ProcessSave();

            return base.SaveChanges();
        }
        private void ProcessSave()
        {
            var currentTime = DateTimeOffset.UtcNow;
            var newlyAddedEntities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added && x.Entity is Entity);

            foreach (var item in newlyAddedEntities)
            {
                var entity = item.Entity as Entity;
                entity.CreatedDate = currentTime;
                entity.CreatedByUser = _currentUserService.GetCurrentUserName();
            }
        }
    }
}
