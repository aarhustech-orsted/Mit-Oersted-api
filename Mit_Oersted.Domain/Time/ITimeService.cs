using System;

namespace Mit_Oersted.Domain.Time
{
    public interface ITimeService
    {
        DateTime Now { get; }
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}
