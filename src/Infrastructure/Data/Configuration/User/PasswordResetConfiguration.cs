using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Data.Configuration.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorCleanArchitecture.Infrastructure.Data.Application.Configuration.User
{
    public sealed class PasswordResetConfiguration : AuditableEntityConfiguration<PasswordReset>
    {
        public PasswordResetConfiguration(ITenantService tenantService) : base(tenantService)
        {
        }

        public override void ConfigureEntity(EntityTypeBuilder<PasswordReset> builder)
        {
            builder.ToTable("PasswordReset", "USER");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasDefaultValueSql("newsequentialid()");
            builder.Property(x => x.Date).HasDefaultValueSql("getdate()");
            builder.Property(x => x.UserId).IsRequired();

            builder.HasOne(x => x.User)
                .WithOne(x => x.PasswordReset);
        }
    }
}
