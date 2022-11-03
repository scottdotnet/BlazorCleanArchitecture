using BlazorCleanArchitecture.Application.Common.Interfaces;
using BlazorCleanArchitecture.Infrastructure.Data.Configuration.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazorCleanArchitecture.Infrastructure.Data.Configuration.User
{
    public sealed class UserConfiguration : AuditableEntityConfiguration<Domain.User.User>
    {
        public UserConfiguration(ITenantService tenantService) : base(tenantService) { }

        public override void ConfigureEntity(EntityTypeBuilder<Domain.User.User> builder)
        {
            builder.ToTable("User", "USER", b => b.IsTemporal());

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.MiddleName).HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Username).IsRequired().HasMaxLength(320);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(320);
            builder.Property(x => x.Password);
            //builder.OwnsOne(x => x.Address, AddressConfigurationExtension.AddressConfiguration);
            builder.Property(x => x.MobileNumber).HasMaxLength(10);
            builder.Property(x => x.PhoneNumber).HasMaxLength(10);
            builder.Property(x => x.Enabled).IsRequired();
            builder.Property(x => x.LoginAttempts).IsRequired();
            builder.Property(x => x.Locked).IsRequired();
            builder.Property(x => x.MFAKey).IsRequired();

            builder.HasData(new Domain.User.User
            {
                Id = 1,
                FirstName = "Scott",
                LastName = "Aldinger",
                Username = "test@test.com",
                Email = "test@test.com",
                Password = "Abcdefgh1",
                Enabled = true,
                LoginAttempts = 0,
                Locked = false,
                MFAKey = Guid.NewGuid(),
                TenantId = 1
            });
        }
    }
}
