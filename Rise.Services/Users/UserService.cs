using System.Collections.Immutable;
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
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await dbContext.Users
            .Include(x => x.Roles) // Ensure Roles are loaded (Eagerly loading)
            .Where(x => x.IsDeleted == false)
            .ToListAsync();
        
        if (query == null)
        {
            return null;
        }

        var users = query.Select(x => MapToUserBase(x));
        
        return users;
    }

    public async Task<UserDto.UserBase?> GetUserAsync()
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await dbContext.Users
            .Include(x => x.Roles) // Ensure Roles are loaded (Eagerly loading)
            .FirstOrDefaultAsync(x => x.IsDeleted == false);
        
        if (query == null)
        {
            return null;
        }
        
        return MapToUserBase(query);
    }



    public async Task<UserDto.UserBase?> GetUserByIdAsync(int id)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await dbContext.Users
            .Include(x => x.Roles) // Ensure Roles are loaded (Eagerly loading)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        
        if (query == null)
        {
            return null;
        }
        
        return MapToUserBase(query);
    }

    public async Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(int id)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await dbContext.Users
            .Include(x => x.Address) // Ensure Address is loaded (Eagerly loading)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        
        if (query == null)
        {
            return null;
        }
        
        var user = new UserDto.UserDetails(
            query.Id, 
            query.BirthDate,
            new AddressDto.GetAdress
            {
                Street = StreetEnumExtensions.GetStreetEnum(query.Address.Street),
                HouseNumber = query.Address.HouseNumber,
                Bus = query.Address.Bus
            },
            query.PhoneNumber,
            query.FirstName,
            query.LastName,
            query.Email
            );

        return user;
    }


    public async Task<bool> CreateUserAsync(UserDto.RegistrationUser userDetails)
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
    
    private UserDto.UserBase MapToUserBase(User user)
    {
        return new UserDto.UserBase
        (
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email
        )
        {
            Roles = user.Roles.Select(r => new RoleDto
            {
                Name = (Shared.Enums.RolesEnum)r.Name
            }).ToImmutableList()
        };
    }
}
