using Rise.Shared.Bookings;
using Rise.Shared.Users;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;

namespace Rise.Shared.Services;

public class ValidationService
{
    private readonly ApplicationDbContext _dbContext;

    public ValidationService(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    public async Task<bool> CheckUserExistsAsync(string userId)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.IsDeleted == false && u.Id == userId);
    }
    
    public async Task<bool> BookingExists(DateTime bookingDate)
    {
        return await _dbContext.Bookings
            .AnyAsync(x => x.IsDeleted == false && x.BookingDate == bookingDate);
    }

    public async Task<bool> ValidateBookingAsync(string userId, BookingDto.UpdateBooking booking)
    {
        var userExists = await CheckUserExistsAsync(userId);
        if (!userExists)
        {
            throw new ArgumentException("User does not exist.");
        }

        // Further booking validation logic (e.g., check if a time slot is available)
        return true;
    }
}