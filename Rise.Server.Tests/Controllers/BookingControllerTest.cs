using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Rise.Server.Controllers;
using Rise.Services.Bookings;
using Rise.Shared.Bookings;
using Microsoft.Extensions.Options;
using Rise.Server.Settings;
using Rise.Shared.Enums;
using Shouldly;

namespace Rise.Server.Tests.Controllers;

public class BookingControllerTest
{
    private readonly Mock<IBookingService> _mockBookingService;
    private readonly BookingController _controller;

    public BookingControllerTest()
    {
        _mockBookingService = new Mock<IBookingService>();

        var bookingSettings = Options.Create(new BookingSettings
        {
            MinReservationDays = 1,
            MaxReservationDays = 30
        });

        _controller = new BookingController(_mockBookingService.Object, bookingSettings);

        // Set up a fake user with authorization roles
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "auth0|12345"),
            new Claim(ClaimTypes.Role, "User")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetAllBookings_ReturnsOkResult_WithBookings()
    {
        // Arrange
        var bookings = new List<BookingDto.ViewBooking>
        {
            new BookingDto.ViewBooking { bookingId = "1", /* other properties */ },
            new BookingDto.ViewBooking { bookingId = "2", /* other properties */ }
        };

        _mockBookingService.Setup(s => s.GetAllAsync()).ReturnsAsync(bookings);

        // Act
        var result = await _controller.GetAllBookings();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(bookings);
    }

