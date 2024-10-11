using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Controllers;

/// <summary>
/// API controller for managing user-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class with the specified user service.
    /// </summary>
    /// <param name="userService">The user service that handles user operations.</param>
    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    /// <summary>
    /// Retrieves the current user asynchronously.
    /// </summary>
    /// <returns>The current <see cref="UserDto"/> object or <c>null</c> if no user is found.</returns>
    [HttpGet]
    public async Task<UserDto?> Get()
    {
        var user = await userService.GetUserAsync();
        return user;
    }
    
    /// <summary>
    /// Retrieves a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The <see cref="UserDto"/> object or <c>null</c> if no user with the specified ID is found.</returns>
    [HttpGet("{id}")]
    public async Task<UserDto?> Get(int id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user;
    }
    
    /// <summary>
    /// Retrieves detailed information about a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve details for.</param>
    /// <returns>The detailed <see cref="UserDto"/> object or <c>null</c> if no user with the specified ID is found.</returns>
    [HttpGet("details/{id}")]
    public async Task<UserDto?> GetDetails(int id)
    {
        var user = await userService.GetUserDetailsAsync(id);
        return user;
    }
    
    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="user">The <see cref="UserDto"/> object containing user details to create.</param>
    /// <returns>The created <see cref="UserDto"/> object or <c>null</c> if the user creation fails.</returns>
    [HttpPost]
    public async Task<UserDto?> Post(UserDto user)
    {
        var createdUser = await userService.CreateUserAsync(user);
        return createdUser;
    }

    /// <summary>
    /// Updates an existing user asynchronously.
    /// </summary>
    /// <param name="user">The <see cref="UserDto"/> object containing updated user details.</param>
    /// <returns><c>true</c> if the update is successful; otherwise, <c>false</c>.</returns>
    [HttpPut]
    public async Task<bool> Put(UserDto user)
    {
        var updated = await userService.UpdateUserAsync(user);
        return updated;
    }

    /// <summary>
    /// Deletes a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns><c>true</c> if the deletion is successful; otherwise, <c>false</c>.</returns>
    [HttpPut("{id}")]
    public async Task<bool> Put(int id)
    {
        var deleted = await userService.DeleteUserAsync(id);
        return deleted;
    }

}