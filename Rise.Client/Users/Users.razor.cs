using System;
using Microsoft.AspNetCore.Components;
using Rise.Shared.Users;

namespace Rise.Client.Users;

public partial class Users
{
    private IEnumerable<UserDto.UserBase>? users;
    private UserDto.UserBase? user;

    [Inject] public required IUserService UserService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        user = await UserService.GetUserByIdAsync("1");
        users = await UserService.GetAllAsync();
    }
}
