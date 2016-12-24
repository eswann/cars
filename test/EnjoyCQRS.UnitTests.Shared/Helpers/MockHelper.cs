using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace EnjoyCQRS.UnitTests.Shared
{
    public static class MockHelper
    {
        public static Mock<ILogger> GetMockLogger()
        {
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(e => e.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
            mockLogger.Setup(e => e.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            return mockLogger;
        }

        public static Mock<ILoggerFactory> GetMockLoggerFactory(ILogger logger)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            mockLoggerFactory.Setup(e => e.CreateLogger(It.IsAny<string>())).Returns(logger);

            return mockLoggerFactory;
        }

        public static ILoggerFactory CreateLoggerFactory(ILogger logger)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();

            mockLoggerFactory.Setup(e => e.CreateLogger(It.IsAny<string>())).Returns(logger);

            return mockLoggerFactory.Object;
        }
    }
}