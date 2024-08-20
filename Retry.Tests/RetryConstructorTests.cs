namespace Retry.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class RetryConstructorTests
    {
        private readonly TimeSpan _validMaxWait = TimeSpan.FromSeconds(10);

        private readonly TimeSpan _validPollingInterval = TimeSpan.FromMilliseconds(500);

        [Test]
        public void Constructor_ValidParameters_ReturnsRetry()
        {
            var retry = new Retry(_validMaxWait);

            Assert.That(retry, Is.Not.Null);
        }

        [Test]
        public void Constructor_ZeroMaxWait_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Retry(TimeSpan.Zero));

            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("maxWait"));
                Assert.That(ex.Message, Is.EqualTo("maxWait TimeSpan must be greater than 00:00:00 (Parameter 'maxWait')"));
            });
        }

        [Test]
        public void Constructor_ZeroPollingInterval_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Retry(_validMaxWait, TimeSpan.Zero));

            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("pollingInterval"));
                Assert.That(ex.Message, Is.EqualTo("pollingInterval TimeSpan must be greater than 00:00:00 (Parameter 'pollingInterval')"));
            });
        }

        [Test]
        public void Constructor_PollingIntervalEqualToMaxWait_ThrowsArgumentException()
        {
            var pollingInterval = _validMaxWait;

            var ex = Assert.Throws<ArgumentException>(() => new Retry(_validMaxWait, pollingInterval));

            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("pollingInterval"));
                Assert.That(ex.Message, Is.EqualTo("pollingInterval (00:00:10) must be less than maxWait (00:00:10) (Parameter 'pollingInterval')"));
            });
        }

        [Test]
        public void Constructor_PollingIntervalGreaterThanMaxWait_ThrowsArgumentException()
        {
            var pollingInterval = _validMaxWait + TimeSpan.FromMilliseconds(1);

            var ex = Assert.Throws<ArgumentException>(() => new Retry(_validMaxWait, pollingInterval));

            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("pollingInterval"));
                Assert.That(ex.Message, Is.EqualTo("pollingInterval (00:00:10.0010000) must be less than maxWait (00:00:10) (Parameter 'pollingInterval')"));
            });
        }

        [Test]
        public void Constructor_NullDateTimeProvider_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Retry(_validMaxWait, _validPollingInterval, null));

            Assert.Multiple(() =>
            {
                Assert.That(ex.ParamName, Is.EqualTo("dateTimeProvider"));
                Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'dateTimeProvider')"));
            });
        }
    }
}