using EnjoyCQRS.Core;
using EnjoyCQRS.EventSource;
using EnjoyCQRS.EventSource.Snapshots;
using EnjoyCQRS.EventSource.Storage;
using EnjoyCQRS.Internal;
using EnjoyCQRS.Logger;
using EnjoyCQRS.MessageBus;
using EnjoyCQRS.MessageBus.InProcess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace EnjoyCQRS.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEnjoyCqrs(this IServiceCollection services)
        {
            services.TryAddSingleton<ILoggerFactory, NoopLoggerFactory>();
            services.TryAddSingleton<IEventUpdateManager, EventUpdateManager>();
            
            services.TryAddScoped<IUnitOfWork, UnitOfWork>();
            services.TryAddScoped<ISession, Session>();
            services.TryAddScoped<IEventPublisher, EventPublisher>();

            services.TryAddTransient<ISnapshotStrategy, IntervalSnapshotStrategy>();
            services.TryAddTransient<IRepository, Repository>();
            services.TryAddTransient<IEventSerializer, EventSerializer>();
            services.TryAddTransient<ISnapshotSerializer, SnapshotSerializer>();
        }
    }
}
