using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rise.Domain.Bookings;
using Rise.Persistence;
using Rise.Services.Bookings;
using Rise.Shared.Enums;

public class BookingServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BookingService _bookingService;
    public BookingServiceTests()
    {
        
        // Set up the in-memory database for ApplicationDbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _bookingService = new BookingService(_dbContext);
    }

    [Fact]
    public async Task GetTakenTimeslotsInDateRange_NullStartDate_ThrowsArgumentNullException()
    {
        DateTime? endDate = DateTime.Now.AddDays(10);
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingService.GetTakenTimeslotsInDateRange(null, endDate));
    }

    [Fact]
    public async Task GetTakenTimeslotsInDateRange_NullEndDate_ThrowsArgumentNullException()
    {
        DateTime? startDate = DateTime.Now.AddDays(10);
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingService.GetTakenTimeslotsInDateRange(startDate, null));
    }

    [Fact]
    public async Task GetTakenTimeslotsInDateRange_StartDateAfterEndDate_ThrowsArgumentException()
    {
        DateTime? startDate = DateTime.Now.AddDays(1);
        DateTime? endDate = DateTime.Now;
        await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.GetTakenTimeslotsInDateRange(startDate, endDate));
    }

    [Fact]
    public async Task GetTakenTimeslotsInDateRange_StartDateEqualToEndDate_ThrowsArgumentException()
    {
        DateTime? startDate = DateTime.Now;
        DateTime? endDate = startDate;
        await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.GetTakenTimeslotsInDateRange(startDate, endDate));
    }

    [Fact]
    public async Task GetTakenTimeslotsInDateRange_NoBookings_ReturnsEmptyList()
    {
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(5);
        
        var result = await _bookingService.GetTakenTimeslotsInDateRange(startDate, endDate);
        
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetTakenTimeslotsInDateRange_WithBookings_ReturnsCorrectTimeslots()
    {
        const int END_DATE_EXTRA_DAYS = 5;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(END_DATE_EXTRA_DAYS);

        // Add bookings that should return to the in-memory database
        _dbContext.Bookings.Add(new Booking(startDate.AddDays(END_DATE_EXTRA_DAYS - 1), "1", TimeSlot.Morning ));
        _dbContext.Bookings.Add(new Booking( startDate.AddDays(END_DATE_EXTRA_DAYS - 2), "1", TimeSlot.Afternoon));
        // Add bookings that should not return
        _dbContext.Bookings.Add(new Booking(startDate.AddDays(END_DATE_EXTRA_DAYS + 1), "1", TimeSlot.Evening ));
        _dbContext.Bookings.Add(new Booking( startDate.AddDays(END_DATE_EXTRA_DAYS + 5), "1", TimeSlot.Morning ));
    

        await _dbContext.SaveChangesAsync();

        var result = await _bookingService.GetTakenTimeslotsInDateRange(startDate, endDate);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.BookingDate == startDate.AddDays(END_DATE_EXTRA_DAYS - 1) && b.TimeSlot == TimeSlot.Morning);
        Assert.Contains(result, b => b.BookingDate == startDate.AddDays(END_DATE_EXTRA_DAYS - 2) && b.TimeSlot == TimeSlot.Afternoon);
    }

    [Fact]
    public async Task GetFreeTimeslotsInDateRange_NullStartDate_ThrowsArgumentNullException()
    {
        DateTime? endDate = DateTime.Now.AddDays(3);
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingService.GetFreeTimeslotsInDateRange(null, endDate));
    }

    [Fact]
    public async Task GetFreeTimeslotsInDateRange_NullEndDate_ThrowsArgumentNullException()
    {
        DateTime? startDate = DateTime.Now;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bookingService.GetFreeTimeslotsInDateRange(startDate, null));
    }

    [Fact]
    public async Task GetFreeTimeslotsInDateRange_StartDateAfterEndDate_ThrowsArgumentException()
    {
        DateTime? startDate = DateTime.Now.AddDays(5);
        DateTime? endDate = DateTime.Now;
        await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.GetFreeTimeslotsInDateRange(startDate, endDate));
    }

    [Fact]
    public async Task GetFreeTimeslotsInDateRange_StartDateEqualToEndDate_ThrowsArgumentException()
    {
        DateTime? startDate = DateTime.Now.AddDays(5);
        DateTime? endDate = startDate;
        await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.GetFreeTimeslotsInDateRange(startDate, endDate));
    }

    [Fact]
    public async Task GetFreeTimeslotsInDateRange_EndDateWithinNonBookablePeriod_ReturnsEmptyList()
    {
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(2); // ToDo -> if we move the blockdate to somewhere else get it there

         // Add bookings that should not get return to the in-memory database
        _dbContext.Bookings.Add(new Booking( startDate.AddDays(1), "1", TimeSlot.Morning ));
        _dbContext.Bookings.Add(new Booking(startDate.AddDays(2), "1", TimeSlot.Afternoon));

        var result = await _bookingService.GetFreeTimeslotsInDateRange(startDate, endDate);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFreeTimeslotsInDateRange_WithBookings_ReturnsAvailableTimeslots()
    {
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now.AddDays(5);

        _dbContext.Bookings.Add(new Booking ( DateTime.Now.AddDays(3), "1", TimeSlot.Morning) );
        _dbContext.Bookings.Add(new Booking ( DateTime.Now.AddDays(4), "1", TimeSlot.Afternoon) ); 

        var result = await _bookingService.GetFreeTimeslotsInDateRange(startDate, endDate);

        Assert.NotNull(result);
        Assert.DoesNotContain(result, r => r.BookingDate == DateTime.Now.AddDays(3) && r.TimeSlot == TimeSlot.Morning);
        Assert.DoesNotContain(result, r => r.BookingDate == DateTime.Now.AddDays(4) && r.TimeSlot == TimeSlot.Afternoon);
    }
}
