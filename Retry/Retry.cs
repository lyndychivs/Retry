namespace Retry
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Retry execute logic until a condition to be True
    /// </summary>
    public class Retry
    {
        private readonly TimeSpan _maxWait;

        private readonly TimeSpan _pollingInterval;

        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Retry"/>, with a default Polling Interval <see cref="TimeSpan"/> of 500 Milliseconds.
        /// </summary>
        /// <param name="maxWait">The maximum Time to wait as <see cref="TimeSpan"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Retry(TimeSpan maxWait)
            : this(maxWait, TimeSpan.FromMilliseconds(500))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Retry"/>.
        /// </summary>
        /// <param name="maxWait">The maximum Time to wait as <see cref="TimeSpan"/>.</param>
        /// <param name="pollingInterval">The wait Time between execution cycles as <see cref="TimeSpan"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public Retry(TimeSpan maxWait, TimeSpan pollingInterval)
            : this(maxWait, pollingInterval, new DateTimeProvider())
        {
        }

        internal Retry(TimeSpan maxWait, TimeSpan pollingInterval, IDateTimeProvider dateTimeProvider)
        {
            if (maxWait == TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(maxWait)} {nameof(TimeSpan)} must be greater than {TimeSpan.Zero}", nameof(maxWait));
            }

            _maxWait = maxWait;

            if (pollingInterval == TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(pollingInterval)} {nameof(TimeSpan)} must be greater than {TimeSpan.Zero}", nameof(pollingInterval));
            }

            if (pollingInterval >= maxWait)
            {
                throw new ArgumentException($"{nameof(pollingInterval)} ({pollingInterval}) must be less than {nameof(maxWait)} ({maxWait})", nameof(pollingInterval));
            }

            _pollingInterval = pollingInterval;

            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        /// <summary>
        /// Repeatedly executes the Function until a True condition is achieved or the Timeout is reached.
        /// </summary>
        /// <param name="function">The Function to execute until a True condition is achieved.</param>
        /// <exception cref="TimeoutException"></exception>
        public void Until(Func<bool> function)
        {
            bool result = false;

            DateTime endTime = _dateTimeProvider.UtcNow + _maxWait;

            var exceptions = new List<Exception>();

            while (_dateTimeProvider.UtcNow < endTime && !result)
            {
                try
                {
                    result = function();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

                if (result)
                {
                    return;
                }

                Thread.Sleep(_pollingInterval);
            }

            throw new TimeoutException($"Timed out after {_maxWait} polling every {_pollingInterval}", new AggregateException(exceptions));
        }
    }
}