namespace Rise.Shared.Enums;

public enum TimeSlot
{
    None,
    [TimeSlotHours(10, 12, "Morning")] Morning,
    [TimeSlotHours(14, 16, "Afternoon")] Afternoon,
    [TimeSlotHours(17, 19, "Evening")] Evening
}

public class TimeSlotHoursAttribute : Attribute
{
    public double StartHour { get; }
    public double EndHour { get; }
    public string TimeSlot { get; }

    public TimeSlotHoursAttribute(double startHour, double endHour, string timeSlot)
    {
        StartHour = startHour;
        EndHour = endHour;
        TimeSlot = timeSlot;
    }
}

public static class TimeSlotEnumExtensions
{
    public static TimeSlot ToTimeSlot(double startHour)
    {
        var startHourToTimeSlot = new Dictionary<double, TimeSlot>
        {
            { TimeSlot.Morning.GetStartHour(), TimeSlot.Morning },
            { TimeSlot.Afternoon.GetStartHour(), TimeSlot.Afternoon },
            { TimeSlot.Evening.GetStartHour(), TimeSlot.Evening }
        };

        return startHourToTimeSlot.TryGetValue(startHour, out var timeSlot) ? timeSlot : TimeSlot.None;
    }

    public static double GetStartHour(this TimeSlot timeSlot)
    {
        var attribute = (TimeSlotHoursAttribute?)Attribute.GetCustomAttribute(
            typeof(TimeSlot).GetField(timeSlot.ToString())!,
            typeof(TimeSlotHoursAttribute));
        
        if (attribute != null)
        {
            return attribute.StartHour;
        }
        
        throw new ArgumentException($"Invalid TimeSlot value: {timeSlot}");
            
    }

    public static double GetEndHour(this TimeSlot timeSlot)
    {
        var attribute = (TimeSlotHoursAttribute?)Attribute.GetCustomAttribute(
            typeof(TimeSlot).GetField(timeSlot.ToString())!,
            typeof(TimeSlotHoursAttribute));

        if (attribute != null)
        {
            return attribute.EndHour;
        }
        
        throw new ArgumentException($"Invalid TimeSlot value: {timeSlot}");
    }
}