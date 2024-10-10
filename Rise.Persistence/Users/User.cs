using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users;

internal class UserConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(User));

        builder.HasIndex(x => x.Id).IsUnique();
        
        builder.Property(x => x.FirstName).IsRequired();
        builder.Property(x => x.LastName).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.Password).IsRequired();
        builder.Property(x => x.BirthDate).IsRequired();
        builder.Property(x => x.PhoneNumber).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder
            .HasOne(x => x.Address)
            .WithOne()
            .HasForeignKey(typeof(Address))
            .IsRequired();
        
        builder
            .HasMany(x => x.Roles)
            .WithOne()
            .HasForeignKey(x => x.Id);
    }
}
