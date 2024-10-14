using System;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Rise.Shared.Users;

namespace Rise.Services.Users;

public class UserAuthService(HttpClient httpClient) : IUserAuthService
{
    public async Task<IEnumerable<UserAuthDto.Index>> GetUsersAsync()
    { 
        var users = await httpClient.GetFromJsonAsync<IEnumerable<UserAuthDto.Index>>("UserAuth");
        return users!;
    }
}