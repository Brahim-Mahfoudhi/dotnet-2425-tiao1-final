using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MudBlazor.Extensions;
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
        var jsonResponse = await httpClient.GetStringAsync("booking");
        return JsonSerializer.Deserialize<IEnumerable<BookingDto.ViewBooking>>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<BookingDto.ViewBooking?> GetBookingById(string id)
    {
        var jsonResponse = await httpClient.GetStringAsync($"booking/{id}");
        return JsonSerializer.Deserialize<BookingDto.ViewBooking>(jsonResponse, jsonSerializerOptions);
    }

    public async Task<BookingDto.ViewBooking> CreateBookingAsync(BookingDto.NewBooking booking)
    {
        var response = await httpClient.PostAsJsonAsync("booking", booking);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to create booking. Status Code: {response.StatusCode}, Message: {errorMessage}");
        }

        var bookingAsync = await response.Content.ReadFromJsonAsync<BookingDto.ViewBooking>(jsonSerializerOptions);

        return bookingAsync;
    }

    public async Task<bool> UpdateBookingAsync(BookingDto.UpdateBooking booking)
    {
        var response = await httpClient.PutAsJsonAsync($"booking/{booking.bookingId}", booking);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBookingAsync(string id)
    {
        var response = await httpClient.DeleteAsync($"booking/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IEnumerable<BookingDto.ViewBooking>?> GetAllUserBookings(string userid)
    {
        var bookings = await httpClient.GetStringAsync($"user/{userid}/bookings");
        return JsonSerializer.Deserialize<IEnumerable<BookingDto.ViewBooking>>(bookings, jsonSerializerOptions);
    }

    public async Task<BookingDto.ViewBooking?> GetFutureUserBooking(string userid)
    {
        var booking = await httpClient.GetStringAsync($"user/{userid}/bookings/future");
        return JsonSerializer.Deserialize<BookingDto.ViewBooking>(booking, jsonSerializerOptions);
    }

    public async Task<IEnumerable<BookingDto.ViewBookingCalender>?> GetTakenTimeslotsInDateRange(DateTime? startDate,
        DateTime? endDate)
    {
        var query = $"/api/Booking/byDateRange";
        if (startDate.HasValue || endDate.HasValue)
        {
            query += "?";
            if (startDate.HasValue)
            {
                query += $"startDate={startDate.ToIsoDateString()}";
            }

            if (endDate.HasValue)
            {
                query += $"{(startDate.HasValue ? "&" : "")}endDate={endDate.ToIsoDateString()}";
            }
        }

        var timeslots = await httpClient.GetStringAsync(query);
        return JsonSerializer.Deserialize<IEnumerable<BookingDto.ViewBookingCalender>>(timeslots,
            jsonSerializerOptions);
    }

    public async Task<IEnumerable<BookingDto.ViewBookingCalender>?> GetFreeTimeslotsInDateRange(DateTime? startDate,
        DateTime? endDate)
    {
        var query = $"/api/Booking/free/byDateRange";
        if (startDate.HasValue || endDate.HasValue)
        {
            query += "?";
            if (startDate.HasValue)
            {
                query += $"startDate={startDate.ToIsoDateString()}";
            }

            if (endDate.HasValue)
            {
                query += $"{(startDate.HasValue ? "&" : "")}endDate={endDate.ToIsoDateString()}";
            }
        }

        var timeslots = await httpClient
            .GetStringAsync(query);

        // to make the json serializer work correctly
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };

        var convertedTimeSlots =
            JsonSerializer.Deserialize<IEnumerable<BookingDto.ViewBookingCalender>>(timeslots, options);

        return convertedTimeSlots;
    }
}