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

    public Task<List<UserDto>> GetAllAsync()
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

        return query.ToListAsync();
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

    public async Task<UserDetailsDto?> GetUserDetailsByIdAsync(int id)
    {
        IQueryable<UserDetailsDto> query = dbContext.Users.Where(x => x.Id == id).Select(x => new UserDetailsDto
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


    public async Task<bool> CreateUserAsync(UserDetailsDto userDetails)
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

    public async Task<bool> UpdateUserAsync(UserDetailsDto userDetails)
    {
        var adress = new Address
        (
            userDetails.Address.Street,
            userDetails.Address.HouseNumber,
            userDetails.Address.Bus
        );

        var entity = await dbContext.Users.FindAsync(userDetails.Id) ?? throw new Exception("User not found");
        entity.FirstName = userDetails.FirstName;
        entity.LastName = userDetails.LastName;
        entity.Email = userDetails.Email;
        entity.BirthDate = userDetails.BirthDate;
        entity.Address = adress;
        entity.PhoneNumber = userDetails.PhoneNumber;

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