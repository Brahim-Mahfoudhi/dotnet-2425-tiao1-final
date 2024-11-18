using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Services.Users;
using Rise.Shared.Enums;

namespace Rise.Services.Tests.Users;

public class UserServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        // Set up in-memory database options
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _userService = new UserService(_dbContext, null);
    }

    private User CreateUser(string id)
    {
        return new User
        (
            id,
            $"John_{id}",
            $"Doe_{id}",
            $"user{id}@example.com",
            DateTime.Now,
            new Address(StreetEnum.AFRIKALAAN.ToString(), id),
            "+3247845784"
        );
    }

    private User CreateUser(int id)
    {
        return CreateUser(id.ToString());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenUsersExist()
    {
        // Arrange
        var user1 = CreateUser("1");
        var user2 = CreateUser("2");
        await _dbContext.Users.AddRangeAsync(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, user => user.Email == "user1@example.com");
        Assert.Contains(result, user => user.Email == "user2@example.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoUsersExist()
    {
        // Arrange - Ensure no users exist in the database

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldNotReturnDeletedUsers()
    {
        // Arrange
        var user1 = CreateUser("1");
        var user2 = CreateUser("2");
        user2.IsDeleted = true;
        await _dbContext.Users.AddRangeAsync(user1, user2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.DoesNotContain(result, user => user.Email == "user2@example.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsersWithRoles_WhenUsersHaveRoles()
    {
        // Arrange
        var role = new Role { Name = RolesEnum.Admin };
        var user = CreateUser("1");
        user.Roles.Add(role);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var userResult = result.FirstOrDefault();
        Assert.NotNull(userResult);
        Assert.Contains(userResult.Roles, r => r.Name == RolesEnum.Admin);
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers_WhenUsersAreAddedConcurrently()
    {
        // Arrange
        var user1 = CreateUser("1");
        var user2 = CreateUser("2");

        await Task.WhenAll(_dbContext.Users.AddAsync(user1).AsTask(), _dbContext.Users.AddAsync(user2).AsTask());
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, user => user.Email == "user1@example.com");
        Assert.Contains(result, user => user.Email == "user2@example.com");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsers_WhenUsersHaveNoRoles()
    {
        // Arrange
        var user = CreateUser("1");
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("user1@example.com", result.First().Email);
        Assert.Empty(result.First().Roles);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers_WhenDatabaseContainsManyUsers()
    {
        // Arrange
        for (int i = 1; i <= 10000; i++)
        {
            var user = CreateUser(i);
            await _dbContext.Users.AddAsync(user);
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10000, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnUsersWithEmptyRoles_WhenNoRolesAssigned()
    {
        // Arrange
        var user = CreateUser("1");
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        var returnedUser = result.First();
        Assert.NotNull(returnedUser);
        Assert.Empty(returnedUser.Roles);
    }
    
    [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = CreateUser("1");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("John_1", result.FirstName);
            Assert.Equal("Doe_1", result.LastName);
            Assert.Equal("user1@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            // No user added to the database

            // Act
            var result = await _userService.GetUserByIdAsync("1");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserIsDeleted()
        {
            // Arrange
            var user = CreateUser("1");
            user.IsDeleted = true;
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync("1");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserWithRoles_WhenUserHasRoles()
        {
            // Arrange
            var role = new Role { Name = RolesEnum.Admin };
            var user = CreateUser("1");
            user.Roles.Add(role);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result.Roles, r => r.Name == RolesEnum.Admin);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserHasNoRoles()
        {
            // Arrange
            var user = CreateUser("2");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync("2");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Roles);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldHandleInvalidUserId_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = string.Empty;

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldHandleInvalidUserId_WhenUserIdIsNull()
        {
            // Arrange
            string userId = null;

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserHasMultipleRoles()
        {
            // Arrange
            var role1 = new Role { Name = RolesEnum.Admin };
            var role2 = new Role { Name = RolesEnum.User };
            var user = CreateUser("3");
            user.Roles.Add(role1);
            user.Roles.Add(role2);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync("3");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Roles.Count);
            Assert.Contains(result.Roles, r => r.Name == RolesEnum.Admin);
            Assert.Contains(result.Roles, r => r.Name == RolesEnum.User);
        }
        
        [Fact]
        public async Task GetUserDetailsByIdAsync_ShouldReturnUserDetails_WhenUserExists()
        {
            // Arrange
            var user = CreateUser("1");
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserDetailsByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("John_1", result.FirstName);
            Assert.Equal("Doe_1", result.LastName);
            Assert.Equal("user1@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserDetailsByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "non_existent";

            // Act
            var result = await _userService.GetUserDetailsByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserDetailsByIdAsync_ShouldReturnNull_WhenUserIsDeleted()
        {
            // Arrange
            var user = CreateUser("2");
            user.IsDeleted = true;
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserDetailsByIdAsync("2");

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetUserDetailsByIdAsync_ShouldReturnNull_WhenUserIdIsNullOrEmpty(string userId)
        {
            // Act
            var result = await _userService.GetUserDetailsByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }
        
    [Fact]
    public async Task GetUserDetailsByIdAsync_ShouldReturnUserDetails_WhenUserHasMultipleRoles()
    {
        // Arrange
        var user = CreateUser("4");
        user.Roles.Add(new Role { Name = RolesEnum.Admin });
        user.Roles.Add(new Role { Name = RolesEnum.User });
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _userService.GetUserDetailsByIdAsync("4");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("4", result.Id);
        Assert.Equal("John_4", result.FirstName);
        Assert.Equal("Doe_4", result.LastName);
        Assert.Equal("user4@example.com", result.Email);
        Assert.Contains(result.Roles, r => r.Name == RolesEnum.User);
        Assert.Contains(result.Roles, r => r.Name == RolesEnum.Admin);
    }
    
    
}