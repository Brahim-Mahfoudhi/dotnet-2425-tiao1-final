using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.Users;

namespace Rise.Client.Users;

public partial class Users
{
    private IEnumerable<UserDto.UserBase>? users;
    private UserDto.UserBase? user;

    private string? userIdAuth0;

    [Inject] public required IUserService UserService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {

// Get the current user's authentication state
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        userIdAuth0 = authState.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

        user = await UserService.GetUserByIdAsync(userIdAuth0);
        users = await UserService.GetAllAsync();
    }
}
