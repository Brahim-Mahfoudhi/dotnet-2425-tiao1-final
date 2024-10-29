using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.Bookings;

namespace Rise.Client.Bookings;

public partial class MyBookingsView
{
    private IEnumerable<BookingDto.ViewBooking>? bookings;
    private BookingDto.ViewBooking? futureBooking;

    private string? userIdAuth0;

    [Inject] public required IBookingService BookingService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Get the current user's authentication state
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        userIdAuth0 = authState.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

        bookings = await BookingService.GetAllUserBookings(userIdAuth0);
        futureBooking = await BookingService.GetFutureUserBooking(userIdAuth0);
    }
}