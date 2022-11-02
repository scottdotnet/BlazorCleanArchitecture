﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Infrastructure.Common.EntityFramework
{
    public abstract class EntityTypeConfigurationDependency
    {
        public abstract void Configure(ModelBuilder modelBuilder);
    }

    public abstract class EntityTypeConfigurationDependency<TEntity> : EntityTypeConfigurationDependency, IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);

        public override void Configure(ModelBuilder modelBuilder)
            => Configure(modelBuilder.Entity<TEntity>());
    }
}
