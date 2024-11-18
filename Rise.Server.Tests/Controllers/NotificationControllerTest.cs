// using System;
// using System.Collections.Generic;
// using System.Security.Claims;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Xunit;
// using Rise.Server.Controllers;
// using Rise.Services.Notifications;
// using Rise.Shared.Notifications;
// using Rise.Shared.Enums;
// using Shouldly;

// namespace Rise.Server.Tests.Controllers
// {
//     public class NotificationControllerTest
//     {
//         private readonly Mock<INotificationService> _mockNotificationService;
//         private readonly NotificationController _controller;

//         public NotificationControllerTest()
//         {
//             _mockNotificationService = new Mock<INotificationService>();
//             _controller = new NotificationController(_mockNotificationService.Object);
//             SetControllerContextWithAuthorizedUser();
//         }

//         private void SetControllerContextWithAuthorizedUser(string role = "Admin")
//         {
//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim(ClaimTypes.NameIdentifier, "auth0|12345"),
//                 new Claim(ClaimTypes.Role, role)
//             }, "mock"));

//             _controller.ControllerContext = new ControllerContext
//             {
//                 HttpContext = new DefaultHttpContext { User = user }
//             };
//         }

//         [Fact]
//         public async Task GetAllNotifications_ReturnsOkResult_WithNotifications_WhenUserIsAdmin()
//         {
//             // Arrange
//             var notifications = CreateSampleNotifications();
//             SetupNotificationServiceForGetAllNotifications(notifications);

//             // Act
//             var result = await _controller.GetAllNotifications("en");

//             // Assert
//             AssertOkResult(result, notifications);
//         }

//         [Fact]
//         public async Task GetAllNotifications_ReturnsUnauthorized_WhenUserIsNotAdmin()
//         {
//             // Arrange
//             SetControllerContextWithAuthorizedUser("User"); // Non-admin role

//             // Act
//             var result = await _controller.GetAllNotifications("en");

//             // Assert
//             result.ShouldBeOfType<ForbidResult>(); // User should not have access
//         }

//         [Fact]
//         public async Task GetUserNotifications_ReturnsOkResult_WithNotifications()
//         {
//             // Arrange
//             var userId = "user1";
//             var notifications = CreateSampleNotifications();
//             SetupNotificationServiceForGetUserNotifications(userId, notifications);

//             // Act
//             var result = await _controller.GetUserNotifications(userId, "en");

//             // Assert
//             AssertOkResult(result, notifications);
//         }

//         [Fact]
//         public async Task CreateNotification_ReturnsOkResult_WithCreatedNotification()
//         {
//             // Arrange
//             var newNotification = new NotificationDto.NewNotification { Title_EN = "New Notification" };
//             var createdNotification = new NotificationDto.ViewNotification { NotificationId = "1", Title = "New Notification" };
//             _mockNotificationService.Setup(s => s.CreateNotificationAsync(newNotification, "en")).ReturnsAsync(createdNotification);

//             // Act
//             var result = await _controller.CreateNotification(newNotification, "en");

//             // Assert
//             AssertOkResult(result, createdNotification);
//         }

//         [Fact]
//         public async Task GetNotificationById_ReturnsOkResult_WhenNotificationExists()
//         {
//             // Arrange
//             var notificationId = "1";
//             var notification = new NotificationDto.ViewNotification { NotificationId = notificationId, Title = "Notification" };
//             SetupNotificationServiceForGetNotificationById(notificationId, notification);

//             // Act
//             var result = await _controller.GetNotificationById(notificationId, "en");

//             // Assert
//             AssertOkResult(result, notification);
//         }

//         [Fact]
//         public async Task GetNotificationById_ReturnsNotFound_WhenNotificationDoesNotExist()
//         {
//             // Arrange
//             var notificationId = "999"; // Non-existent ID
//             SetupNotificationServiceForGetNotificationById(notificationId, null);

//             // Act
//             var result = await _controller.GetNotificationById(notificationId, "en");

//             // Assert
//             AssertNotFoundResult(result, $"Notification with ID {notificationId} not found.");
//         }

