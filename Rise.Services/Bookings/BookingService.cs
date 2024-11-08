using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rise.Domain.Bookings;
using Rise.Persistence;
using Rise.Shared.Bookings;
using Rise.Shared.Enums;

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
            .Include(b => b.Id)
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
            timeSlot: booking.timeSlot,
            bookingDate: booking.bookingDate,
            userId: booking.userId
        );
        _dbContext.Bookings.Add(entity);
        int response = await _dbContext.SaveChangesAsync();

        return response > 0;
    }

    public async Task<bool> UpdateBookingAsync(BookingDto.UpdateBooking booking)
    {
        var entity = await _dbContext.Bookings.FindAsync(booking.bookingId) ?? throw new Exception("Booking not found");

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
            bookingId = booking.Id,
            bookingDate = booking.BookingDate,
            battery = battery,
            boat = boat
        };
    }

    public async Task<IEnumerable<BookingDto.ViewBookingCalender>?> GetTakenTimeslotsInDateRange(DateTime? startDate,
        DateTime? endDate)
    {
        // Check validity of parameters
        if (startDate == null)
        {
            throw new ArgumentNullException(nameof(startDate));
        }

        if (endDate == null)
        {
            throw new ArgumentNullException(nameof(endDate));
        }

        // Validate the date range
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be before the end date.", nameof(startDate));
        }

        // Get all bookings from the db that are between start and enddate (limits included)
        List<Booking> bookings = await _dbContext.Bookings
            .Where(booking => booking.BookingDate.Date >= startDate && booking.BookingDate.Date <= endDate)
            .ToListAsync();

        return bookings.Select(booking => ViewBookingCalenderFromBooking(booking, true)).ToList();
    }

    public async Task<IEnumerable<BookingDto.ViewBookingCalender>?> GetFreeTimeslotsInDateRange(DateTime? startDate,
        DateTime? endDate)
    {
        int NON_BOOKABLE_DAYS_AFTER_TODAY = 2;

        // Check validity of parameters
        if (startDate == null)
        {
            throw new ArgumentNullException(nameof(startDate));
        }

        if (endDate == null)
        {
            throw new ArgumentNullException(nameof(endDate));
        }
       
        // Validate the date range
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date must be before the end date.", nameof(startDate));
        }

        // if there is no possible free timeslot. (cannot book before or on the enddate) return empty list (shortcut)
        if (endDate <= DateTime.Now.AddDays(NON_BOOKABLE_DAYS_AFTER_TODAY))
        {
            return new List<BookingDto.ViewBookingCalender>();
        }

        // Get all bookings from the db that are between startdate and enddate (limits included)
        List<Booking> bookings = await _dbContext.Bookings
            .Where(booking => booking.BookingDate.Date >= startDate && booking.BookingDate.Date <= endDate)
            .ToListAsync();

        //generate all possible timeslots
        List<BookingDto.ViewBookingCalender> allPossibleTimeslotsInRange = new List<BookingDto.ViewBookingCalender>();
        for (DateTime date = startDate.Value; date <= endDate; date = date.AddDays(1))
        {
            if (date <= DateTime.Now.AddDays(NON_BOOKABLE_DAYS_AFTER_TODAY))
            {
                continue;
            }

            foreach (TimeSlot timeSlot in Enum.GetValues(typeof(TimeSlot)))
            {
                if (timeSlot == TimeSlot.None)
                {
                    continue;
                }

                allPossibleTimeslotsInRange.Add(
                    new BookingDto.ViewBookingCalender
                    {
                        BookingDate = date,
                        TimeSlot = timeSlot,
                        Available = true
                    }
                );
            }
        }

        // return all possible timeslots without the occupied ones
        return allPossibleTimeslotsInRange
            .Where(slot => !bookings.Exists(booking =>
                booking.BookingDate.Date == slot.BookingDate.Date && booking.TimeSlot == slot.TimeSlot))
            .ToList();
    }

    private static BookingDto.ViewBookingCalender ViewBookingCalenderFromBooking(Booking booking, bool Available)
    {
        return new BookingDto.ViewBookingCalender
        {
            BookingDate = booking.BookingDate,
            TimeSlot = booking.TimeSlot,
            Available = Available
        };
    }
}