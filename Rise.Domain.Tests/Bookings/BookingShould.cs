using System.Reflection.Metadata;
using Rise.Domain.Bookings;
using Rise.Shared.Enums;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Rise.Domain.Tests.Bookings;

public class BookingShould
{
    [Fact]
    public void ShouldCreateCorrectBooking()
    {
        Booking booking = new Booking(new DateTime(1999, 12, 12), "1", TimeSlot.Morning);

        booking.BookingDate.ShouldBe(new DateTime(1999, 12, 12));
        booking.TimeSlot.ShouldBe(TimeSlot.Morning);
        booking.TimeSlot.GetStartHour().ShouldBe(TimeSlot.Morning.GetStartHour());
    }
    
    [Fact]
    public void ShouldThrowInvalidBookingDate()
    {
        Action action = () => { new Booking(new DateTime(default), "1", TimeSlot.Afternoon); };
    
        action.ShouldThrow<ArgumentException>();
    }
    
    [Fact]
    public void ShouldThrowInvalidBoat()
    {
        Boat boat = default;
        Booking booking = new Booking(new DateTime(1999, 12, 12), "1", TimeSlot.Evening);
    
        Action action = () => { booking.AddBoat(boat); };
        action.ShouldThrow<ArgumentException>();
    }
    
    [Fact]
    public void ShouldThrowInvalidBattery()
    {
        Battery battery = default;
        Booking booking = new Booking(new DateTime(1999, 12, 12),"1", TimeSlot.Morning);
    
    
        Action action = () => { booking.AddBattery(battery); };
        action.ShouldThrow<ArgumentException>();
    }
}