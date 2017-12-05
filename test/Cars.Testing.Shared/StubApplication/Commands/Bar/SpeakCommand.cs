using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
    public class SpeakCommand : CommandBase
    {
        public string Text { get; }

        public SpeakCommand(Guid aggregateId, string text) : base(aggregateId)
        {
            Text = text;
        }
    }
}