//         [Fact]
//         public async Task DeleteNotification_ReturnsOkResult_WhenNotificationIsDeleted()
//         {
//             // Arrange
//             var notificationId = "1";
//             SetupNotificationServiceForDeleteNotification(notificationId, true);

//             // Act
//             var result = await _controller.DeleteNotification(notificationId);

//             // Assert
//             AssertOkResult(result, true);
//         }

//         [Fact]
//         public async Task DeleteNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
//         {
//             // Arrange
//             var notificationId = "999"; // Non-existent ID
//             SetupNotificationServiceForDeleteNotification(notificationId, false);

//             // Act
//             var result = await _controller.DeleteNotification(notificationId);

//             // Assert
//             AssertNotFoundResult(result, $"Notification with ID {notificationId} not found.");
//         }

//         [Fact]
//         public async Task UpdateNotification_ReturnsOkResult_WhenNotificationIsUpdated()
//         {
//             // Arrange
//             var updateNotification = new NotificationDto.UpdateNotification { NotificationId = "1", Title_EN = "Updated Title" };
//             _mockNotificationService.Setup(s => s.UpdateNotificationAsync(updateNotification)).ReturnsAsync(true);

//             // Act
//             var result = await _controller.UpdateNotification(updateNotification);

//             // Assert
//             AssertOkResult(result, new { message = "Notification updated successfully." });
//         }

//         [Fact]
//         public async Task UpdateNotification_ReturnsNotFound_WhenNotificationDoesNotExist()
//         {
//             // Arrange
//             var updateNotification = new NotificationDto.UpdateNotification { NotificationId = "999" };
//             _mockNotificationService.Setup(s => s.UpdateNotificationAsync(updateNotification)).ReturnsAsync(false);

//             // Act
//             var result = await _controller.UpdateNotification(updateNotification);

//             // Assert
//             AssertNotFoundResult(result, $"Notification with ID {updateNotification.NotificationId} not found.");
//         }

//         [Fact]
//         public async Task GetUnreadNotifications_ReturnsOkResult_WithUnreadNotifications()
//         {
//             // Arrange
//             var userId = "user1";
//             var notifications = CreateSampleNotifications();
//             SetupNotificationServiceForGetUnreadNotifications(userId, notifications);

//             // Act
//             var result = await _controller.GetUnreadNotifications(userId, "en");

//             // Assert
//             AssertOkResult(result, notifications);
//         }

//         [Fact]
//         public async Task GetReadNotifications_ReturnsOkResult_WithReadNotifications()
//         {
//             // Arrange
//             var userId = "user1";
//             var notifications = CreateSampleNotifications();
//             SetupNotificationServiceForGetReadNotifications(userId, notifications);

//             // Act
//             var result = await _controller.GetReadNotifications(userId, "en");

//             // Assert
//             AssertOkResult(result, notifications);
//         }

//         [Fact]
//         public async Task GetUserNotificationsByType_ReturnsOkResult_WithNotifications()
//         {
//             // Arrange
//             var userId = "user1";
//             var notificationType = NotificationType.General;
//             var notifications = CreateSampleNotifications();
//             SetupNotificationServiceForGetNotificationsByType(userId, notificationType, notifications);

//             // Act
//             var result = await _controller.GetUserNotificationsByType(userId, notificationType, "en");

//             // Assert
//             AssertOkResult(result, notifications);
//         }

//         // Helper Methods for Setting Up Mock Services
//         private void SetupNotificationServiceForGetAllNotifications(IEnumerable<NotificationDto.ViewNotification> notifications)
//         {
//             _mockNotificationService.Setup(s => s.GetAllNotificationsAsync("en")).ReturnsAsync(notifications);
//         }

//         private void SetupNotificationServiceForGetUserNotifications(string userId, IEnumerable<NotificationDto.ViewNotification> notifications)
//         {
//             _mockNotificationService.Setup(s => s.GetAllUserNotifications(userId, "en")).ReturnsAsync(notifications);
//         }

//         private void SetupNotificationServiceForGetNotificationById(string notificationId, NotificationDto.ViewNotification? notification)
//         {
//             _mockNotificationService.Setup(s => s.GetNotificationById(notificationId, "en")).ReturnsAsync(notification);
//         }

