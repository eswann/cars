using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cars.EventSource.Projections;
using Cars.Testing.Shared.StubApplication.Domain.Bar;
using Cars.Testing.Shared.StubApplication.Domain.Bar.Projections;
using FluentAssertions;
using Xunit;

namespace Cars.UnitTests.Projections
{
    public class ProjectionAttributeTests
    {
        [Fact]
        public async Task Should_scan_all_attributes_for_stream()
        {
            var bar = Bar.Create(Guid.NewGuid());

            var scanner = new ProjectionProviderAttributeScanner();

            var result = await scanner.ScanAsync(bar.GetType()).ConfigureAwait(false);

            Enumerable.Count(result.Providers).Should().Be(3);
        }

        [Fact]
        public void Should_throw_exception_when_target_is_not_stream()
        {
            var scanner = new ProjectionProviderAttributeScanner();

            Func<Task> func = async () => await scanner.ScanAsync(typeof(FakeNonStream)).ConfigureAwait(false);

            func.ShouldThrowExactly<TargetException>();
        }

        [Fact]
        public async Task Should_provide_a_projection_instance_from_stream()
        {
            var bar = Bar.Create(Guid.NewGuid());

            var scanner = new ProjectionProviderAttributeScanner();

            var result = await scanner.ScanAsync(bar.GetType()).ConfigureAwait(false);

            var provider = Enumerable.First(result.Providers, e => e.GetType().Name == nameof(BarProjectionProvider));

            var projection = (BarProjection) provider.CreateProjection(bar);

            projection.Id.Should().Be(bar.AggregateId);
            projection.LastText.Should().Be(bar.LastText);
            projection.Messages.Count.Should().Be(bar.Messages.Count);
            projection.UpdatedAt.Should().Be(bar.UpdatedAt);
        }
    }
    
    internal class FakeNonStream
    {

    }
}
