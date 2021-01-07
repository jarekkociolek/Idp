using Idp.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Idp.Server.DbContexts
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(q => q.Subject).IsUnique();
            modelBuilder.Entity<User>().HasIndex(q => q.Username).IsUnique();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var updatedConcurrencyAwareEntities = ChangeTracker.Entries().Where(q => q.State == EntityState.Modified).OfType<IConcurrencyAware>();

            foreach (var entity in updatedConcurrencyAwareEntities)
            {
                entity.ConcurrencyStamp = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
