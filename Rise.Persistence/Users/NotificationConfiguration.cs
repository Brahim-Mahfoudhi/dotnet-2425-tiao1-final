using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;

namespace Rise.Persistence.Users;

internal class NotificationConfiguration : EntityConfiguration<Notification>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(Notification));
        builder.HasIndex(x => x.Id).IsUnique();
        builder.Property(x => x.Title_EN).IsRequired();
        builder.Property(x => x.Title_NL).IsRequired();
        builder.Property(x => x.Message_EN).IsRequired();
        builder.Property(x => x.Message_NL).IsRequired();
        builder.Property(x => x.IsRead).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
    }
}