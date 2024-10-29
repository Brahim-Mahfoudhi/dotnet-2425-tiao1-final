using Rise.Domain.Bookings;
using Shouldly;

namespace Rise.Domain.Tests.Bookings;

public class BoatShould
{
    [Fact]
    public void ShouldCreateCorrectBoat()
    {
        string name = "Bootje";
        int countBookings = 1;
        List<string> listComments = new List<string>();
        listComments.Add("comment1");
        listComments.Add("comment2");
        Boat boat = new Boat(name, countBookings, listComments);
        boat.Name.ShouldBe(name);
        boat.CountBookings.ShouldBe(countBookings);
        boat.ListComments.ShouldBe(listComments);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("    ")]
    [InlineData("")]
    public void ShouldThrowInvalidName(string name)
    {
        int countBookings = 1;
        List<string> listComments = new List<string>();
        listComments.Add("comment1");
        listComments.Add("comment2");
        Action action = () => { new Boat(name, countBookings, listComments); };

        action.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowInvalidCountBookings()
    {
        string name = "Bootje";
        int countBookings = -1;
        List<string> listComments = new List<string>();
        listComments.Add("comment1");
        listComments.Add("comment2");
        Action action = () => { new Boat(name, countBookings, listComments); };

        action.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldThrowInvalidListComments()
    {
        string name = "Bootje";
        int countBookings = 1;
        List<string> listComments = null;
        Action action = () => { new Boat(name, countBookings, listComments); };

        action.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldAddBooking()
    {
        Boat boat = new Boat("boot");
        boat.AddBooking();
        boat.CountBookings.ShouldBe(1);
    }

    [Fact]
    public void ShouldAddComment()
    {
        Boat boat = new Boat("boot");
        boat.AddComment("String");
        boat.ListComments.Count.ShouldBe(1);
    }

    [Fact]
    public void ShouldThrowInvalidComment()
    {
        Boat boat = new Boat("boot");
        Action action = () => { boat.AddComment(null); };

        action.ShouldThrow<ArgumentException>();
    }
}