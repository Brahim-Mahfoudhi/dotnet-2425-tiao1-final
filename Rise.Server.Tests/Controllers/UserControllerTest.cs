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
using Microsoft.VisualBasic.CompilerServices;
using Rise.Domain.Users;
using Rise.Shared.Enums;
using Rise.Services.Events;

namespace Rise.Server.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAuth0UserService> _auth0UserServiceMock;
        private readonly Mock<IValidationService> _validationServiceMock;

        private readonly Mock<IEventDispatcher> _eventDispatcherMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _auth0UserServiceMock = new Mock<IAuth0UserService>();
            _validationServiceMock = new Mock<IValidationService>();
            _eventDispatcherMock = new Mock<IEventDispatcher>();
            _userController = new UserController(_userServiceMock.Object, _auth0UserServiceMock.Object,
                _validationServiceMock.Object, _eventDispatcherMock.Object);
        }

        private UserDto.RegistrationUser CreateRegistrationUser(int id)
        {
            return CreateRegistrationUser(id.ToString());
        }

        private UserDto.RegistrationUser CreateRegistrationUser(string userid = "1")
        {
            return new UserDto.RegistrationUser($"John{userid}", $"Doe{userid}", $"john{userid}.doe@example.com", "+3245784578",
                $"verystrongpassword{userid}", userid,
                new AddressDto.GetAdress() { Street = StreetEnum.AFRIKALAAN, HouseNumber = $"1{userid}" },
                new DateTime(1990, 1, 1));
        }

        private UserDto.UserDetails CreateUserDetails(int userid)
        {
            return CreateUserDetails(userid.ToString());
        }

        private UserDto.UserDetails CreateUserDetails(string userid = "1")
        {
            return new UserDto.UserDetails()
            {
                Id = userid,
                FirstName = $"Keoma{userid}",
                LastName = $"King{userid}",
                Email = $"kingkeoma{userid}@gmail.in",
                Address = new AddressDto.GetAdress() { Street = StreetEnum.AFRIKALAAN, HouseNumber = $"1{userid}" },
                Roles = [new RoleDto() { Name = RolesEnum.User }],
                BirthDate = new DateTime(1990, 1, IntegerType.FromString(userid))
            };
        }

        private UserDto.UserBase CreateUserBase(int id)
        {
            return CreateUserBase(id.ToString());
        }

        private UserDto.UserBase CreateUserBase(string id = "1")
        {
            return new UserDto.UserBase(id, $"Keoma{id}", $"King{id}", $"kingkeoma{id}@gmail.in",
                [new RoleDto() { Name = RolesEnum.User }]);
        }

        private UserDto.UpdateUser CreateUpdateUser(int id)
        {
            return CreateUpdateUser(id.ToString());
        }

        private UserDto.UpdateUser CreateUpdateUser(string id = "1")
        {
            return new UserDto.UpdateUser
            {
                Id = id,
                FirstName = $"John{id}",
                LastName = $"Doe{id}",
                Email = $"john{id}.doe@example.com"
            };
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
            var user = CreateUserBase(userId);
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
            var userDetails = CreateUserDetails(userId);

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
            var userDetails = CreateUpdateUser();
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
            var userDetails = CreateUpdateUser();
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

            var user = new UserDto.UserBase(
                userId,
                "John",
                "Doe",
                "john.doe@example.com",
                ImmutableList.Create(new RoleDto { Name = RolesEnum.User })
            );

            _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _userServiceMock.Setup(s => s.SoftDeleteUserAsync(userId)).ReturnsAsync(true);
            _auth0UserServiceMock.Setup(a => a.SoftDeleteAuth0UserAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }


        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";
            _userServiceMock
                .Setup(s => s.SoftDeleteUserAsync(userId))
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
            _validationServiceMock.Setup(b => b.CheckActiveBookings(userId)).ReturnsAsync(true);

            // Act
            var result = await _userController.Delete(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);
        }


        [Fact]
        public async Task IsEmailTaken_ShouldReturnTrue_WhenEmailIsTaken()
        {
            // Arrange
            string email = "taken@example.com";
            _auth0UserServiceMock.Setup(a => a.IsEmailTakenAsync(email)).ReturnsAsync(true);

            // Act
            var result = await _userController.IsEmailTaken(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task IsEmailTaken_ShouldReturnFalse_WhenEmailIsNotTaken()
        {
            // Arrange
            string email = "not_taken@example.com";
            _auth0UserServiceMock.Setup(a => a.IsEmailTakenAsync(email)).ReturnsAsync(false);

            // Act
            var result = await _userController.IsEmailTaken(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public async Task GetFilteredUsers_ShouldReturnOk_WhenUsersAreFound()
        {
            // Arrange
            var filter = new UserFilter();
            var users = new List<UserDto.UserBase>();

            for (int i = 0; i < 10; i++)
            {
                users.Add(CreateUserBase(i));
            }

            _userServiceMock.Setup(s => s.GetFilteredUsersAsync(filter)).ReturnsAsync(users);

            // Act
            var result = await _userController.GetFilteredUsers(filter);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(users, okResult.Value);
        }

        [Fact]
        public async Task GetFilteredUsers_ShouldReturnNotFound_WhenNoUsersAreFound()
        {
            // Arrange
            var filter = new UserFilter();
            _userServiceMock.Setup(s => s.GetFilteredUsersAsync(filter))
                .ReturnsAsync((IEnumerable<UserDto.UserBase>)null);

            // Act
            var result = await _userController.GetFilteredUsers(filter);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("No users found matching the given filters.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetFilteredUsers_ShouldReturnBadRequest_WhenFilterIsInvalid()
        {
            // Arrange
            var filter = new UserFilter();
            _userServiceMock.Setup(s => s.GetFilteredUsersAsync(filter))
                .ThrowsAsync(new ArgumentException("Invalid filter"));

            // Act
            var result = await _userController.GetFilteredUsers(filter);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Invalid filter argument: Invalid filter", badRequestResult.Value);
        }

        [Fact]
        public async Task GetFilteredUsers_ShouldReturnForbid_WhenUnauthorizedAccessExceptionIsThrown()
        {
            // Arrange
            var filter = new UserFilter();
            _userServiceMock.Setup(s => s.GetFilteredUsersAsync(filter))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _userController.GetFilteredUsers(filter);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task GetFilteredUsers_ShouldReturnInternalServerError_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            var filter = new UserFilter();
            _userServiceMock.Setup(s => s.GetFilteredUsersAsync(filter)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _userController.GetFilteredUsers(filter);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("An unexpected error occurred: Unexpected error", objectResult.Value);
        }
    }
}