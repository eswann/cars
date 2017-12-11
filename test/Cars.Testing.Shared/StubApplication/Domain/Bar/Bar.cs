using System;
using System.Collections.Generic;
using Cars.EventSource;

namespace Cars.Testing.Shared.StubApplication.Domain.Bar
{
    public class Bar : Aggregate
    {
        private readonly List<string> _messages = new List<string>();

        public string LastText { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyList<string> Messages => _messages.AsReadOnly();

        public Bar()
        {
        }

        private Bar(Guid id)
        {
            Emit(new BarCreated(id));
        }

        public static Bar Create(Guid id)
        {
            return new Bar(id);
        }

        public void Speak(string text)
        {
            Emit(new SpokeSomething(text));
        }

        protected override void RegisterEvents()
        {
            SubscribeTo<BarCreated>(e =>
            {
                AggregateId = e.AggregateId;
                UpdatedAt = DateTime.UtcNow;
            });

            SubscribeTo<SpokeSomething>(e =>
            {
                LastText = e.Text;
                UpdatedAt = DateTime.UtcNow;

                _messages.Add(e.Text);
            });
        }
    }
}