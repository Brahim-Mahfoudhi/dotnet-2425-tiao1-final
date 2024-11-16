using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Rise.Server.Controllers;
using Rise.Shared.Users;
using Rise.Services.Users;
using Rise.Shared.Bookings;
using Rise.Shared.Services;
using Auth0.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Rise.Domain.Users;
using Rise.Shared.Enums;

namespace Rise.Server.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAuth0UserService> _auth0UserServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _auth0UserServiceMock = new Mock<IAuth0UserService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _userController = new UserController(_userServiceMock.Object, _auth0UserServiceMock.Object,
                _bookingServiceMock.Object);
        }

        private UserDto.RegistrationUser CreateRegistrationUser()
        {
            return new UserDto.RegistrationUser("John", "Doe", "john.doe@example.com", "+3245784578",
                "verystrongpassword", "1",
                new AddressDto.GetAdress() { Street = StreetEnum.AFRIKALAAN, HouseNumber = "1" },
                new DateTime(1990, 1, 1));
        }

        private UserDto.UserDetails CreateUserDetails()
        {
            return new UserDto.UserDetails()
            {
                Id = "1", FirstName = "Keoma", LastName = "King", Email = "kingkeoma@gmail.in",
                Address = new AddressDto.GetAdress() { Street = StreetEnum.AFRIKALAAN, HouseNumber = "1" },
                Roles = [new RoleDto() { Name = RolesEnum.User }], BirthDate = new DateTime(1990, 1, 1)
            };
        }

        private UserDto.UserBase CreateUserBase(int id)
        {
            return new UserDto.UserBase(id.ToString(), $"Keoma{id}", $"King{id}", $"kingkeoma{id}@gmail.in",
                [new RoleDto() { Name = RolesEnum.User }]);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkResult_WhenUsersExist()
        {
            // Arrange
            var users = new List<UserDto.UserBase>();
            for (int i = 0; i < 10; i++)
            {
                users.Add(CreateUserBase(i));
            }

            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnNotFound_WhenNoUsersExist()
        {
            // Arrange
            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync((IEnumerable<UserDto.UserBase>)null);

            // Act
            var result = await _userController.GetAllUsers();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Get_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var user = new UserDto.UserBase(userId, "John", "Doe", "john.doe@example.com");
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userController.Get(userId);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((UserDto.UserBase)null);

            // Act
            var result = await _userController.Get(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ShouldHandleException_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var userId = "1";
            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await Record.ExceptionAsync(() => _userController.Get(userId));

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Exception>(result);
            Assert.Equal("Unexpected error", result.Message);
        }

        [Fact]
        public async Task GetDetails_ShouldReturnUserDetails_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var userDetails = new UserDto.UserDetails(userId, "John", "Doe", "john.doe@example.com", null, null,
                DateTime.UtcNow);

            _userServiceMock.Setup(s => s.GetUserDetailsByIdAsync(userId)).ReturnsAsync(userDetails);

            // Act
            var result = await _userController.GetDetails(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Verify the response is an OK (200) status
            Assert.Equal(200, okResult.StatusCode);

            var returnedUser = Assert.IsType<UserDto.UserDetails>(okResult.Value); // Extract the returned user details
            Assert.Equal(userDetails.Id, returnedUser.Id);
            Assert.Equal(userDetails.FirstName, returnedUser.FirstName);
            Assert.Equal(userDetails.LastName, returnedUser.LastName);
            Assert.Equal(userDetails.Email, returnedUser.Email);
        }
        
        [Fact]
        public async Task GetDetails_ShouldReturnNotFound_WhenUserDetailsNotExist()
        {
            // Arrange
            var userId = "1";
            _userServiceMock.Setup(s => s.GetUserDetailsByIdAsync(userId)).ReturnsAsync((UserDto.UserDetails)null);

            // Act
            var result = await _userController.GetDetails(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            // Ensure that Value is not null and contains the expected message
            Assert.NotNull(notFoundResult.Value);

            // Use dynamic to access message property
            var responseObject = notFoundResult.Value as dynamic;
            Assert.NotNull(responseObject);
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var userDetails = CreateRegistrationUser();
            _auth0UserServiceMock.Setup(a => a.RegisterUserAuth0(userDetails)).ReturnsAsync(userDetails);
            _userServiceMock.Setup(s => s.CreateUserAsync(userDetails)).ReturnsAsync((true, "UserCreatedSuccess"));

            // Act
            var result = await _userController.Post(userDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturnConflict_WhenUserAlreadyExists()
        {
            // Arrange
            var userDetails = CreateRegistrationUser();
            _auth0UserServiceMock.Setup(a => a.RegisterUserAuth0(userDetails))
                .ThrowsAsync(new UserAlreadyExistsException("User already exists"));

            // Act
            var result = await _userController.Post(userDetails);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenUserDetailsAreInvalid()
        {
            // Arrange
            UserDto.RegistrationUser userDetails = null;

            // Act
            var result = await _userController.Post(userDetails);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Post_ShouldReturnInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var userDetails = CreateRegistrationUser();
            _auth0UserServiceMock.Setup(a => a.RegisterUserAuth0(userDetails))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _userController.Post(userDetails);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnOk_WhenUserUpdatedSuccessfully()
        {
            // Arrange
            var userDetails = new UserDto.UpdateUser
                { Id = "1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            _auth0UserServiceMock.Setup(a => a.UpdateUserAuth0(userDetails)).ReturnsAsync(true);
            _auth0UserServiceMock.Setup(a => a.AssignRoleToUser(userDetails)).ReturnsAsync(true);
            _userServiceMock.Setup(s => s.UpdateUserAsync(userDetails)).ReturnsAsync(true);

            // Act
            var result = await _userController.Put(userDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenUserDetailsAreInvalid()
        {
            // Arrange
            UserDto.UpdateUser userDetails = null;

            // Act
            var result = await _userController.Put(userDetails);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }


        [Fact]
        public async Task Put_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userDetails = new UserDto.UpdateUser
            {
                Id = "1",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            _userServiceMock.Setup(s => s.UpdateUserAsync(userDetails)).ReturnsAsync(false);

            // Act
            var result = await _userController.Put(userDetails);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }


        [Fact]
        public async Task Delete_ShouldReturnOk_WhenUserDeletedSuccessfully()
        {
            // Arrange
            var userId = "1";
            _userServiceMock.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull( okResult.Value);
        }


        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _userServiceMock
                .Setup(s => s.DeleteUserAsync(userId))
                .ThrowsAsync(new UserNotFoundException($"User with ID {userId} not found."));

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenUserHasActiveBookings()
        {
            // Arrange
            var userId = "1";
            _bookingServiceMock.Setup(b => b.GetAllUserBookings(userId)).ReturnsAsync(new List<BookingDto.ViewBooking>
                { new BookingDto.ViewBooking() { bookingId = "123" } });

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);
        }


        [Fact]
        public async Task GetAllUserBookings_ShouldReturnOkResult_WhenUserHasBookings()
        {
            // Arrange
            var userId = "1";
            var bookings = new List<BookingDto.ViewBooking> { new BookingDto.ViewBooking() { bookingId = "123" } };
            _bookingServiceMock.Setup(b => b.GetAllUserBookings(userId)).ReturnsAsync(bookings);

            // Act
            var result = await _userController.GetAllUserBookings(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(bookings, okResult.Value);
        }

        [Fact]
        public async Task GetAllUserBookings_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _bookingServiceMock.Setup(b => b.GetAllUserBookings(userId))
                .ThrowsAsync(new UserNotFoundException("User not found"));

            // Act
            var result = await _userController.GetAllUserBookings(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.NotNull(notFoundResult.Value);
        }
    }
}