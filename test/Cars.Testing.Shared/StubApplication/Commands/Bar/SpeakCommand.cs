using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
    public class SpeakCommand : Command
    {
        public string Text { get; }

        public SpeakCommand(Guid streamId, string text) : base(streamId)
        {
            Text = text;
        }
    }
}