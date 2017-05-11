using System;
using EnjoyCQRS.Commands;

namespace EnjoyCQRS.Testing.Shared.StubApplication.Commands.BarAggregate
{
    public class ManyDependenciesCommand : ICommand
    {
        public string Text { get; }

        public ManyDependenciesCommand(string text)
        {
            Text = text;
        }

        public Guid AggregateId { get; } = Guid.NewGuid();
    }
}