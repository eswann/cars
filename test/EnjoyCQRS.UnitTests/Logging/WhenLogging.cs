using System.Collections.Generic;
using System.Linq;
using EnjoyCQRS.Testing.Shared.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace EnjoyCQRS.UnitTests.Logging
{
    public class WhenLogging
    {
        private readonly IList<TestMessage> _messages = new List<TestMessage>();
        private readonly ILoggerFactory _loggerFactory;

        public WhenLogging()
        {
            _loggerFactory = new TestLoggerFactory(true, _messages);
        }

        [Fact]
        public void Can_log_critical_message()
        {
            var logger = _loggerFactory.CreateLogger("TestLogger");
             
            logger.LogCritical("Critical Message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().NotBeNull();
            loggedMessage.LogLevel.ToString().Should().Be("Critical");
            loggedMessage.Exception.Should().BeNull();
            loggedMessage.LoggerName.Should().Be("TestLogger");
            loggedMessage.Message.Should().Be("Critical Message");
        }

        [Fact]
        public void Can_log_error_message()
        {
            var logger = _loggerFactory.CreateLogger("TestLogger");

            logger.LogError("Error Message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().NotBeNull();
            loggedMessage.LogLevel.ToString().Should().Be("Error");
            loggedMessage.Exception.Should().BeNull();
            loggedMessage.LoggerName.Should().Be("TestLogger");
            loggedMessage.Message.Should().Be("Error Message");
        }

        [Fact]
        public void Can_log_warning_message()
        {
            var logger = _loggerFactory.CreateLogger("TestLogger");

            logger.LogWarning("Warning Message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().NotBeNull();
            loggedMessage.LogLevel.ToString().Should().Be("Warning");
            loggedMessage.Exception.Should().BeNull();
            loggedMessage.LoggerName.Should().Be("TestLogger");
            loggedMessage.Message.Should().Be("Warning Message");
        }

        [Fact]
        public void Can_log_information_message()
        {
            var logger = _loggerFactory.CreateLogger("TestLogger");

            logger.LogInformation("Information Message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().NotBeNull();
            loggedMessage.LogLevel.ToString().Should().Be("Information");
            loggedMessage.Exception.Should().BeNull();
            loggedMessage.LoggerName.Should().Be("TestLogger");
            loggedMessage.Message.Should().Be("Information Message");
        }

        [Fact]
        public void Can_log_debug_message()
        {
            var logger = _loggerFactory.CreateLogger("TestLogger");

            logger.LogDebug("Debug Message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().NotBeNull();
            loggedMessage.LogLevel.ToString().Should().Be("Debug");
            loggedMessage.Exception.Should().BeNull();
            loggedMessage.LoggerName.Should().Be("TestLogger");
            loggedMessage.Message.Should().Be("Debug Message");
        }

        [Fact]
        public void Can_log_trace_message()
        {
            var logger = _loggerFactory.CreateLogger("TestLogger");

            logger.LogTrace("Trace Message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().NotBeNull();
            loggedMessage.LogLevel.ToString().Should().Be("Trace");
            loggedMessage.Exception.Should().BeNull();
            loggedMessage.LoggerName.Should().Be("TestLogger");
            loggedMessage.Message.Should().Be("Trace Message");
        }

        [Fact]
        public void Disabled_logger_doesnt_log_a_message()
        {
            _messages.Clear();
            var loggerFactory = new TestLoggerFactory(false, _messages);
            var logger = loggerFactory.CreateLogger("DisabledLogger");

            logger.LogCritical("Disabled logger message");

            var loggedMessage = _messages.LastOrDefault();
            loggedMessage.Should().BeNull();
        }
    }

}