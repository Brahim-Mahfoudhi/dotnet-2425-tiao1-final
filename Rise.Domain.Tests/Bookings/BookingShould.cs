using System.Reflection.Metadata;
using Rise.Domain.Bookings;
using Shouldly;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Rise.Domain.Tests.Bookings;

public class BookingShould
{
    [Fact]
    public void ShouldCreateCorrectBooking()
    {
        Booking booking = new Booking(1, 1, new DateTime(1999, 12, 12), "1");

        booking.CountAdults.ShouldBe(1);
        booking.CountChildren.ShouldBe(1);
        booking.BookingDate.ShouldBe(new DateTime(1999, 12, 12));
    }

    [Fact]
    public void ShouldThrowInvalidCountAdults()
    {
        Action action = () => { new Booking(-1, 1, new DateTime(1999, 12, 12), "1"); };

        action.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowInvalidCountChildren()
    {
        Action action = () => { new Booking(1, -1, new DateTime(1999, 12, 12), "1"); };

        action.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowInvalidBookingDate()
    {
        Action action = () => { new Booking(1, -1, new DateTime(default), "1"); };

        action.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowInvalidBoat()
    {
        Boat boat = default;
        Booking booking = new Booking(1, 1, new DateTime(1999, 12, 12), "1");


        Action action = () => { booking.AddBoat(boat); };
        action.ShouldThrow<ArgumentException>();
    }
    
    [Fact]
    public void ShouldThrowInvalidBattery()
    {
        Battery battery = default;
        Booking booking = new Booking(1, 1, new DateTime(1999, 12, 12),"1");


        Action action = () => { booking.AddBattery(battery); };
        action.ShouldThrow<ArgumentException>();
    }
}