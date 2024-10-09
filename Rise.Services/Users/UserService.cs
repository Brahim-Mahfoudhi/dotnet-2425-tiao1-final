using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Users;
using Rise.Domain.Users;
using Microsoft.VisualBasic;

namespace Rise.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<UserDto?> GetUserAsync()
    {
        IQueryable<UserDto> query = dbContext.Users.Select(x => new UserDto
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



    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        IQueryable<UserDto> query = dbContext.Users.Where(x => x.Id == id).Select(x => new UserDto
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

    public async Task<UserDto?> GetUserDetailsAsync(int id)
    {
        IQueryable<UserDto> query = dbContext.Users.Where(x => x.Id == id).Select(x => new UserDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            BirthDate = x.BirthDate,
            Address = new AddressDto
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
        });

        var user = await query.FirstOrDefaultAsync();

        return user;
    }


    public async Task<UserDto?> CreateUserAsync(UserDto user)
    {
        var adress = new Address
        (
            user.Address.Street,
            user.Address.HouseNumber,
            user.Address.Bus
        );
        var entity = new User(
            firstName: user.FirstName,
            lastName: user.LastName,
            email: user.Email,
            password: user.Password,
            birthDate: user.BirthDate,
            address: adress,
            phoneNumber: user.PhoneNumber
        );

        dbContext.Users.Add(entity);
        await dbContext.SaveChangesAsync();

        user.Id = entity.Id;

        return user ?? null;
    }

    public async Task<bool> UpdateUserAsync(UserDto user)
    {
        var adress = new Address
        (
            user.Address.Street,
            user.Address.HouseNumber,
            user.Address.Bus
        );
        var entity = await dbContext.Users.FindAsync(user.Id) ?? throw new Exception("User not found");
        entity.FirstName = user.FirstName;
        entity.LastName = user.LastName;
        entity.Email = user.Email;
        entity.BirthDate = user.BirthDate;
        entity.Address = adress;
        entity.PhoneNumber = user.PhoneNumber;

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