namespace HotelManager.Common;

public interface ISystemTimeProvider
{
    DateOnly GetCurrentDateUtc();
}

public class SystemTimeProvider : ISystemTimeProvider
{
    public DateOnly GetCurrentDateUtc()
    {
        return DateOnly.FromDateTime(DateTime.UtcNow);
    }
}