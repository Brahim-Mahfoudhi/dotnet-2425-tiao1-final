using Rise.Shared.Enums;
using Rise.Shared.Notifications;
using Rise.Shared.Users;

namespace Rise.Services.Events.User;

/// <summary>
/// Handles the UserRegisteredEvent to notify admins.
/// </summary>
public class NotifyAdminsOnUserDeletionHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyAdminsOnUserDeletionHandler"/> class.
    /// </summary>
    /// <param name="notificationService">The notification service.</param>
    /// <param name="userService">The user service.</param>
    public NotifyAdminsOnUserDeletionHandler(
        INotificationService notificationService,
        IUserService userService)
    {
        _notificationService = notificationService;
        _userService = userService;
    }

    /// <summary>
    /// Handles the UserRegisteredEvent to notify admins about user deletion.
    /// </summary>
    /// <param name="event">The user registered event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task HandleAsync(UserRegisteredEvent @event)
    {
        // Fetch all admin users
        var admins = await _userService.GetFilteredUsersAsync(new UserFilter { Role = RolesEnum.Admin });

        // Create notifications for each admin
        foreach (var admin in admins)
        {
            var notification = new NotificationDto.NewNotification
            {
                UserId = admin.Id,
                Title_EN = $"User Deleted : {@event.FirstName} {@event.LastName}",
                Title_NL = $"Gebruiker verwijderd: {@event.FirstName} {@event.LastName}",
                Message_EN = $"The user {@event.FirstName} {@event.LastName} (ID: {@event.UserId}) has initiated their own deletion.",
                Message_NL = $"De gebruiker {@event.FirstName} {@event.LastName} (ID: {@event.UserId}) heeft zijn eigen verwijdering ingediend.",
                RelatedEntityId = @event.UserId,
                Type = NotificationType.Alert
            };

            await _notificationService.CreateNotificationAsync(notification);
        }
    }
}
