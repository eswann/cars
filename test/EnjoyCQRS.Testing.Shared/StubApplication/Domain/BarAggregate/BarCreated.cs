using System;
using EnjoyCQRS.Events;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Domain.BarAggregate
{
    public class BarCreated : DomainEvent
    {
        public BarCreated(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}