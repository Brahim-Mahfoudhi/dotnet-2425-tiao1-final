using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Domain.Users;
using Rise.Shared.Enums;
using Rise.Shared.Users;

namespace Rise.Client.Users;

public partial class UserValidation
{
    [Parameter] public EventCallback<string> OnUserSelected { get; set; }
    private bool _isLoading = false;
    private IEnumerable<UserDto.UserBase> _newlyRegisteredUsers = new List<UserDto.UserBase>();
    private IEnumerable<UserDto.UserBase> _usersWithUserRole = new List<UserDto.UserBase>();

    [Inject] public required IUserService UserService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }


    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        UserFilter newFilter = new UserFilter
        {
            Role = RolesEnum.Pending
        };
        UserFilter userFilter = new UserFilter
        {
            Role = RolesEnum.User
        };
        _newlyRegisteredUsers = await UserService.GetFilteredUsersAsync(newFilter);
        _usersWithUserRole = await UserService.GetFilteredUsersAsync(userFilter);
        _isLoading = false;
    }
    // protected void NavigateToUserDetails(string userId)
    // {
    //     NavigationManager.NavigateTo($"/userdetails/{userId}");
    // }
    private void HandleUserClick(string userId)
    {
        OnUserSelected.InvokeAsync(userId);
    }
}

