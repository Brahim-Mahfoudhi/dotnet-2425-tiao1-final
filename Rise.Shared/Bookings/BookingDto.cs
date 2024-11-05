namespace Rise.Shared.Bookings;

public class BookingDto
{
    public class NewBooking
    {
        public DateTime bookingDate { get; set; } = DateTime.Now;
        public string userId { get; set; } = default!;
    }

    public class ViewBooking
    {
        public string bookingId { get; set; } = default!;
        public DateTime bookingDate { get; set; } = DateTime.Now;
        public BoatDto.ViewBoat boat { get; set; } = new();
        public BatteryDto.ViewBattery battery { get; set; } = new();
    }


    public class UpdateBooking
    {
        public string bookingId { get; set; } = default!;
        public DateTime? bookingDate { get; set; } = DateTime.Now;
        public BoatDto.NewBoat? boat { get; set; } = new();
        public BatteryDto.NewBattery? battery { get; set; } = new();
    }
}