namespace Retry.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    public class RetryTests
    {
        private readonly TimeSpan _validMaxWait = TimeSpan.FromSeconds(1);

        private readonly TimeSpan _validPollingInterval = TimeSpan.FromMilliseconds(2);

        [Test]
        public void Until_FunctionReturnsTrue_ExecutesOnce()
        {
            var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderWithNoTicks());
            int counter = 0;

            retry.Until(() =>
            {
                counter++;
                return true;
            });

            Assert.That(counter, Is.EqualTo(1));
        }

        [Test]
        public void Until_FunctionReturnsTrueOnSecondCycle_ExecutesTwice()
        {
            var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderWithNoTicks());
            int counter = 0;

            retry.Until(() =>
            {
                counter++;

                if (counter <= 1)
                {
                    return false;
                }

                return true;
            });

            Assert.That(counter, Is.EqualTo(2));
        }

        [Test]
        public void Until_FunctionReturnsFalse_ThrowsTimeoutException()
        {
            var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderThatTicksForOneCycle());
            int counter = 0;

            var ex = Assert.Throws<TimeoutException>(() =>
            {
                retry.Until(() =>
                {
                    counter++;
                    return false;
                });
            });

            Assert.Multiple(() =>
            {
                Assert.That(counter, Is.EqualTo(1));
                Assert.That(ex.Message, Is.EqualTo("Timed out after 00:00:01 polling every 00:00:00.0020000"));
                Assert.That(ex.InnerException, Is.TypeOf<AggregateException>());
                Assert.That(ex.InnerException?.Message, Is.EqualTo("One or more errors occurred."));
            });
        }

        [Test]
        public void Until_FunctionThrowsExceptionOnlyOnFirstCycle_DoesNotThrow()
        {
            var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderWithNoTicks());
            int counter = 0;

            retry.Until(() =>
            {
                counter++;

                if (counter <= 1)
                {
                    throw new Exception("test exception");
                }

                return true;
            });

            Assert.That(counter, Is.EqualTo(2));
        }

        [Test]
        public void Until_FunctionThrowsException_ThrowsTimeoutException()
        {
            var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderThatTicksForOneCycle());
            int counter = 0;

            var ex = Assert.Throws<TimeoutException>(() =>
            {
                retry.Until(() =>
                {
                    counter++;
                    throw new Exception("test exception");
                });
            });

            Assert.Multiple(() =>
            {
                Assert.That(counter, Is.EqualTo(1));
                Assert.That(ex.Message, Is.EqualTo("Timed out after 00:00:01 polling every 00:00:00.0020000"));
                Assert.That(ex.InnerException, Is.TypeOf<AggregateException>());
                Assert.That(ex.InnerException?.Message, Is.EqualTo("One or more errors occurred. (test exception)"));
                Assert.That((ex.InnerException as AggregateException)?.InnerExceptions, Has.Count.EqualTo(1));
            });
        }

        private static IDateTimeProvider CreateDateTimeProviderWithNoTicks()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc));
            return dateTimeProvider;
        }

        private static IDateTimeProvider CreateDateTimeProviderThatTicksForOneCycle()
        {
            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(
                x => new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                x => new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc),
                x => new DateTime(2000, 1, 1, 1, 1, 2, DateTimeKind.Utc));
            return dateTimeProvider;
        }
    }
}