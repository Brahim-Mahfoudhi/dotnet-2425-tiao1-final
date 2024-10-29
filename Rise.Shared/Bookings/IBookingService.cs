namespace Rise.Shared.Bookings;

public interface IBookingService
{
    Task<IEnumerable<BookingDto.ViewBooking>?> GetAllAsync();
    Task<BookingDto.ViewBooking?> GetBookingById(string id);
    Task<bool> CreateBookingAsync(BookingDto.NewBooking booking);
    Task<bool> UpdateBookingAsync(BookingDto.UpdateBooking booking);
    Task<bool> DeleteBookingAsync(string id);
    
    Task<IEnumerable<BookingDto.ViewBooking>?> GetAllUserBookings(string userId);
    
    Task<BookingDto.ViewBooking?> GetFutureUserBooking(string userId);
    
}