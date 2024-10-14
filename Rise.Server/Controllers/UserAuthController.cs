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
    public async Task<IEnumerable<UserAuthDto.Index>> GetUsers()
    {
        var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo());
        Console.WriteLine(users);
        return users.Select(x => new UserAuthDto.Index
        {
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Blocked = x.Blocked ?? false,
        });
    }
}