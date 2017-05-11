using EnjoyCQRS.EventSource;
using EnjoyCQRS.EventSource.Snapshots;
using EnjoyCQRS.EventSource.Storage;
using EnjoyCQRS.Logger;
using EnjoyCQRS.MessageBus;
using EnjoyCQRS.MessageBus.InProcess;
using FluentAssertions;
using Grace.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace EnjoyCQRS.Grace.UnitTests
{
    public class WhenRegisteringDefaultImplementations
    {
        [Fact]
        public void Default_implementations_are_present()
        {
            var container = new DependencyInjectionContainer();
            container.RegisterDefaults();

            container.Locate<IEventSerializer>().Should().BeOfType<EventSerializer>();
            container.Locate<ISnapshotSerializer>().Should().BeOfType<SnapshotSerializer>();
            container.Locate<IEventSerializer>().Should().BeOfType<EventSerializer>();
            container.Locate<ILoggerFactory>().Should().BeOfType<NoopLoggerFactory>();
            container.Locate<ISnapshotStrategy>().Should().BeOfType<IntervalSnapshotStrategy>();

            container.Locate<IUnitOfWork>().Should().BeOfType<UnitOfWork>();
            container.Locate<ISession>().Should().BeOfType<Session>();
            container.Locate<IRepository>().Should().BeOfType<Repository>();
            container.Locate<IEventPublisher>().Should().BeOfType<EventPublisher>();

            container.Locate<ICommandDispatcher>().Should().BeOfType<GraceCommandDispatcher>();
            container.Locate<IEventRouter>().Should().BeOfType<GraceEventRouter>();

        }
    }
}