//         private void SetupNotificationServiceForDeleteNotification(string notificationId, bool result)
//         {
//             _mockNotificationService.Setup(s => s.DeleteNotificationAsync(notificationId)).ReturnsAsync(result);
//         }

//         private void SetupNotificationServiceForGetUnreadNotifications(string userId, IEnumerable<NotificationDto.ViewNotification> notifications)
//         {
//             _mockNotificationService.Setup(s => s.GetUnreadUserNotifications(userId, "en")).ReturnsAsync(notifications);
//         }

//         private void SetupNotificationServiceForGetReadNotifications(string userId, IEnumerable<NotificationDto.ViewNotification> notifications)
//         {
//             _mockNotificationService.Setup(s => s.GetReadUserNotifications(userId, "en")).ReturnsAsync(notifications);
//         }

//         private void SetupNotificationServiceForGetNotificationsByType(string userId, NotificationType type, IEnumerable<NotificationDto.ViewNotification> notifications)
//         {
//             _mockNotificationService.Setup(s => s.GetUserNotificationsByType(userId, type, "en")).ReturnsAsync(notifications);
//         }

//         // Helper Methods for Assertions
//         private static void AssertOkResult(IActionResult result, object expectedValue)
//         {
//             var okResult = result as OkObjectResult;
//             okResult.ShouldNotBeNull();
//             okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
//             okResult.Value.ShouldBeEquivalentTo(expectedValue);
//         }

//         private static void AssertNotFoundResult(IActionResult result, string expectedMessage)
//         {
//             var notFoundResult = result as NotFoundObjectResult;
//             notFoundResult.ShouldNotBeNull();
//             notFoundResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
//             notFoundResult.Value.ShouldBeEquivalentTo(new { message = expectedMessage });
//         }

//         // Helper Method to Create Sample Notifications
//         private static IEnumerable<NotificationDto.ViewNotification> CreateSampleNotifications()
//         {
//             return new List<NotificationDto.ViewNotification>
//             {
//                 new NotificationDto.ViewNotification { NotificationId = "1", Title = "Notification 1", Message = "Message 1", IsRead = false, Type = NotificationType.General, CreatedAt = DateTime.UtcNow },
//                 new NotificationDto.ViewNotification { NotificationId = "2", Title = "Notification 2", Message = "Message 2", IsRead = true, Type = NotificationType.UserRegistration, CreatedAt = DateTime.UtcNow }
//             };
//         }
//     }
// }
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Rise.Server.Controllers;
// using Rise.Shared.Enums;
// using Rise.Shared.Notifications;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Xunit;

// public class NotificationControllerTests
// {
//     private readonly Mock<INotificationService> _mockNotificationService;
//     private readonly NotificationController _controller;

//     public NotificationControllerTests()
//     {
//         _mockNotificationService = new Mock<INotificationService>();
//         _controller = new NotificationController(_mockNotificationService.Object);
//     }

//     [Fact]
//     public async Task GetAllNotifications_ReturnsOkResult_WithNotifications()
//     {
//         // Arrange
//         var notifications = new List<NotificationDto.ViewNotification>
//         {
//             new NotificationDto.ViewNotification { NotificationId = "1", Title = "Test Notification", Message = "Test Message", IsRead = false, Type = NotificationType.General }
//         };
//         _mockNotificationService.Setup(service => service.GetAllNotificationsAsync(It.IsAny<string>())).ReturnsAsync(notifications);

//         // Act
//         var result = await _controller.GetAllNotifications();

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         Assert.Equal(200, okResult.StatusCode);
//         Assert.Equal(notifications, okResult.Value);
//     }

//     [Fact]
//     public async Task GetAllNotifications_ReturnsInternalServerError_OnException()
//     {
//         // Arrange
//         _mockNotificationService.Setup(service => service.GetAllNotificationsAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

//         // Act
//         var result = await _controller.GetAllNotifications();

//         // Assert
//         var statusResult = Assert.IsType<ObjectResult>(result);
//         Assert.Equal(500, statusResult.StatusCode);
//     }

