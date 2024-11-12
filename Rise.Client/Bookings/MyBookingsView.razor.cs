using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Rise.Domain.Bookings;
using Rise.Shared.Bookings;

namespace Rise.Client.Bookings;

public partial class MyBookingsView
{
    [Inject] private IDialogService DialogService { get; set; }
    private IEnumerable<BookingDto.ViewBooking>? _bookings;
    private BookingDto.ViewBooking? futureBooking;

    private string? userIdAuth0;

    private bool _loadingFutureBookings;
    private List<BookingDto.ViewBooking> _futureElement = new List<BookingDto.ViewBooking>();

    [Inject] public required IBookingService BookingService { get; set; }
    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Get the current user's authentication state
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        userIdAuth0 = authState.User.Claims.FirstOrDefault(c => c.Type == "sub").Value;

        _bookings = await BookingService.GetAllUserBookings(userIdAuth0);
        futureBooking = await BookingService.GetFutureUserBooking(userIdAuth0);
        _futureElement.Add(futureBooking);
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
            futureBooking = await BookingService.GetFutureUserBooking(userIdAuth0);
            _futureElement.Clear();
            _futureElement.Add(futureBooking);
            StateHasChanged();
        }
    }
}