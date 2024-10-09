using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpGet]
    public async Task<UserDto?> Get()
    {
        var user = await userService.GetUserAsync();
        return user;
    }

    [HttpGet("{id}")]
    public async Task<UserDto?> Get(int id)
    {
        var user = await userService.GetUserByIdAsync(id);
        return user;
    }

    [HttpGet("details/{id}")]
    public async Task<UserDto?> GetDetails(int id)
    {
        var user = await userService.GetUserDetailsAsync(id);
        return user;
    }

    [HttpPost]
    public async Task<UserDto?> Post(UserDto user)
    {
        var createdUser = await userService.CreateUserAsync(user);
        return createdUser;
    }

    [HttpPut]
    public async Task<bool> Put(UserDto user)
    {
        var updated = await userService.UpdateUserAsync(user);
        return updated;
    }

    [HttpPut("{id}")]
    public async Task<bool> Put(int id)
    {
        var deleted = await userService.DeleteUserAsync(id);
        return deleted;
    }

}