//     [Fact]
//     public async Task GetUserNotifications_ReturnsOkResult_WithUserNotifications()
//     {
//         // Arrange
//         var userId = "123";
//         var notifications = new List<NotificationDto.ViewNotification>
//         {
//             new NotificationDto.ViewNotification { NotificationId = "1", Title = "User Notification", Message = "User Message", IsRead = false, Type = NotificationType.Alert }
//         };
//         _mockNotificationService.Setup(service => service.GetAllUserNotifications(userId, It.IsAny<string>())).ReturnsAsync(notifications);

//         // Act
//         var result = await _controller.GetUserNotifications(userId);

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         Assert.Equal(200, okResult.StatusCode);
//         Assert.Equal(notifications, okResult.Value);
//     }

//     [Fact]
//     public async Task GetUserNotifications_ReturnsBadRequest_OnArgumentNullException()
//     {
//         // Arrange
//         var userId = "123";
//         _mockNotificationService.Setup(service => service.GetAllUserNotifications(userId, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("userId"));

//         // Act
//         var result = await _controller.GetUserNotifications(userId);

//         // Assert
//         var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//         Assert.Equal(400, badRequestResult.StatusCode);
//     }

//     [Fact]
//     public async Task CreateNotification_ReturnsOkResult_WithCreatedNotification()
//     {
//         // Arrange
//         var newNotification = new NotificationDto.NewNotification
//         {
//             UserId = "123",
//             Title_EN = "Title",
//             Message_EN = "Message",
//             Type = NotificationType.General
//         };
//         var createdNotification = new NotificationDto.ViewNotification
//         {
//             NotificationId = "1",
//             Title = "Title",
//             Message = "Message",
//             IsRead = false,
//             Type = NotificationType.General
//         };
//         _mockNotificationService.Setup(service => service.CreateNotificationAsync(newNotification, It.IsAny<string>())).ReturnsAsync(createdNotification);

//         // Act
//         var result = await _controller.CreateNotification(newNotification);

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         Assert.Equal(200, okResult.StatusCode);
//         Assert.Equal(createdNotification, okResult.Value);
//     }

//     [Fact]
//     public async Task CreateNotification_ReturnsBadRequest_OnArgumentNullException()
//     {
//         // Arrange
//         var newNotification = new NotificationDto.NewNotification();
//         _mockNotificationService.Setup(service => service.CreateNotificationAsync(newNotification, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("notification"));

//         // Act
//         var result = await _controller.CreateNotification(newNotification);

//         // Assert
//         var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//         Assert.Equal(400, badRequestResult.StatusCode);
//     }

//     [Fact]
//     public async Task DeleteNotification_ReturnsOkResult_WhenNotificationIsDeleted()
//     {
//         // Arrange
//         var notificationId = "1";
//         _mockNotificationService.Setup(service => service.DeleteNotificationAsync(notificationId)).ReturnsAsync(true);

//         // Act
//         var result = await _controller.DeleteNotification(notificationId);

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         Assert.Equal(200, okResult.StatusCode);
//         Assert.Equal(true, okResult.Value);
//     }

//     [Fact]
//     public async Task DeleteNotification_ReturnsNotFound_WhenNotificationIsNotFound()
//     {
//         // Arrange
//         var notificationId = "1";
//         _mockNotificationService.Setup(service => service.DeleteNotificationAsync(notificationId)).ReturnsAsync(false);

//         // Act
//         var result = await _controller.DeleteNotification(notificationId);

//         // Assert
//         var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//         Assert.Equal(404, notFoundResult.StatusCode);
//     }

//     [Fact]
//     public async Task UpdateNotification_ReturnsOkResult_WhenNotificationIsUpdated()
//     {
//         // Arrange
//         var updateNotification = new NotificationDto.UpdateNotification
//         {
//             NotificationId = "1",
//             Title_EN = "Updated Title",
//             Message_EN = "Updated Message",
//             IsRead = true
//         };
//         _mockNotificationService.Setup(service => service.UpdateNotificationAsync(updateNotification)).ReturnsAsync(true);

//         // Act
//         var result = await _controller.UpdateNotification(updateNotification);

//         // Assert
//         var okResult = Assert.IsType<OkObjectResult>(result);
//         Assert.Equal(200, okResult.StatusCode);
//     }

