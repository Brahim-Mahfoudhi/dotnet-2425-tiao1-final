using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users;

internal class AddressConfiguration : EntityConfiguration<Address>
{
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
