using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Services.Bookings;
using Rise.Shared.Bookings;

namespace Rise.Server.Controllers;

/// <summary>
/// API controller for managing booking-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookingController"/> class with the specified booking service.
    /// </summary>
    /// <param name="bookingService">The booking service that handles booking operations.</param>
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Retrieves all bookings asynchronously.
    /// </summary>
    /// <returns>List of <see cref="BookingDto"/> objects or <c>null</c> if no bookings are found.</returns>
    [HttpGet("all")]
    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllBookings()
    {

        var bookings = await _bookingService.GetAllAsync();
        return bookings;
    }

    /// <summary>
    /// Retrieves a booking by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the booking to retrieve.</param>
    /// <returns>The <see cref="BookingDto"/> object or <c>null</c> if no booking with the specified ID is found.</returns>
    [HttpGet("{id}")]
    public async Task<BookingDto.ViewBooking?> Get(string id)
    {
        var booking = await _bookingService.GetBookingById(id);
        return booking;
    }

    /// <summary>
    /// Creates a new booking asynchronously.
    /// </summary>
    /// <param name="booking">The <see cref="BookingDto.NewBooking"/> object containing booking details to create.</param>
    /// <returns>The created <see cref="BookingDto.NewBooking"/> object or <c>null</c> if the booking creation fails.</returns>
    [HttpPost]
    public async Task<bool> Post(BookingDto.NewBooking booking)
    {
        var created = await _bookingService.CreateBookingAsync(booking);
        return created;
    }

    /// <summary>
    /// Updates an existing booking asynchronously.
    /// </summary>
    /// <param name="id">The id of an existing <see cref="Booking"/></param>
    /// <param name=" booking">The <see cref="BookingDto.NewBooking"/> object containing updated booking details.</param>
    /// <returns><c>true</c> if the update is successful; otherwise, <c>false</c>.</returns>
    [HttpPut("{id}")]
    public async Task<bool> Put(BookingDto.UpdateBooking booking)
    {
        var updatedBooking = await _bookingService.UpdateBookingAsync(booking);
        return updatedBooking;
    }

    /// <summary>
    /// Deletes a booking by their ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the booking to delete.</param>
    /// <returns><c>true</c> if the deletion is successful; otherwise, <c>false</c>.</returns>
    [HttpDelete("{id}")]
    public async Task<bool> Delete(string id)
    {
        var deleted = await _bookingService.DeleteBookingAsync(id);
        return deleted;
    }

    /// <summary>
    /// Retrieves all bookings asynchronously for specific user.
    /// </summary>
    /// <returns>List of <see cref="BookingDto"/> objects or <c>null</c> if no bookings are found.</returns>
    [HttpGet("all/{userid}")]
    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllUserBookings(string userid)
    {

        var bookings = await _bookingService.GetAllUserBookings(userid);
        return bookings;
    }

    /// <summary>
    /// Retrieves future booking asynchronously for specific user.
    /// </summary>
    /// <returns><see cref="BookingDto"/> object or <c>null</c> if no booking is found.</returns>
    [HttpGet("future/{userid}")]
    public async Task<BookingDto.ViewBooking>? GetFutureUserBooking(string userid)
    {

        var booking = await _bookingService.GetFutureUserBooking(userid);
        return booking;
    }

}