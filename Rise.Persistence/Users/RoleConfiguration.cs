using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users;

/// <summary>
/// Provides configuration settings for the <see cref="Role"/> entity in the persistence layer.
/// </summary>
internal class RoleConfiguration : EntityConfiguration<Role>
{
    /// <summary>
    /// Configures the <see cref="Role"/> entity using the specified <see cref="EntityTypeBuilder{TEntity}"/>.
    /// </summary>
    /// <param name="builder">The builder used to configure the <see cref="Role"/> entity.</param>
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(Role));

        builder.HasIndex(x => x.Id).IsUnique();
        
        builder.Property(x => x.Name).IsRequired();
    }
}
