namespace Retry
{
    using System;

    internal class DateTimeProvider: IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}