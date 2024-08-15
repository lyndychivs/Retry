namespace Retry
{
    using System;

    internal interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}