using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Users;
using Rise.Shared.Users;
using Microsoft.AspNetCore.Authorization;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;

namespace Rise.Server.Controllers;

/// <summary>
/// API controller for managing user-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IManagementApiClient _managementApiClient;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class with the specified user service.
    /// </summary>
    /// <param name="userService">The user service that handles user operations.</param>
    /// <param name="managementApiClient">The management API for Auth0</param>
    public UserController(IUserService userService, IManagementApiClient managementApiClient)
    {
        _userService = userService;
        _managementApiClient = managementApiClient;
    }

    /// <summary>
    /// Retrieves the current user asynchronously.
    /// </summary>
    /// <returns>The current <see cref="UserDto"/> object or <c>null</c> if no user is found.</returns>
    [HttpGet]
    public async Task<UserDto.UserBase?> Get()
    {
        var user = await _userService.GetUserAsync();
        return user;
    }

    /// <summary>
    /// Retrieves all users asynchronously.
    /// </summary>
    /// <returns>List of <see cref="UserDto"/> objects or <c>null</c> if no users are found.</returns>
    [HttpGet("all")]
    public async Task<IEnumerable<UserDto.UserBase>?> GetAllUsers()
    {
        var users = await _userService.GetAllAsync();
        return users;
    }

    /// <summary>
    /// Retrieves a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The <see cref="UserDto"/> object or <c>null</c> if no user with the specified ID is found.</returns>
    [HttpGet("{id}")]
    public async Task<UserDto.UserBase?> Get(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user;
    }

    /// <summary>
    /// Retrieves detailed information about a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve details for.</param>
    /// <returns>The detailed <see cref="UserDto.UserDetails"/> object or <c>null</c> if no user with the specified ID is found.</returns>
    [HttpGet("details/{id}")]
    public async Task<UserDto.UserDetails?> GetDetails(int id)
    {
        var user = await _userService.GetUserDetailsByIdAsync(id);
        return user;
    }

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="userDetails">The <see cref="UserDto.CreateUser"/> object containing user details to create.</param>
    /// <returns>The created <see cref="UserDto.CreateUser"/> object or <c>null</c> if the user creation fails.</returns>
    [HttpPost("createuser")]
    public async Task<bool> Post(UserDto.CreateUser userDetails)
    {
        var created = await _userService.CreateUserAsync(userDetails);
        return created;
    }

    /// <summary>
    /// Updates an existing user asynchronously.
    /// </summary>
    /// <param name="id">The id of an existing <see cref="User"/></param>
    /// <param name="userDetails">The <see cref="UserDto.UpdateUser"/> object containing updated user details.</param>
    /// <returns><c>true</c> if the update is successful; otherwise, <c>false</c>.</returns>
    [HttpPut("{id}")]
    public async Task<bool> Put(int id, UserDto.UpdateUser userDetails)
    {
        var updated = await _userService.UpdateUserAsync(id, userDetails);
        return updated;
    }

    /// <summary>
    /// Deletes a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns><c>true</c> if the deletion is successful; otherwise, <c>false</c>.</returns>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        return deleted;
    }
    
    [HttpGet("auth/users")]
    public async Task<IEnumerable<UserDto.UserTable>> GetUsers()
    {
        var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
        Console.WriteLine(users);
        return users.Select(x => new UserDto.UserTable
        {
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Blocked = x.Blocked ?? false,
        });
    }

    [HttpGet("auth/user")]
    public async Task<UserDto.UserTable> GetUser(String id)
    {
        var test = await _managementApiClient.Users.GetAsync(id);
        Console.Write(test);
        return new UserDto.UserTable
        {
            Email = test.Email,
            FirstName = test.FirstName,
            LastName = test.LastName,
            Blocked = test.Blocked ?? false,
        };
    }

    [HttpPost("auth/user")]
    public async Task<UserDto.UserTable> CreateUser(UserDto.UserBase userBase)
    {
        throw new NotImplementedException();
    }

}
