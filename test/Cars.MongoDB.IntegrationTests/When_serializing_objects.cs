using System;
using Cars.EventStore.MongoDB;
using Cars.Testing.Shared.StubApplication.Domain.Foo;
using FluentAssertions;
using Xunit;

namespace Cars.MongoDB.IntegrationTests
{
    public class When_serializing_objects
    {
        [Fact]
        public void Object_can_be_serialized_and_deserialized()
        {
            var serializer = new BsonTextSerializer();

            var id = Guid.NewGuid();
            var objectToSerialize = new Foo(id);

            var serializedString = serializer.Serialize(objectToSerialize);

            var resultObject = serializer.Deserialize<Foo>(serializedString);

            resultObject.AggregateId.Should().Be(id);
        }

        [Fact]
        public void Object_can_be_serialized_and_deserialized_by_type_name()
        {
            var serializer = new BsonTextSerializer();

            var id = Guid.NewGuid();
            var objectToSerialize = new Foo(id);

            var serializedString = serializer.Serialize(objectToSerialize);

            var resultObject = (Foo)serializer.Deserialize(serializedString, typeof(Foo).AssemblyQualifiedName);

            resultObject.AggregateId.Should().Be(id);
        }
    }
}
