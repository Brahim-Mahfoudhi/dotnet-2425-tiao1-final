using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Users;
using Rise.Domain.Users;

namespace Rise.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<List<UserDto.GetUser>> GetAllAsync()
    {
        IQueryable<UserDto.GetUser> query = dbContext.Users
            .Select(x => new UserDto.GetUser
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
        })
            .Where(x => x.IsDeleted == false);

        return query.ToListAsync();
    }

    public async Task<UserDto.GetUser?> GetUserAsync()
    {
        IQueryable<UserDto.GetUser> query = dbContext.Users
            .Select(x => new UserDto.GetUser
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
        })
            .Where(x => x.IsDeleted == false);

        var user = await query.FirstOrDefaultAsync();

        return user;
    }



    public async Task<UserDto.GetUser?> GetUserByIdAsync(int id)
    {
        IQueryable<UserDto.GetUser> query = dbContext.Users
            .Select(x => new UserDto.GetUser
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Roles = x.Roles.Select(r => new RoleDto
            {
                Name = (Shared.Enums.RolesEnum)r.Name
            }).ToList()
        })
            .Where(x => x.Id == id && x.IsDeleted == false);

        var user = await query.FirstOrDefaultAsync();

        return user;
    }

    public async Task<UserDto.GetUserDetails?> GetUserDetailsByIdAsync(int id)
    {
        IQueryable<UserDto.GetUserDetails> query = dbContext.Users
            .Select(x => new UserDto.GetUserDetails
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            BirthDate = x.BirthDate,
            Address = new AddressDto.GetAdress
            {
                Street = x.Address.Street,
                HouseNumber = x.Address.HouseNumber,
                Bus = x.Address.Bus
            },
            Roles = x.Roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = (Shared.Enums.RolesEnum)r.Name
            }).ToList(),
            PhoneNumber = x.PhoneNumber
        })
            .Where(x => x.Id == id && x.IsDeleted == false);

        var user = await query.FirstOrDefaultAsync();

        return user;
    }


    public async Task<bool> CreateUserAsync(UserDto.CreateUser userDetails)
    {
        var adress = new Address
        (
            userDetails.Address.Street,
            userDetails.Address.HouseNumber,
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

        dbContext.Users.Add(entity);
        int response = await dbContext.SaveChangesAsync();

        return response > 0;

    }

    public async Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails)
    {
        var entity = await dbContext.Users.FindAsync(userDetails.Id) ?? throw new Exception("User not found");
        
        var adress = new Address
        (
            userDetails.Address.Street ?? entity.Address.Street,
            userDetails.Address.HouseNumber ?? entity.Address.HouseNumber,
            userDetails.Address.Bus ?? entity.Address.Bus
        );
        
        entity.FirstName = userDetails.FirstName ?? entity.FirstName;
        entity.LastName = userDetails.LastName ?? entity.LastName;
        entity.Email = userDetails.Email ?? entity.Email;
        entity.BirthDate = userDetails.BirthDate ?? entity.BirthDate;
        entity.Address = adress;
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

}