    [Fact]
    public async Task GetAllBookings_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _mockBookingService.Setup(s => s.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllBookings();

        // Assert
        var statusCodeResult = result.Result as ObjectResult;
        statusCodeResult.ShouldNotBeNull();
        statusCodeResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        statusCodeResult.Value.ShouldBe("An error occurred while processing your request.");
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithBooking_WhenBookingExists()
    {
        // Arrange
        var bookingId = "1";
        var booking = new BookingDto.ViewBooking { bookingId = bookingId };
        _mockBookingService.Setup(s => s.GetBookingById(bookingId)).ReturnsAsync(booking);

        // Act
        var result = await _controller.Get(bookingId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(booking);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = "999"; // Non-existent ID
        _mockBookingService.Setup(s => s.GetBookingById(bookingId)).ReturnsAsync((BookingDto.ViewBooking?)null);

        // Act
        var result = await _controller.Get(bookingId);

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;
        notFoundResult.ShouldNotBeNull();
        notFoundResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        notFoundResult.Value.ShouldBe($"Booking with ID '{bookingId}' was not found.");
    }

    [Fact]
    public async Task Get_ReturnsBadRequest_WhenBookingIdIsInvalid()
    {
        // Act
        var result = await _controller.Get("");

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();
        badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        badRequestResult.Value.ShouldBe("Booking ID cannot be null or empty.");
    }

    [Fact]
    public async Task Post_ReturnsCreatedAtActionResult_WhenBookingIsCreated()
    {
        // Arrange
        var booking = new BookingDto.NewBooking
        {
            /* properties */
        };
        var createdBooking = new BookingDto.ViewBooking { bookingId = "1" };

        _mockBookingService.Setup(s => s.CreateBookingAsync(booking)).ReturnsAsync(createdBooking);

        // Act
        var result = await _controller.Post(booking);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult.ShouldNotBeNull();
        createdResult.StatusCode.ShouldBe(StatusCodes.Status201Created);
        createdResult.RouteValues["id"].ShouldBe(createdBooking.bookingId);
        createdResult.Value.ShouldBe(createdBooking);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenBookingIsNull()
    {
        // Act
        var result = await _controller.Post(null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();
        badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        badRequestResult.Value.ShouldBe("Booking details cannot be null.");
    }

    [Fact]
    public async Task Post_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var booking = new BookingDto.NewBooking
        {
            /* properties */
        };
        _mockBookingService.Setup(s => s.CreateBookingAsync(booking)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Post(booking);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.ShouldNotBeNull();
        statusCodeResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        statusCodeResult.Value.ShouldBe("An error occurred while processing your request: Database error");
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenBookingIsDeleted()
    {
        // Arrange
        var bookingId = "1";
        _mockBookingService.Setup(s => s.DeleteBookingAsync(bookingId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(bookingId);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.ShouldNotBeNull();
        noContentResult.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = "999"; // Non-existent ID
        _mockBookingService.Setup(s => s.DeleteBookingAsync(bookingId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(bookingId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.ShouldNotBeNull();
        notFoundResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        notFoundResult.Value.ShouldBe($"Booking with ID '{bookingId}' was not found.");
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_WhenBookingIdIsInvalid()
    {
        // Act
        var result = await _controller.Delete("");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();
        badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        badRequestResult.Value.ShouldBe("Booking ID cannot be null or empty.");
    }

    [Fact]
    public async Task Put_ReturnsNoContent_WhenBookingIsUpdated()
    {
        // Arrange
        var bookingId = "1";
        var booking = new BookingDto.UpdateBooking { bookingId = bookingId };
        _mockBookingService.Setup(s => s.UpdateBookingAsync(booking)).ReturnsAsync(true);

        // Act
        var result = await _controller.Put(bookingId, booking);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.ShouldNotBeNull();
        noContentResult.StatusCode.ShouldBe(StatusCodes.Status204NoContent);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenBookingDoesNotExist()
    {
        // Arrange
        var bookingId = "999"; // Non-existent ID
        var booking = new BookingDto.UpdateBooking { bookingId = bookingId };
        _mockBookingService.Setup(s => s.UpdateBookingAsync(booking)).ReturnsAsync(false);

        // Act
        var result = await _controller.Put(bookingId, booking);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.ShouldNotBeNull();
        notFoundResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        notFoundResult.Value.ShouldBe($"Booking with ID '{bookingId}' was not found.");
    }

    [Fact]
    public async Task Put_ReturnsBadRequest_WhenBookingIdIsInvalid()
    {
        // Arrange
        var booking = new BookingDto.UpdateBooking { bookingId = "1" };

        // Act
        var result = await _controller.Put("", booking);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();
        badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        badRequestResult.Value.ShouldBe("Invalid booking ID or details.");
    }

    [Fact]
    public async Task GetBookingsByDateRange_ReturnsOkResult_WithBookings()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-1);
        var endDate = DateTime.UtcNow.Date.AddDays(1);
        var bookings = new List<BookingDto.ViewBookingCalender>
        {
            new BookingDto.ViewBookingCalender { BookingDate = startDate.AddDays(1), TimeSlot = TimeSlot.Morning},
            new BookingDto.ViewBookingCalender { BookingDate = startDate.AddDays(1), TimeSlot = TimeSlot.Afternoon}
        };

        _mockBookingService.Setup(s => s.GetTakenTimeslotsInDateRange(startDate, endDate)).ReturnsAsync(bookings);

        // Act
        var result = await _controller.GetBookingsByDateRange(startDate, endDate);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(bookings);
    }

    [Fact]
    public async Task GetBookingsByDateRange_ReturnsBadRequest_WhenDatesAreMissing()
    {
        // Act
        var result = await _controller.GetBookingsByDateRange(null, null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();
        badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        badRequestResult.Value.ShouldBe("Start date and end date are required.");
    }

    [Fact]
    public async Task GetBookingsByDateRange_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(-1);
        var endDate = DateTime.UtcNow.Date.AddDays(1);

        _mockBookingService.Setup(s => s.GetTakenTimeslotsInDateRange(startDate, endDate))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetBookingsByDateRange(startDate, endDate);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.ShouldNotBeNull();
        statusCodeResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        statusCodeResult.Value.ShouldBe("An error occurred while processing your request.");
    }

    [Fact]
    public async Task GetFreeTimeslotsByDateRange_ReturnsOkResult_WithFreeTimeslots()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = DateTime.UtcNow.Date.AddDays(2);
        var freeTimeslots = new List<BookingDto.ViewBookingCalender>
        {
            new BookingDto.ViewBookingCalender { BookingDate = DateTime.Now, Available = true, TimeSlot = TimeSlot.Morning},
            new BookingDto.ViewBookingCalender { BookingDate = DateTime.Now, Available = true, TimeSlot = TimeSlot.Afternoon }
        };

        _mockBookingService.Setup(s => s.GetFreeTimeslotsInDateRange(startDate, endDate)).ReturnsAsync(freeTimeslots);

        // Act
        var result = await _controller.GetFreeTimeslotsByDateRange(startDate, endDate);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.ShouldNotBeNull();
        okResult.StatusCode.ShouldBe(StatusCodes.Status200OK);
        okResult.Value.ShouldBe(freeTimeslots);
    }

    [Fact]
    public async Task GetFreeTimeslotsByDateRange_ReturnsBadRequest_WhenDatesAreMissing()
    {
        // Act
        var result = await _controller.GetFreeTimeslotsByDateRange(null, null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.ShouldNotBeNull();
        badRequestResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        badRequestResult.Value.ShouldBe("Start date and end date are required.");
    }

    [Fact]
    public async Task GetFreeTimeslotsByDateRange_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var startDate = DateTime.UtcNow.Date.AddDays(1);
        var endDate = DateTime.UtcNow.Date.AddDays(2);

        _mockBookingService.Setup(s => s.GetFreeTimeslotsInDateRange(startDate, endDate))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetFreeTimeslotsByDateRange(startDate, endDate);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.ShouldNotBeNull();
        statusCodeResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        statusCodeResult.Value.ShouldBe("An error occurred while processing your request.");
    }
}