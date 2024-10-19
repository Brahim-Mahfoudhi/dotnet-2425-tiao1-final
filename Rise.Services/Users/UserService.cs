using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Shared.Users;
using Rise.Domain.Users;
using Rise.Shared.Enums;

namespace Rise.Services.Users;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;


    public UserService(ApplicationDbContext dbContext, HttpClient httpClient)
    {
        this._dbContext = dbContext;
        this._httpClient = httpClient;
        this._jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        this._jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<IEnumerable<UserDto.UserBase>> GetAllAsync()
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Users
            .Include(x => x.Roles) // Ensure Roles are loaded (Eagerly loading)
            .Where(x => x.IsDeleted == false)
            .ToListAsync();
        
        if (query == null)
        {
            return null;
        }
        
        return query.Select(x => MapToUserBase(x));
    }

    public async Task<UserDto.UserBase?> GetUserAsync()
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Users
            .Include(x => x.Roles) // Ensure Roles are loaded (Eagerly loading)
            .FirstOrDefaultAsync(x => x.IsDeleted == false);
        
        if (query == null)
        {
            return null;
        }
        
        return MapToUserBase(query);
    }

    public async Task<UserDto.UserBase?> GetUserByIdAsync(string id)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Users
            .Include(x => x.Roles) // Ensure Roles are loaded (Eagerly loading)
            .FirstOrDefaultAsync(x => x.Id.Equals(id) && x.IsDeleted == false);
        
        if (query == null)
        {
            return null;
        }
        
        return MapToUserBase(query);
    }

    public async Task<UserDto.UserDetails?> GetUserDetailsByIdAsync(string id)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Users
            .Include(x => x.Address) // Ensure Address is loaded (Eagerly loading)
            .FirstOrDefaultAsync(x => x.Id.Equals(id) && x.IsDeleted == false);
        
        if (query == null)
        {
            return null;
        }
        
        return MapToUserDetails(query);
    }

    public async Task<bool> CreateUserAsync(UserDto.RegistrationUser userDetails)
    {
        Console.WriteLine("Creating new user" + userDetails);
        var entity = new User(
            id : userDetails.Id,
            firstName: userDetails.FirstName,
            lastName: userDetails.LastName,
            email: userDetails.Email,
            birthDate: userDetails.BirthDate ?? DateTime.UtcNow,
            address: new Address(
                street: userDetails.Address.Street.ToString() ?? "",
                houseNumber: userDetails.Address.HouseNumber ?? "",
                bus: userDetails.Address.Bus),
            phoneNumber: userDetails.PhoneNumber
        );
        entity.AddRole(new Role(RolesEnum.Pending));
        
        _dbContext.Users.Add(entity);
        int response = await _dbContext.SaveChangesAsync();

        return response > 0;
    }

    public async Task<bool> UpdateUserAsync(UserDto.UpdateUser userDetails)
    {
        var entity = await _dbContext.Users.FindAsync(userDetails.Id) ?? throw new Exception("User not found");

        entity.FirstName = userDetails.FirstName ?? entity.FirstName;
        entity.LastName = userDetails.LastName ?? entity.LastName;
        entity.Email = userDetails.Email ?? entity.Email;
        entity.BirthDate = userDetails.BirthDate ?? entity.BirthDate;
        entity.PhoneNumber = userDetails.PhoneNumber ?? entity.PhoneNumber;

        _dbContext.Users.Update(entity);
        int response = await _dbContext.SaveChangesAsync();

        return response > 0;
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        var entity = await _dbContext.Users.FindAsync(id) ?? throw new Exception("User not found");

        entity.SoftDelete();
        _dbContext.Users.Update(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserDto.Auth0User>> GetAuth0Users()
    {
        var users = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto.Auth0User>>("user/auth/users");
        return users!;
    }
    
    /// <summary>
    /// Maps a User to a UserBase DTO file
    /// </summary>
    /// <param name="user"></param>
    /// <returns>UserDto.UserBase</returns>
    private UserDto.UserBase MapToUserBase(User user)
    {
        return new UserDto.UserBase
            (
                user.Id, 
                user.FirstName,
                user.LastName,
                user.Email,
                ExtractRoles(user)
            );
    }
    
    /// <summary>
    /// Maps a User to a UserDetails DTO file
    /// </summary>
    /// <param name="user"></param>
    /// <returns>UserDto.UserDetails</returns>
    private UserDto.UserDetails MapToUserDetails(User user)
    {
        return new UserDto.UserDetails
            (
                user.Id, 
                user.FirstName,
                user.LastName,
                user.Email,
                ExtractAdress(user),
                ExtractRoles(user),
                user.BirthDate
            );
    }
    
    /// <summary>
    /// Extracts list of roles from a User
    /// </summary>
    /// <param name="user"></param>
    /// <returns>ImmutableList<RoleDto></returns>
    private ImmutableList<RoleDto> ExtractRoles(User user)
    {
        return user.Roles.Select(r => new RoleDto
        {
            Name = (Shared.Enums.RolesEnum)r.Name
        }).ToImmutableList();
    }

    /// <summary>
    /// Extracts AdressDTO from a User
    /// </summary>
    /// <param name="user"></param>
    /// <returns>ddressDto.GetAdress</returns>
    private AddressDto.GetAdress ExtractAdress(User user)
    {
        return new AddressDto.GetAdress
        {
            Street = StreetEnumExtensions.GetStreetEnum(user.Address.Street),
            HouseNumber = user.Address.HouseNumber,
            Bus = user.Address.Bus
        };
    }

    private Address MapToAddress(AddressDto.GetAdress adress)
    {
       return new Address
        (
            StreetEnumExtensions.GetStreetName(adress.Street),
            adress.HouseNumber ?? "",
            adress.Bus
        );
    }
}
