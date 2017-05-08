using System;
using System.Reflection;
using EnjoyCQRS.Commands;
using EnjoyCQRS.Events;
using EnjoyCQRS.EventSource;
using EnjoyCQRS.EventSource.Snapshots;
using EnjoyCQRS.EventSource.Storage;
using EnjoyCQRS.Logger;
using EnjoyCQRS.MessageBus.InProcess;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Grace.DependencyInjection.Conditions;

namespace EnjoyCQRS.Grace
{
    public static class RegistrationExtensions
    {
        public static void RegisterDefaults(this IInjectionScope scope)
        {
            scope.Configure(config =>
            {
                config.Export<EventSerializer>().ByInterfaces().Lifestyle.Singleton();
                config.Export<SnapshotSerializer>().ByInterfaces().Lifestyle.Singleton();
                config.Export<NoopLoggerFactory>().ByInterfaces().Lifestyle.Singleton();
                config.Export<IntervalSnapshotStrategy>().ByInterfaces().Lifestyle.Singleton();

                config.Export<UnitOfWork>().ByInterfaces().Lifestyle.SingletonPerScope();
                config.Export<Session>().ByInterfaces().Lifestyle.SingletonPerScope();
                config.Export<Repository>().ByInterfaces().Lifestyle.SingletonPerScope();
                config.Export<EventPublisher>().ByInterfaces().Lifestyle.SingletonPerScope();
                
            });
        }

        public static void RegisterCommandHandlersInAssembly<T>(this IInjectionScope scope)
        {
            RegisterCommandHandlersInAssembly(scope, typeof(T));
        }

        public static void RegisterCommandHandlersInAssembly(this IInjectionScope scope, Type typeInAssembly)
        {
            RegisterCommandHandlersInAssembly(scope, typeInAssembly.GetTypeInfo().Assembly);
        }

        public static void RegisterCommandHandlersInAssembly(this IInjectionScope scope, Assembly assembly)
        {
            scope.Configure(config =>
            {
                config.Export(assembly.GetExportedTypes()).BasedOn(typeof(ICommandHandler<>)).ByInterfaces().Lifestyle.SingletonPerScope();
            });
        }

        public static void RegisterEventHandlersInAssembly<T>(this IInjectionScope scope)
        {
            RegisterEventHandlersInAssembly(scope, typeof(T));
        }

        public static void RegisterEventHandlersInAssembly(this IInjectionScope scope, Type typeInAssembly)
        {
            RegisterEventHandlersInAssembly(scope, typeInAssembly.GetTypeInfo().Assembly);
        }

        public static void RegisterEventHandlersInAssembly(this IInjectionScope scope, Assembly assembly)
        {
            scope.Configure(config =>
            {
                config.Export(assembly.GetExportedTypes()).BasedOn(typeof(IEventHandler<>)).ByInterfaces().Lifestyle.SingletonPerScope();
            });
        }
    }
}
