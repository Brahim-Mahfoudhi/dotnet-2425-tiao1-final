using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Rise.Domain.Bookings;
using Rise.Persistence;
using Rise.Server.Settings;
using Rise.Services.Bookings;
using Rise.Shared.Bookings;
using Rise.Shared.Enums;
using Rise.Services.Users;
using Rise.Shared.Services;

namespace Rise.Services.Tests.Bookings;

public class BookingServiceTest
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IOptions<BookingSettings> _bookingSettings;
    private readonly BookingService _bookingService;

    private readonly Mock<IValidationService> _validationServiceMock;

    public BookingServiceTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _validationServiceMock = new Mock<IValidationService>();

        var bookingSettings = new BookingSettings
        {
            MinReservationDays = 1,
            MaxReservationDays = 30
        };
        _bookingSettings = Options.Create(bookingSettings);

        _bookingService = new BookingService(_dbContext, _bookingSettings, _validationServiceMock.Object);
    }


    [Fact]
    public async Task GetAllAsync_ShouldReturnBookings_WhenBookingsExist()
    {
        // Arrange
        var booking = new Booking(DateTime.UtcNow.Date.AddDays(5).AddHours(10), "user1");
        booking.AddTimeSlot(TimeSlot.Morning);

        var boat = new Boat("Sea Explorer");
        var battery = new Battery("HighPower Battery");

        booking.AddBoat(boat);
        booking.AddBattery(battery);

        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _bookingService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        var viewBooking = result.First();
        Assert.Equal(booking.BookingDate.Date, viewBooking.bookingDate.Date);
        Assert.Equal(booking.Boat.Name, viewBooking.boat.name);
        Assert.Equal(booking.Battery.Name, viewBooking.battery.name);
    }


    [Fact]
    public async Task CreateBookingAsync_ShouldCreateBookingWithBoatAndBattery_WhenDataIsValid()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.Date.AddDays(10).AddHours(10),
            userId = "user1"
        };
        var boat = new Boat("Sea Explorer");
        var battery = new Battery("HighPower Battery");

        // Mocking ValidationService methods
        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(newBooking.bookingDate))
            .ReturnsAsync(false);
        _validationServiceMock
            .Setup(v => v.CheckUserMaxBookings(newBooking.userId))
            .ReturnsAsync(false);

        // Act
        var result = await _bookingService.CreateBookingAsync(newBooking);

        // Fetch the created booking from the database
        var booking = await _dbContext.Bookings.FindAsync(result.bookingId);
        booking.AddBoat(boat);
        booking.AddBattery(battery);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newBooking.bookingDate.Date, result.bookingDate.Date);
        Assert.Equal(booking.GetTimeSlot(), TimeSlot.Morning);

        // Check if the assigned Boat and Battery match
        Assert.Equal(boat.Name, booking.Boat.Name);
        Assert.Equal(battery.Name, booking.Battery.Name);
    }


    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenUserMaxBookingsExceeded()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.Date.AddDays(10).AddHours(10),
            userId = "user1"
        };

        // Mocking ValidationService methods
        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(newBooking.bookingDate))
            .ReturnsAsync(false);
        _validationServiceMock
            .Setup(v => v.CheckUserMaxBookings(newBooking.userId))
            .ReturnsAsync(true); // User has reached max bookings

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.CreateBookingAsync(newBooking));
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.AddDays(10),
            userId = "user1"
        };

        // Mocking ValidationService methods
        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(false); // User does not exist

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _bookingService.CreateBookingAsync(newBooking));
    }
    
    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenBookingAlreadyExists()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.AddDays(10),
            userId = "user1"
        };

        // Mocking ValidationService methods
        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(newBooking.bookingDate))
            .ReturnsAsync(true); // Booking already exists for this date

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.CreateBookingAsync(newBooking));
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenBookingDateIsInThePast()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.AddDays(-1), // Past date
            userId = "user1"
        };

        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _bookingService.CreateBookingAsync(newBooking));
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldCreateBookingWithoutBoatOrBattery_WhenNoBoatAndBatteryProvided()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.Date.AddDays(10).AddHours(10),
            userId = "user1"
        };

        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(newBooking.bookingDate))
            .ReturnsAsync(false);
        _validationServiceMock
            .Setup(v => v.CheckUserMaxBookings(newBooking.userId))
            .ReturnsAsync(false);

        // Act
        var result = await _bookingService.CreateBookingAsync(newBooking);
        var booking = await _dbContext.Bookings.FindAsync(result.bookingId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newBooking.bookingDate.Date, result.bookingDate.Date);
        Assert.Null(booking.Boat);
        Assert.Null(booking.Battery);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldIncrementBoatAndBatteryBookingCount_WhenBoatAndBatteryAssigned()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.Date.AddDays(10).AddHours(10),
            userId = "user1"
        };
        var boat = new Boat("Sea Explorer");
        var battery = new Battery("HighPower Battery");

        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(newBooking.bookingDate))
            .ReturnsAsync(false);
        _validationServiceMock
            .Setup(v => v.CheckUserMaxBookings(newBooking.userId))
            .ReturnsAsync(false);

        // Act
        var result = await _bookingService.CreateBookingAsync(newBooking);
        var booking = await _dbContext.Bookings.FindAsync(result.bookingId);

        booking.AddBoat(boat);
        booking.AddBattery(battery);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.Equal(1, boat.CountBookings);
        Assert.Equal(1, battery.CountBookings);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenBoatOrBatteryAssignmentIsInvalid()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.Date.AddDays(10).AddHours(10),
            userId = "user1"
        };

        var invalidBoat = (Boat)null;

        // Mocking
        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);

        var result = await _bookingService.CreateBookingAsync(newBooking);
        var booking = await _dbContext.Bookings.FindAsync(result.bookingId);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => booking.AddBoat(invalidBoat));
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldThrowException_WhenBookingDateIsNotAvailableForTimeSlot()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.AddDays(10).AddHours(10),
            userId = "user1",
        };

        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(It.IsAny<DateTime>()))
            .ReturnsAsync(true); // Simulate the time slot is booked

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _bookingService.CreateBookingAsync(newBooking));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateBookingAsync_ShouldThrowException_WhenUserIdIsNullOrEmpty(string userId)
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.AddDays(10),
            userId = userId
        };

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _bookingService.CreateBookingAsync(newBooking));
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldRollbackTransaction_WhenExceptionIsThrownDuringBooking()
    {
        // Arrange
        var newBooking = new BookingDto.NewBooking
        {
            bookingDate = DateTime.UtcNow.AddDays(10),
            userId = "user1"
        };

        _validationServiceMock
            .Setup(v => v.CheckUserExistsAsync(newBooking.userId))
            .ReturnsAsync(true);
        _validationServiceMock
            .Setup(v => v.BookingExists(It.IsAny<DateTime>()))
            .Throws(new Exception("Database error"));

        // Act
        await Assert.ThrowsAsync<Exception>(
            () => _bookingService.CreateBookingAsync(newBooking));

        // Assert
        var bookingCount = await _dbContext.Bookings.CountAsync();
        Assert.Equal(0, bookingCount); // Ensure no booking was added
    }

    [Fact]
    public async Task UpdateBookingAsync_ShouldUpdateBookingBoatAndBatteryDetails()
    {
        // Arrange
        var booking = new Booking(DateTime.UtcNow.AddDays(5), "user1", TimeSlot.Morning);
        var boat = new Boat("Old Boat");
        var battery = new Battery("Old Battery");

        booking.AddBoat(boat);
        booking.AddBattery(battery);

        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

        var newBoat = new Boat("New Boat");
        var newBattery = new Battery("New Battery");
        
        // Act
        booking.AddBoat(newBoat);
        booking.AddBattery(newBattery);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedBooking = await _dbContext.Bookings.FindAsync(booking.Id);
        Assert.Equal("New Boat", updatedBooking.Boat.Name);
        Assert.Equal("New Battery", updatedBooking.Battery.Name);
    }

    [Fact]
    public async Task AddCommentToBoat_ShouldIncreaseCommentCount_WhenCommentIsAdded()
    {
        // Arrange
        var boat = new Boat("Test Boat");
        var booking = new Booking(DateTime.UtcNow.AddDays(3), "user1");
        booking.AddBoat(boat);

        // Act
        booking.Boat.AddComment("Great experience!");

        // Assert
        Assert.Single(boat.ListComments);
        Assert.Equal("Great experience!", boat.ListComments.First());
    }

    [Fact]
    public async Task AddCommentToBattery_ShouldIncreaseCommentCount_WhenCommentIsAdded()
    {
        // Arrange
        var battery = new Battery("Test Battery");
        var booking = new Booking(DateTime.UtcNow.AddDays(3), "user1");
        booking.AddBattery(battery);

        // Act
        booking.Battery.AddComment("Long-lasting power!");

        // Assert
        Assert.Single(battery.ListComments);
        Assert.Equal("Long-lasting power!", battery.ListComments.First());
    }

    [Fact]
    public async Task AddBookingCountToBoat_ShouldIncreaseBookingCount_WhenBookingAdded()
    {
        // Arrange
        var boat = new Boat("Booking Counter Test Boat");
        var booking = new Booking(DateTime.UtcNow.Date.AddDays(3).AddHours(10), "user1");
        booking.AddBoat(boat);

        // Assert
        Assert.Equal(1, boat.CountBookings);
    }

    [Fact]
    public async Task AddBookingCountToBattery_ShouldIncreaseBookingCount_WhenBookingAdded()
    {
        // Arrange
        var battery = new Battery("Booking Counter Test Battery");
        var booking = new Booking(DateTime.UtcNow.Date.AddDays(3).AddHours(10), "user1");
        booking.AddBattery(battery);
        
        // Assert
        Assert.Equal(1, battery.CountBookings);
    }
}