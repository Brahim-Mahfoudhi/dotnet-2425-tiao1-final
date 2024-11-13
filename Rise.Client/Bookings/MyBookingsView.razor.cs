using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MudBlazor;
using Rise.Domain.Bookings;
using Rise.Server.Settings;
using Rise.Shared.Bookings;

namespace Rise.Client.Bookings;

public partial class MyBookingsView
{
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] public required IBookingService BookingService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    
    private IEnumerable<BookingDto.ViewBooking>? _pastBookings;
    private IEnumerable<BookingDto.ViewBooking>? _futureBookings;
    private int _maxBookings;
    private string? userIdAuth0;
    private bool _loadingFutureBookings;

    protected override async Task OnInitializedAsync()
    {
        // Get the current user's authentication state
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        userIdAuth0 = authState.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

        _pastBookings = await BookingService.GetPastUserBookings(userIdAuth0) ?? new List<BookingDto.ViewBooking>();
        _futureBookings = await BookingService.GetFutureUserBookings(userIdAuth0) ?? new List<BookingDto.ViewBooking>();
    }

    private async Task DeleteBooking(string bookingId)
    {
        var result = await DialogService.ShowMessageBox(
            Localizer["CancelBookingTitle"],
            Localizer["CancelBookingMessage"],
            Localizer["CancelBookingYes"],
            Localizer["CancelBookingNo"]
        );

        if (result == true)
        {
            await BookingService.DeleteBookingAsync(bookingId);
            _futureBookings = await BookingService.GetFutureUserBookings(userIdAuth0);
            StateHasChanged();
        }
    }
}