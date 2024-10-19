// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using System.Linq;
// using Rise.Shared.Enums;
// using Rise.Shared.Users;

// namespace Rise.Client.Users;

// public class FakeUserService : IUserService
// {
//     readonly List<RoleDto> roles = [new() { Id = 2, Name = RolesEnum.User }];
//     public Task<bool> CreateUserAsync(UserDto.CreateUser userDetails)
//     {
//         return Task.FromResult(true);
//     }

//     public Task<bool> DeleteUserAsync(int id)
//     {
//         return Task.FromResult(true);
//     }

//     public Task<IEnumerable<UserDto.GetUser>> GetAllAsync()
//     {
//         var users = Enumerable.Range(1, 5).Select(i => new UserDto.GetUser { Id = i, FirstName = $"First {i}", LastName = $"Last {i}", Email = $"test{i}@test.com", Roles = roles, IsDeleted = false });
//         return Task.FromResult(users);
//     }

//     public Task<UserDto.GetUser?> GetUserAsync()
//     {
//         var user = new UserDto.GetUser { Id = 1, FirstName = "First", LastName = "Last", Email = "test@test.com", Roles = roles, IsDeleted = false };
//         return Task.FromResult(user ?? null);
//     }

//     public Task<UserDto.GetUser?> GetUserByIdAsync(int id)
//     {
//         var user = new UserDto.GetUser { Id = id, FirstName = $"First {id}", LastName = $"Last {id}", Email = $"test{id}@test.com", Roles = roles, IsDeleted = false };
//         return Task.FromResult(user ?? null);
//     }

//     public Task<UserDto.GetUserDetails?> GetUserDetailsByIdAsync(int id)
//     {
//         var userDetails = new UserDto.GetUserDetails { Id = id, FirstName = $"First {id}", LastName = $"Last {id}", Email = $"test{id}@test.com", BirthDate = DateTime.Now, PhoneNumber = "1234567890", Address = new AddressDto.GetAdress { Street = StreetEnum.AFRIKALAAN, HouseNumber = "1" }, Roles = roles, IsDeleted = false };
//         return Task.FromResult(userDetails ?? null);
//     }

//     public Task<bool> UpdateUserAsync(int id, UserDto.UpdateUser userDetails)
//     {
//         return Task.FromResult(true);
//     }
// }
