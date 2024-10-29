using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Bookings;
using Rise.Persistence;
using Rise.Shared.Bookings;

namespace Rise.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public BookingService(ApplicationDbContext dbContext)
    {
        this._dbContext = dbContext;
        this._jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>> GetAllAsync()
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Bookings
            .Include(b => b.Battery)
            .Include(b => b.Boat)
            .Where(x => x.IsDeleted == false)
            .ToListAsync();

        if (query == null)
        {
            return null;
        }

        return query.Select(MapToDto);
    }

    public async Task<BookingDto.ViewBooking> GetBookingById(string id)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Bookings
            .Include(x => x.Battery)
            .Include(x => x.Boat)
            .FirstOrDefaultAsync(x => x.Id.Equals(id) && x.IsDeleted == false);

        if (query == null)
        {
            return null;
        }

        return MapToDto(query);
    }

    public async Task<bool> CreateBookingAsync(BookingDto.NewBooking booking)
    {
        var entity = new Booking(
            countAdults: booking.countAdults,
            countChildren: booking.countChildren,
            bookingDate: booking.bookingDate,
            userId: booking.userId
        );
        _dbContext.Bookings.Add(entity);
        int response = await _dbContext.SaveChangesAsync();

        return response > 0;
    }

    public async Task<bool> UpdateBookingAsync(BookingDto.UpdateBooking booking)
    {
        var entity = await _dbContext.Bookings.FindAsync(booking.id) ?? throw new Exception("Booking not found");

        entity.CountAdults = booking.countAdults ?? entity.CountAdults;
        entity.CountChildren = booking.countChildren ?? entity.CountChildren;
        entity.BookingDate = booking.bookingDate ?? entity.BookingDate;
        /*entity.Boat = booking.boat ?? entity.Boat;
        entity.Battery = booking.battery ?? entity.Battery;

        _dbContext.Users.Update(entity);*/
        int response = await _dbContext.SaveChangesAsync();

        return response > 0;
    }

    public Task<bool> DeleteBookingAsync(string id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllUserBookings(string userId)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Bookings
            .Include(b => b.Battery)
            .Include(b => b.Boat)
            .Where(x => x.UserId.Equals(userId) && x.IsDeleted == false)
            .ToListAsync();

        if (query == null)
        {
            return null;
        }

        return query.Select(MapToDto);
    }

    public async Task<BookingDto.ViewBooking?> GetFutureUserBooking(string userId)
    {
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        Console.WriteLine(userId);
        
        var query = await _dbContext.Bookings
            .Include(x => x.Battery)
            .Include(x => x.Boat)
            .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.IsDeleted == false && x.BookingDate > DateTime.Now);

        if (query == null)
        {
            return null;
        }

        return MapToDto(query);
    }

    private BookingDto.ViewBooking MapToDto(Booking booking)
    {
        var battery = new BatteryDto.ViewBattery();
        if (booking.Battery != null)
        {
            battery = new BatteryDto.ViewBattery()
            {
                name = booking.Battery.Name,
                countBookings = booking.Battery.CountBookings,
                listComments = booking.Battery.ListComments,
            };
        }

        var boat = new BoatDto.ViewBoat();
        if (booking.Boat != null)
        {
            boat = new BoatDto.ViewBoat()
            {
                name = booking.Boat.Name,
                countBookings = booking.Boat.CountBookings,
                listComments = booking.Boat.ListComments,
            };
        }

        return new BookingDto.ViewBooking()
        {
            countChildren = booking.CountChildren,
            countAdults = booking.CountAdults,
            bookingDate = booking.BookingDate,
            battery = battery,
            boat = boat
        };
    }
}