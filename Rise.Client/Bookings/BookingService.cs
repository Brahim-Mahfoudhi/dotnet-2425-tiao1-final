using System.Net.Http.Json;
using System.Text.Json;
using Rise.Shared.Bookings;

namespace Rise.Client.Bookings;

public class BookingService : IBookingService
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;


    public BookingService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllAsync()
    {
        var jsonResponse = await httpClient.GetStringAsync("booking/all");
        return JsonSerializer.Deserialize<IEnumerable<BookingDto.ViewBooking>>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<BookingDto.ViewBooking?> GetBookingById(string id)
    {
        var jsonResponse = await httpClient.GetStringAsync($"booking/{id}");
        return JsonSerializer.Deserialize<BookingDto.ViewBooking>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<bool> CreateBookingAsync(BookingDto.NewBooking booking)
    {
        Console.WriteLine(JsonSerializer.Serialize(booking, jsonSerializerOptions));
        var response = await httpClient.PostAsJsonAsync("booking", booking);
        Console.WriteLine($"response: {response}");
        return true;
    }

    public async Task<bool> UpdateBookingAsync(BookingDto.UpdateBooking booking)
    {
        var response = await httpClient.PutAsJsonAsync($"booking/{booking.id}", booking);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBookingAsync(string id)
    {
        var response = await httpClient.DeleteAsync($"booking/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllUserBookings(string userid)
    {
        var bookings = await httpClient.GetStringAsync($"booking/all/{userid}");
        return JsonSerializer.Deserialize<IEnumerable<BookingDto.ViewBooking>>(bookings, jsonSerializerOptions);
    }

    public async Task<BookingDto.ViewBooking?> GetFutureUserBooking(string userid)
    {
        var booking = await httpClient.GetStringAsync($"booking/future/{userid}");
        return JsonSerializer.Deserialize<BookingDto.ViewBooking>(booking, jsonSerializerOptions);

    }
}