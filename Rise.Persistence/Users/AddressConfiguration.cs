using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users;

/// <summary>
/// Provides configuration settings for the <see cref="Address"/> entity in the persistence layer.
/// </summary>
internal class AddressConfiguration : EntityConfiguration<Address>
{
    /// <summary>
    /// Configures the <see cref="Address"/> entity using the specified <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the <see cref="Address"/> entity.</param>
    public override void Configure(EntityTypeBuilder<Address> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(Address));
        
        builder.HasIndex(x => x.Id).IsUnique();
        
        builder.Property(x => x.Street).IsRequired();
        builder.Property(x => x.HouseNumber).IsRequired();
        builder.Property(x => x.Bus).HasMaxLength(10);
    }
}
