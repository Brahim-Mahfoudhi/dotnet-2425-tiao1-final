using Rise.Shared.Enums;
using Rise.Shared.Notifications;
using Rise.Shared.Users;

namespace Rise.Services.Events.User;

/// <summary>
/// Handles the UserRegisteredEvent to notify admins.
/// </summary>
public class NotifyAdminsOnUserRegistrationHandler : IEventHandler<UserRegisteredEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyAdminsOnUserRegistrationHandler"/> class.
    /// </summary>
    /// <param name="notificationService">The notification service.</param>
    /// <param name="userService">The user service.</param>
    public NotifyAdminsOnUserRegistrationHandler(
        INotificationService notificationService,
        IUserService userService)
    {
        _notificationService = notificationService;
        _userService = userService;
    }

    /// <summary>
    /// Handles the UserRegisteredEvent to notify all admin users.
    /// </summary>
    /// <param name="event">The user registered event.</param>
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
                Title_EN = $"New User Registered: {@event.FirstName} {@event.LastName}",
                Title_NL = $"Nieuwe gebruiker geregistreerd: {@event.FirstName} {@event.LastName}",
                Message_EN = $"A new user {@event.FirstName} {@event.LastName} has registered.",
                Message_NL = $"Een nieuwe gebruiker {@event.FirstName} {@event.LastName} heeft zich geregistreerd.",
                RelatedEntityId = @event.UserId,
                Type = NotificationType.UserRegistration
            };

            await _notificationService.CreateNotificationAsync(notification);
        }
    }
}
