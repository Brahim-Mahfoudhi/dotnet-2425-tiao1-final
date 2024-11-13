using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rise.Domain.Bookings;
using Rise.Persistence;
using Rise.Server.Settings;
using Rise.Shared.Bookings;
using Rise.Shared.Enums;
using Rise.Services.Users;
using Rise.Shared.Services;

namespace Rise.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly int _minReservationDays;
    private readonly int _maxReservationDays;
    private readonly ValidationService _validationService;


    public BookingService(ApplicationDbContext dbContext, IOptions<BookingSettings> options,
        ValidationService validationService)
    {
        _minReservationDays = options.Value.MinReservationDays;
        _maxReservationDays = options.Value.MaxReservationDays;
        _validationService = validationService;

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

    public async Task<BookingDto.ViewBooking> CreateBookingAsync(BookingDto.NewBooking booking)
    {
        if (!await _validationService.CheckUserExistsAsync(booking.userId))
        {
            throw new UserNotFoundException("Invalid user");
        }

        //Check if user has not reached the maximum allowed bookings
        if (await _validationService.CheckUserMaxBookings(booking.userId))
        {
            throw new InvalidOperationException("You have reached the maximum number of bookings allowed.");
        }

        //Check if the requested booking is still available
        if (await _validationService.BookingExists(booking.bookingDate))
        {
            throw new InvalidDataException("Booking already exists on this date");
        }

        //Check if requested booking is within the set timerange
        if (!CheckWithinDateRange(booking.bookingDate))
        {
            throw new InvalidDataException(
                $"Invalid date selection. Please choose a date that is at least {_minReservationDays} days from " +
                $"today and no more than {_maxReservationDays} days ahead.");
        }

        var entity = new Booking(
            timeSlot: TimeSlotEnumExtensions.ToTimeSlot(booking.bookingDate.Hour),
            bookingDate: booking.bookingDate,
            userId: booking.userId
        );
        var created = _dbContext.Bookings.Add(entity);
        int response = await _dbContext.SaveChangesAsync();

        return MapToDto(created.Entity);
    }

    public async Task<bool> UpdateBookingAsync(BookingDto.UpdateBooking booking)
    {
        var entity = await _dbContext.Bookings.FindAsync(booking.bookingId) ?? throw new Exception("Booking not found");

        if (booking.bookingDate != null && booking.bookingDate != entity.BookingDate)
        {
            if (await _validationService.BookingExists(booking.bookingDate.Value))
            {
                throw new InvalidOperationException("Booking already exists on this date");
            }

            entity.BookingDate = booking.bookingDate.Value;
        }

        /*entity.Boat = booking.boat ?? entity.Boat;
        entity.Battery = booking.battery ?? entity.Battery;

        _dbContext.Users.Update(entity);*/
        int response = await _dbContext.SaveChangesAsync();

        return response > 0;
    }

    public async Task<bool> DeleteBookingAsync(string bookingId)
    {
        var entity = await _dbContext.Bookings.FindAsync(bookingId) ?? throw new Exception("Booking not found");

        entity.SoftDelete();
        _dbContext.Bookings.Update(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllUserBookings(string userId)
    {
        if (!await _validationService.CheckUserExistsAsync(userId))
        {
            throw new UserNotFoundException("Invalid user");
        }
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Bookings
            .Include(b => b.Battery)
            .Include(b => b.Boat)
            .Where(x => x.UserId.Equals(userId) && x.IsDeleted == false)
            .OrderByDescending(x => x.BookingDate)
            .ToListAsync();

        if (query == null)
        {
            return null;
        }

        return query.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetFutureUserBookings(string userId)
    {
        if (!await _validationService.CheckUserExistsAsync(userId))
        {
            throw new UserNotFoundException("Invalid user");
        }
        // Changed method so that DTO creation is out of the LINQ Query
        // You need to avoid using methods with optional parameters directly
        // in the LINQ query that EF is trying to translate
        var query = await _dbContext.Bookings
            .Include(x => x.Battery)
            .Include(x => x.Boat)
            .Where(x => x.UserId.Equals(userId) && x.IsDeleted == false && x.BookingDate >= DateTime.Now)
            .OrderByDescending(x => x.BookingDate)
            .ToListAsync();

        if (query == null)
        {
            return null;
        }

        return query.Select(MapToDto);
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetPastUserBookings(string userId)
    {
        if (!await _validationService.CheckUserExistsAsync(userId))
        {
            throw new UserNotFoundException("Invalid user");
        }
        var query = await _dbContext.Bookings
            .Include(x => x.Battery)
            .Include(x => x.Boat)
            .Where(x => x.UserId.Equals(userId) && x.IsDeleted == false && x.BookingDate < DateTime.Now)
            .OrderByDescending(x => x.BookingDate)
            .ToListAsync();

        if (query == null)
        {
            return null;
        }

        return query.Select(MapToDto);
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
            boat = boat,
            timeSlot = TimeSlotEnumExtensions.ToTimeSlot(booking.BookingDate.Hour),
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
            .Where(booking => booking.BookingDate.Date >= startDate && booking.BookingDate.Date <= endDate &&
                              booking.IsDeleted == false)
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
            .Where(booking => booking.BookingDate.Date >= startDate && booking.BookingDate.Date <= endDate &&
                              booking.IsDeleted == false)
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
                booking.BookingDate.Date == slot.BookingDate.Date && booking.GetTimeSlot() == slot.TimeSlot))
            .ToList();
    }

    private static BookingDto.ViewBookingCalender ViewBookingCalenderFromBooking(Booking booking, bool Available)
    {
        return new BookingDto.ViewBookingCalender
        {
            BookingDate = booking.BookingDate,
            TimeSlot = booking.GetTimeSlot(),
            Available = Available
        };
    }

    private bool CheckWithinDateRange(DateTime bookingDate)
    {
        return DateTime.Now.AddDays(_minReservationDays) <= bookingDate &&
               bookingDate <= DateTime.Now.AddDays(_maxReservationDays);
    }
}