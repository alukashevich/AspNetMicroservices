﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence.Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Persistence
{
    namespace Ordering.Infrastructure.Persistence
    {
        public class OrderContext : DbContext
        {
            public OrderContext(DbContextOptions<OrderContext> options) : base(options)
            {
            }

            public DbSet<Order> Orders { get; set; }

            public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                foreach (var entry in ChangeTracker.Entries<EntityBase>())
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.CreatedDate = DateTime.Now;
                            entry.Entity.CreatedBy = "swn";
                            break;
                        case EntityState.Modified:
                            entry.Entity.LastModifiedDate = DateTime.Now;
                            entry.Entity.LastModifiedBy = "swn";
                            break;
                    }
                }
                return base.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
