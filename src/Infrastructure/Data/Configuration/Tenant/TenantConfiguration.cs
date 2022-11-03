using BlazorCleanArchitecture.Infrastructure.Common.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorCleanArchitecture.Infrastructure.Data.Configuration.Tenant
{
    public sealed class TenantConfiguration : EntityTypeConfigurationDependency<Domain.Tenant.Tenant>
    {
        public override void Configure(EntityTypeBuilder<Domain.Tenant.Tenant> builder)
        {
            builder.ToTable("Tenant", "TENANT", b => b.IsTemporal());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Domain).IsRequired();

            builder.HasData(new Domain.Tenant.Tenant
            {
                Id = 1,
                Name = "Main",
                Domain = "main.com"
            });
        }
    }
}
