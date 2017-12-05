using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Cars.Commands;
using Cars.Events;
using Cars.EventSource;
using Cars.EventSource.Projections;
using Cars.EventSource.SerializedEvents;
using Cars.EventSource.Snapshots;
using Cars.EventSource.Storage;
using Cars.MessageBus;
using Cars.MessageBus.InProcess;

namespace Cars.Configuration
{
	public static class Registration
	{
		private static readonly Type _commandType = typeof(ICommandHandler<,>);
		private static readonly Type _eventType = typeof(IEventHandler<>);

		public static IServiceCollection AddCars(this IServiceCollection services)
		{
			services.AddSingleton<IEventSerializer, EventSerializer>();
			services.AddSingleton<ISnapshotSerializer, SnapshotSerializer>();
			services.AddSingleton<IProjectionSerializer, ProjectionSerializer>();
			services.AddSingleton<IProjectionProviderScanner, ProjectionProviderAttributeScanner>();

			services.AddSingleton<ILoggerFactory, LoggerFactory>();
			services.AddSingleton<ISnapshotStrategy, IntervalSnapshotStrategy>();

			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddScoped<ISession, Session>();
			services.AddScoped<IRepository, Repository>();
			services.AddScoped<ICommandDispatcher, CommandDispatcher>();
			services.AddScoped<IEventRouter, EventRouter>();
			services.AddScoped<IEventPublisher, EventPublisher>();
			//services.AddScoped<IEventStore, InMemoryEventStore>();
			
			return services;
		}

		public static void RegisterCommandHandlersInAssemblyOf<T>(this IServiceCollection services)
		{
			services.RegisterCommandHandlersInAssemblyOf(typeof(T));
		}

		public static void RegisterCommandHandlersInAssemblyOf(this IServiceCollection services, Type typeInAssembly)
		{
			services.RegisterCommandHandlersInAssembly(typeInAssembly.GetTypeInfo().Assembly);
		}

		public static void RegisterCommandHandlersInAssembly(this IServiceCollection services, Assembly assembly)
		{
			RegisterHandlersInAssembly(services, assembly, _commandType);
		}

		public static void RegisterEventHandlersInAssemblyOf<T>(this IServiceCollection services)
		{
			services.RegisterEventHandlersInAssemblyOf(typeof(T));
		}

		public static void RegisterEventHandlersInAssemblyOf(this IServiceCollection services, Type typeInAssembly)
		{
			services.RegisterEventHandlersInAssembly(typeInAssembly.GetTypeInfo().Assembly);
		}

		public static void RegisterEventHandlersInAssembly(this IServiceCollection services, Assembly assembly)
		{
			RegisterHandlersInAssembly(services, assembly, _eventType);
		}

		private static void RegisterHandlersInAssembly(IServiceCollection serviceCollection, Assembly assembly, Type handlerType)
		{
			var implementationTypes = assembly.GetExportedTypes().Where(x =>
			{
				var typeInfo = x.GetTypeInfo();
				return typeInfo.IsClass && !typeInfo.IsAbstract
				       && typeInfo.ImplementedInterfaces.Any(i => i.GetTypeInfo().IsGenericType
				                                                  && i.GetGenericTypeDefinition() == handlerType);
			});
			foreach (var implementationType in implementationTypes)
			{
			    var interfaceTypes = implementationType.GetTypeInfo().GetInterfaces();
				foreach (var interfaceType in interfaceTypes)
				{
					serviceCollection.AddScoped(interfaceType, implementationType);
				}
			}
		}

	}



}
