using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Bookings;

namespace Rise.Persistence.Bookings;

internal class BoatConfiguration: EntityConfiguration<Boat>
{
    /// <summary>
    /// Configures the <see cref="Boat"/> entity using the specified <see cref="EntityTypeBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the <see cref="Boat"/> entity.</param>
    public override void Configure(EntityTypeBuilder<Boat> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(Boat));

        builder.HasIndex(x => x.Id).IsUnique();
        
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.CountBookings).IsRequired();
        builder.Property(x => x.ListComments).IsRequired();
    }
}