using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;
namespace Rise.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UserAuthController : ControllerBase
{
    private readonly IManagementApiClient _managementApiClient;
    public UserAuthController(IManagementApiClient managementApiClient)
    {
        _managementApiClient = managementApiClient;
    }
    [HttpGet]
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

    [HttpGet("user")]
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

    [HttpPost]
    public async Task<UserDto.UserTable> CreateUser(UserDto.UserBase userBase)
    {
        throw new NotImplementedException();
    }
}