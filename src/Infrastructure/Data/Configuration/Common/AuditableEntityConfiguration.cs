using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.Common;
using BlazorCleanArchitecture.Infrastructure.Common.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorCleanArchitecture.Infrastructure.Data.Configuration.Common
{
    public abstract class AuditableEntityConfiguration<TEntity> : EntityTypeConfigurationDependency<TEntity>
        where TEntity : AuditableEntity
    {

        private readonly ITenantService _tenantService;

        public AuditableEntityConfiguration(ITenantService tenantService)
            => _tenantService = tenantService;

        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            ConfigureEntity(builder);

            builder.Property(x => x.Created).ValueGeneratedOnAdd();
            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.Modified).ValueGeneratedOnUpdate();
            builder.Property(x => x.ModifiedBy);
            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.TenantId).IsRequired();

            builder.HasQueryFilter(x => x.TenantId == _tenantService.Tenant.Id);
        }

        public abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
    }
}
