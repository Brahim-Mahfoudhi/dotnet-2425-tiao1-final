using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Users;
using Rise.Domain.Users;
using Rise.Shared.Enums;

namespace Rise.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<UserDto.UserBase>> GetAllAsync()
    {
        IQueryable<UserDto.UserBase> query = dbContext.Users
            .Where(x => x.IsDeleted == false)
            .Select(x => new UserDto.UserBase
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = x.Roles.Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = (Shared.Enums.RolesEnum)r.Name
                    }).ToList()
                });

        var users = await query.ToListAsync();
        return users;
    }

    public async Task<UserDto.UserBase?> GetUserAsync()
    {
        IQueryable<UserDto.UserBase> query = dbContext.Users
            .Where(x => x.IsDeleted == false)
            .Select(x => new UserDto.UserBase
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = x.Roles.Select(r => new RoleDto
                    {
                        Id = r.Id,
                        Name = (Shared.Enums.RolesEnum)r.Name
                    }).ToList()
                });

        var user = await query.FirstOrDefaultAsync();

        return user;
    }



    public async Task<UserDto.UserBase?> GetUserByIdAsync(int id)
    {
        IQueryable<UserDto.UserBase> query = dbContext.Users
            .Where(x => x.Id == id && x.IsDeleted == false)
            .Select(x => new UserDto.UserBase
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = x.Roles.Select(r => new RoleDto
                    {
                        Name = (Shared.Enums.RolesEnum)r.Name
                    }).ToList()
                });

        var user = await query.FirstOrDefaultAsync();

        return user;
    }

    public async Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(int id)
    {
        IQueryable<UserDto.UserDetails> query = dbContext.Users
            .Where(x => x.Id == id && x.IsDeleted == false)
            .Select(x => new UserDto.UserDetails
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                BirthDate = x.BirthDate,
                Address = new AddressDto.GetAdress
                {
                    Street = StreetEnumExtensions.GetStreetEnum(x.Address.Street),
                    HouseNumber = x.Address.HouseNumber,
                    Bus = x.Address.Bus
                },
                Roles = x.Roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = (Shared.Enums.RolesEnum)r.Name
                }).ToList(),
                PhoneNumber = x.PhoneNumber
            });

        var user = await query.FirstOrDefaultAsync();

        return user;
    }


    public async Task<bool> CreateUserAsync(UserDto.CreateUser userDetails)
    {
        var adress = new Address
        (
            StreetEnumExtensions.GetStreetName(userDetails.Address.Street),
            userDetails.Address.HouseNumber ?? "",
            userDetails.Address.Bus
        );

        var entity = new User(
            firstName: userDetails.FirstName,
            lastName: userDetails.LastName,
            email: userDetails.Email,
            password: userDetails.Password,
            birthDate: userDetails.BirthDate,
            address: adress,
            phoneNumber: userDetails.PhoneNumber
        );
        entity.AddRole(new Role(RolesEnum.Pending));

        dbContext.Users.Add(entity);
        int response = await dbContext.SaveChangesAsync();

        return response > 0;

    }

    public async Task<bool> UpdateUserAsync(int id, UserDto.UpdateUser userDetails)
    {
        var entity = await dbContext.Users.FindAsync(id) ?? throw new Exception("User not found");

        entity.FirstName = userDetails.FirstName ?? entity.FirstName;
        entity.LastName = userDetails.LastName ?? entity.LastName;
        entity.Email = userDetails.Email ?? entity.Email;
        entity.BirthDate = userDetails.BirthDate ?? entity.BirthDate;
        entity.PhoneNumber = userDetails.PhoneNumber ?? entity.PhoneNumber;

        dbContext.Users.Update(entity);
        int response = await dbContext.SaveChangesAsync();

        return response > 0;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var entity = await dbContext.Users.FindAsync(id) ?? throw new Exception("User not found");

        entity.SoftDelete();
        dbContext.Users.Update(entity);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public Task<IEnumerable<UserDto.UserTable>> GetUsersTableAsync()
    {
        throw new NotImplementedException();
    }
}
