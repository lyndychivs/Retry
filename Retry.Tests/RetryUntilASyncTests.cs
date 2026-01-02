namespace Retry.Tests;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

[TestFixture]
internal sealed class RetryUntilAsyncTests
{
    private readonly TimeSpan _validMaxWait = TimeSpan.FromSeconds(1);

    private readonly TimeSpan _validPollingInterval = TimeSpan.FromMilliseconds(2);

    [Test]
    public async Task UntilAsync_FunctionReturnsTrue_ExecutesOnce()
    {
        var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderWithNoTicks());
        int counter = 0;

        await retry.UntilAsync(async () =>
        {
            counter++;
            return await Task.FromResult(true);
        });

        Assert.That(counter, Is.EqualTo(1));
    }

    [Test]
    public async Task UntilAsync_FunctionReturnsTrueOnSecondCycle_ExecutesTwice()
    {
        var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderWithNoTicks());
        int counter = 0;

        await retry.UntilAsync(async () =>
        {
            counter++;

            if (counter <= 1)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        });

        Assert.That(counter, Is.EqualTo(2));
    }

    [Test]
    public async Task UntilAsync_WithPollingInterval_CorrectlyAwaitsPollingIntervalBeforeSecondCycle()
    {
        var retry = new Retry(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), CreateDateTimeProviderWithNoTicks());
        int counter = 0;
        var stopwatch = new Stopwatch();

        await retry.UntilAsync(async () =>
        {
            counter++;
            stopwatch.Start();

            if (counter <= 1)
            {
                return await Task.FromResult(false);
            }

            stopwatch.Stop();
            return await Task.FromResult(true);
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counter, Is.EqualTo(2));
            Assert.That(stopwatch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
        }
    }

    [Test]
    public async Task UntilAsync_FunctionReturnsFalse_ThrowsTimeoutException()
    {
        var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderThatTicksForOneCycle());
        int counter = 0;

        var ex = Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await retry.UntilAsync(async () =>
            {
                counter++;
                return await Task.FromResult(false);
            });
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counter, Is.EqualTo(1));
            Assert.That(ex.Message, Is.EqualTo("Timed out after 00:00:01 polling every 00:00:00.0020000"));
            Assert.That(ex.InnerException, Is.TypeOf<AggregateException>());
            Assert.That(ex.InnerException?.Message, Is.EqualTo("One or more errors occurred."));
        }
    }

    [Test]
    public async Task UntilAsync_FunctionThrowsExceptionOnlyOnFirstCycle_DoesNotThrow()
    {
        var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderWithNoTicks());
        int counter = 0;

        await retry.UntilAsync(async () =>
        {
            counter++;

            if (counter <= 1)
            {
                throw new Exception("test exception");
            }

            return await Task.FromResult(true);
        });

        Assert.That(counter, Is.EqualTo(2));
    }

    [Test]
    public async Task UntilAsync_FunctionThrowsException_ThrowsTimeoutException()
    {
        var retry = new Retry(_validMaxWait, _validPollingInterval, CreateDateTimeProviderThatTicksForOneCycle());
        int counter = 0;

        var ex = Assert.ThrowsAsync<TimeoutException>(async () =>
        {
            await retry.UntilAsync(async () =>
            {
                counter++;
                throw new Exception("test exception");
            });
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(counter, Is.EqualTo(1));
            Assert.That(ex.Message, Is.EqualTo("Timed out after 00:00:01 polling every 00:00:00.0020000"));
            Assert.That(ex.InnerException, Is.TypeOf<AggregateException>());
            Assert.That(ex.InnerException?.Message, Is.EqualTo("One or more errors occurred. (test exception)"));
            Assert.That((ex.InnerException as AggregateException)?.InnerExceptions, Has.Count.EqualTo(1));
        }
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
