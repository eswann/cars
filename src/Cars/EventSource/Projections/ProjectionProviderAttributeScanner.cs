using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cars.Attributes;

namespace Cars.EventSource.Projections
{
    public class ProjectionProviderAttributeScanner : IProjectionProviderScanner
    {
        private static readonly ConcurrentDictionary<string, HashSet<IProjectionProvider>> _cachedProviders = new ConcurrentDictionary<string, HashSet<IProjectionProvider>>();

        public Task<ScannerResult> ScanAsync(Type type)
        {
            if (!typeof(IAggregateMutator).IsAssignableFrom(type)) throw new TargetException($"The target should be {nameof(IAggregateMutator)}.");

            var providers = _cachedProviders.GetOrAdd(type.FullName, key =>
            {
                var foundProviders = type
                    .GetTypeInfo()
                    .GetCustomAttributes<ProjectionProviderAttribute>()
                    .Select(e => e.Provider)
                    .Distinct()
                    .Select(e => Activator.CreateInstance(e))
                    .Cast<IProjectionProvider>();

                return new HashSet<IProjectionProvider>(foundProviders);
            });

            var result = new ScannerResult(providers);

            return Task.FromResult(result);
        }

        public async Task<ScannerResult> ScanAsync<TAggregate>() where TAggregate : IAggregateMutator
        {
            return await ScanAsync(typeof(TAggregate));
        }
    }

    public class ScannerResult
    {
        public IEnumerable<IProjectionProvider> Providers { get; }

        public ScannerResult(IEnumerable<IProjectionProvider> providers)
        {
            Providers = providers;
        }
    }
}
