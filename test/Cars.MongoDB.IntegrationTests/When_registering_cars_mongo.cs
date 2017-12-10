using Cars.EventStore.MongoDB.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Cars.MongoDB.IntegrationTests
{
    public class When_registering_cars_mongo
    {
        [Fact]
        public void Services_are_registered_in_serviceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCarsMongo();

            serviceCollection.Count.Should().Be(11);
        }
    }
}