//     [Fact]
//     public async Task UpdateNotification_ReturnsNotFound_WhenNotificationIsNotFound()
//     {
//         // Arrange
//         var updateNotification = new NotificationDto.UpdateNotification
//         {
//             NotificationId = "1"
//         };
//         _mockNotificationService.Setup(service => service.UpdateNotificationAsync(updateNotification)).ReturnsAsync(false);

//         // Act
//         var result = await _controller.UpdateNotification(updateNotification);

//         // Assert
//         var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//         Assert.Equal(404, notFoundResult.StatusCode);
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Moq;
using Rise.Server.Controllers;
using Rise.Shared.Enums;
using Rise.Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class NotificationControllerTests
{
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly NotificationController _controller;

    public NotificationControllerTests()
    {
        _mockNotificationService = new Mock<INotificationService>();
        _controller = new NotificationController(_mockNotificationService.Object);
    }

    /// <summary>
    /// Helper method to create sample notifications.
    /// </summary>
    private List<NotificationDto.ViewNotification> CreateSampleNotifications()
    {
        return new List<NotificationDto.ViewNotification>
        {
            new NotificationDto.ViewNotification
            {
                NotificationId = "1",
                Title = "Sample Notification 1",
                Message = "Sample Message 1",
                IsRead = false,
                Type = NotificationType.General
            },
            new NotificationDto.ViewNotification
            {
                NotificationId = "2",
                Title = "Sample Notification 2",
                Message = "Sample Message 2",
                IsRead = true,
                Type = NotificationType.Alert
            }
        };
    }

    [Fact]
    public async Task GetAllNotifications_ReturnsOkResult_WithNotifications()
    {
        // Arrange
        var notifications = CreateSampleNotifications();
        _mockNotificationService.Setup(service => service.GetAllNotificationsAsync(It.IsAny<string>())).ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetAllNotifications();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(notifications, okResult.Value);
    }

    [Fact]
    public async Task GetAllNotifications_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _mockNotificationService.Setup(service => service.GetAllNotificationsAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetAllNotifications();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetUserNotifications_ReturnsOkResult_WithUserNotifications()
    {
        // Arrange
        var userId = "123";
        var notifications = CreateSampleNotifications();
        _mockNotificationService.Setup(service => service.GetAllUserNotifications(userId, It.IsAny<string>())).ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetUserNotifications(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(notifications, okResult.Value);
    }

    [Fact]
    public async Task GetUserNotifications_ReturnsBadRequest_OnArgumentNullException()
    {
        // Arrange
        var userId = "123";
        _mockNotificationService.Setup(service => service.GetAllUserNotifications(userId, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("userId"));

        // Act
        var result = await _controller.GetUserNotifications(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task CreateNotification_ReturnsOkResult_WithCreatedNotification()
    {
        // Arrange
        var newNotification = new NotificationDto.NewNotification
        {
            UserId = "123",
            Title_EN = "Title",
            Message_EN = "Message",
            Type = NotificationType.General
        };
        var createdNotification = new NotificationDto.ViewNotification
        {
            NotificationId = "1",
            Title = "Title",
            Message = "Message",
            IsRead = false,
            Type = NotificationType.General
        };
        _mockNotificationService.Setup(service => service.CreateNotificationAsync(newNotification, It.IsAny<string>())).ReturnsAsync(createdNotification);

        // Act
        var result = await _controller.CreateNotification(newNotification);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(createdNotification, okResult.Value);
    }

    [Fact]
    public async Task CreateNotification_ReturnsBadRequest_OnArgumentNullException()
    {
        // Arrange
        var newNotification = new NotificationDto.NewNotification();
        _mockNotificationService.Setup(service => service.CreateNotificationAsync(newNotification, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("notification"));

        // Act
        var result = await _controller.CreateNotification(newNotification);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task DeleteNotification_ReturnsOkResult_WhenNotificationIsDeleted()
    {
        // Arrange
        var notificationId = "1";
        _mockNotificationService.Setup(service => service.DeleteNotificationAsync(notificationId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteNotification(notificationId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(true, okResult.Value);
    }

    [Fact]
    public async Task DeleteNotification_ReturnsNotFound_WhenNotificationIsNotFound()
    {
        // Arrange
        var notificationId = "1";
        _mockNotificationService.Setup(service => service.DeleteNotificationAsync(notificationId)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteNotification(notificationId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task UpdateNotification_ReturnsOkResult_WhenNotificationIsUpdated()
    {
        // Arrange
        var updateNotification = new NotificationDto.UpdateNotification
        {
            NotificationId = "1",
            Title_EN = "Updated Title",
            Message_EN = "Updated Message",
            IsRead = true
        };
        _mockNotificationService.Setup(service => service.UpdateNotificationAsync(updateNotification)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateNotification(updateNotification);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task UpdateNotification_ReturnsNotFound_WhenNotificationIsNotFound()
    {
        // Arrange
        var updateNotification = new NotificationDto.UpdateNotification
        {
            NotificationId = "1"
        };
        _mockNotificationService.Setup(service => service.UpdateNotificationAsync(updateNotification)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateNotification(updateNotification);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetNotificationById_ReturnsOkResult_WithNotification()
    {
        // Arrange
        var notificationId = "1";
        var notification = new NotificationDto.ViewNotification
        {
            NotificationId = notificationId,
            Title = "Sample Notification",
            Message = "Sample Message",
            IsRead = false,
            Type = NotificationType.General
        };
        _mockNotificationService.Setup(service => service.GetNotificationById(notificationId, It.IsAny<string>())).ReturnsAsync(notification);

        // Act
        var result = await _controller.GetNotificationById(notificationId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(notification, okResult.Value);
    }

    [Fact]
    public async Task GetNotificationById_ReturnsNotFound_WhenNotificationIsNotFound()
    {
        // Arrange
        var notificationId = "1";
        _mockNotificationService.Setup(service => service.GetNotificationById(notificationId, It.IsAny<string>())).ReturnsAsync((NotificationDto.ViewNotification?)null);

        // Act
        var result = await _controller.GetNotificationById(notificationId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task GetUnreadNotifications_ReturnsOkResult_WithUnreadNotifications()
    {
        // Arrange
        var userId = "123";
        var notifications = CreateSampleNotifications();
        _mockNotificationService.Setup(service => service.GetUnreadUserNotifications(userId, It.IsAny<string>())).ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetUnreadNotifications(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(notifications, okResult.Value);
    }

    [Fact]
    public async Task GetUnreadNotifications_ReturnsBadRequest_OnArgumentNullException()
    {
        // Arrange
        var userId = "123";
        _mockNotificationService.Setup(service => service.GetUnreadUserNotifications(userId, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("userId"));

        // Act
        var result = await _controller.GetUnreadNotifications(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetReadNotifications_ReturnsOkResult_WithReadNotifications()
    {
        // Arrange
        var userId = "123";
        var notifications = CreateSampleNotifications();
        _mockNotificationService.Setup(service => service.GetReadUserNotifications(userId, It.IsAny<string>())).ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetReadNotifications(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(notifications, okResult.Value);
    }

    [Fact]
    public async Task GetReadNotifications_ReturnsBadRequest_OnArgumentNullException()
    {
        // Arrange
        var userId = "123";
        _mockNotificationService.Setup(service => service.GetReadUserNotifications(userId, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("userId"));

        // Act
        var result = await _controller.GetReadNotifications(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetUserNotificationsByType_ReturnsOkResult_WithNotificationsByType()
    {
        // Arrange
        var userId = "123";
        var type = NotificationType.General;
        var notifications = CreateSampleNotifications();
        _mockNotificationService.Setup(service => service.GetUserNotificationsByType(userId, type, It.IsAny<string>())).ReturnsAsync(notifications);

        // Act
        var result = await _controller.GetUserNotificationsByType(userId, type);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(notifications, okResult.Value);
    }

    [Fact]
    public async Task GetUserNotificationsByType_ReturnsBadRequest_OnArgumentNullException()
    {
        // Arrange
        var userId = "123";
        var type = NotificationType.General;
        _mockNotificationService.Setup(service => service.GetUserNotificationsByType(userId, type, It.IsAny<string>())).ThrowsAsync(new ArgumentNullException("userId"));

        // Act
        var result = await _controller.GetUserNotificationsByType(userId, type);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

}
