using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Users;
using Rise.Shared.Users;
using Microsoft.AspNetCore.Authorization;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Auth0.Core.Exceptions;

namespace Rise.Server.Controllers;

/// <summary>
/// API controller for managing user-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
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
    [Authorize]
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
    [Authorize(Roles = "Admin")]
    public async Task<IEnumerable<UserDto.UserBase>?> GetAllUsers()
    {

        // var users2 = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
        var users = await _userService.GetAllAsync();
        // return users2.Select(x => new UserDto.UserBase(x.UserId, x.FirstName, x.LastName, x.Email));
        return users;
    }

    /// <summary>
    /// Retrieves a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The <see cref="UserDto"/> object or <c>null</c> if no user with the specified ID is found.</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<UserDto.UserBase?> Get(string id)
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
    [Authorize]
    public async Task<UserDto.UserDetails?> GetDetails(string id)
    {
        var user = await _userService.GetUserDetailsByIdAsync(id);
        return user;
    }

    /// <summary>
    /// Creates a new user asynchronously.
    /// </summary>
    /// <param name="userDetails">The <see cref="UserDto.RegistrationUser"/> object containing user details to create.</param>
    /// <returns>The created <see cref="UserDto.RegistrationUser"/> object or <c>null</c> if the user creation fails.</returns>
    [HttpPost]
    public async Task<IActionResult> Post(UserDto.RegistrationUser userDetails)
    {
        try
        {
            var userDb = await RegisterUserAuth0(userDetails);
            var (success, message) = await _userService.CreateUserAsync(userDb);

            if (success)
            {
                return Ok(new { message }); // Localization key
            }
            else
            {
                return BadRequest(new { message }); // Localization key
            }
        }
        catch (UserAlreadyExistsException)
        {
            return Conflict(new { message = "UserAlreadyExists" }); // Localization key
        }
        catch (DatabaseOperationException)
        {
            return StatusCode(500, new { message = "UserCreationFailed" }); // Localization key
        }
        catch (ExternalServiceException)
        {
            return StatusCode(503, new { message = "ExternalServiceUnavailable" }); // Localization key
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "UnexpectedErrorOccurred" }); // Localization key
        }
    }


    /// <summary>
    /// Updates an existing user asynchronously.
    /// </summary>
    /// <param name="id">The id of an existing <see cref="User"/></param>
    /// <param name="userDetails">The <see cref="UserDto.UpdateUser"/> object containing updated user details.</param>
    /// <returns><c>true</c> if the update is successful; otherwise, <c>false</c>.</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<bool> Put(UserDto.UpdateUser userDetails)
    {
        var updated = await _userService.UpdateUserAsync(userDetails);
        return updated;
    }

    /// <summary>
    /// Deletes a user by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns><c>true</c> if the deletion is successful; otherwise, <c>false</c>.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<bool> Delete(string id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        return deleted;
    }

    // [HttpGet("auth/users")]
    // [Authorize]
    // public async Task<IEnumerable<UserDto.Auth0User>> GetUsers()
    // {
    //     var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
    //     Console.WriteLine(users);
    //     return users.Select(x => new UserDto.Auth0User(
    //         x.Email,
    //         x.FirstName,
    //         x.LastName,
    //         x.Blocked ?? false
    //     ));
    // }

    [HttpGet("auth/users")]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            // Fetch users from Auth0
            var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());

            // Transform and return the user list if successful
            var auth0Users = users.Select(x => new UserDto.Auth0User(
                x.Email,
                x.FirstName,
                x.LastName,
                x.Blocked ?? false
            ));

            return Ok(auth0Users);
        }
        catch (ApiException ex)
        {
            // Handle specific Auth0 API exceptions, like network or request issues
            return StatusCode(503, new { message = "Auth0 service is unavailable. Please try again later.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle any unexpected errors and return a 500 Internal Server Error
            return StatusCode(500, new { message = "An unexpected error occurred while fetching users.", detail = ex.Message });
        }
    }

    // [HttpGet("auth/user/{id}")]
    // [Authorize]
    // public async Task<UserDto.Auth0User> GetUser(string id)
    // {
    //     var test = await _managementApiClient.Users.GetAsync(id);
    //     Console.Write(test);
    //     return new UserDto.Auth0User
    //         (test.Email,
    //         test.FirstName,
    //         test.LastName,
    //         test.Blocked ?? false);
    // }

    [HttpGet("auth/user/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string id)
    {
        try
        {
            // Attempt to retrieve the user by ID
            var user = await _managementApiClient.Users.GetAsync(id);

            if (user == null)
            {
                // Return 404 Not Found if the user does not exist
                return NotFound(new { message = $"User with ID {id} was not found." });
            }

            // Transform and return the user data if found
            var auth0User = new UserDto.Auth0User
            (
                user.Email,
                user.FirstName,
                user.LastName,
                user.Blocked ?? false
            );

            return Ok(auth0User);
        }
        catch (ApiException ex)
        {
            // Handle specific Auth0 API exceptions
            return StatusCode(503, new { message = "Auth0 service is unavailable. Please try again later.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors
            return StatusCode(500, new { message = "An unexpected error occurred while fetching the user.", detail = ex.Message });
        }
    }


    private async Task<UserDto.RegistrationUser> RegisterUserAuth0(UserDto.RegistrationUser user)
    {
        var userCreateRequest = new UserCreateRequest
        {
            Email = user.Email,
            Password = user.Password,
            Connection = "Username-Password-Authentication",
            FirstName = user.FirstName,
            LastName = user.LastName,
        };
        try
        {
            // Check if the user already exists in Auth0
            var usersWithEmail = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest { Query = $"email:\"{user.Email}\"" });
            if (usersWithEmail.Any())
            {
                throw new UserAlreadyExistsException("UserAlreadyExists"); // Localization key
            }

            var response = await _managementApiClient.Users.CreateAsync(userCreateRequest);
            Console.WriteLine("Created user: " + response.UserId);
            return new UserDto.RegistrationUser(response.FirstName, response.LastName, response.Email, user.PhoneNumber, null, response.UserId, user.Address, user.BirthDate);

        }
        catch (UserAlreadyExistsException)
        {
            throw;
        }
        catch (ApiException ex)
        {
            throw new ExternalServiceException("ExternalServiceUnavailable", ex);
        }
        catch (Exception ex)
        {
            throw new ExternalServiceException("UnexpectedErrorOccurred", ex);
        }
    }

    /// <summary>
    /// Checks if an email is already taken asynchronously.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns><c>true</c> if the email exists; otherwise, <c>false</c>.</returns>
    [HttpGet("exists")]
    [AllowAnonymous] // Allows anyone to call this method
    public async Task<IActionResult> IsEmailTaken([FromQuery] string email)
    {
        try
        {
            // Check if any user exists with this email in Auth0
            var usersWithEmail = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest { Query = $"email:\"{email}\"" });
            bool isTaken = usersWithEmail.Any();
            Console.WriteLine("Email taken: " + isTaken);

            return Ok(isTaken);
        }
        catch (ApiException ex)
        {
            // Handle specific Auth0 API exceptions
            return StatusCode(503, new { message = "Auth0 service is unavailable. Please try again later.", detail = ex.Message });
        }
        catch (Exception ex)
        {
            // Handle any other unexpected errors
            return StatusCode(500, new { message = "An unexpected error occurred while checking the email.", detail = ex.Message });
        }
    }
}
