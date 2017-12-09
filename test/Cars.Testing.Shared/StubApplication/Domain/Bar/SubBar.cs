using System;
using System.Collections.Generic;
using Cars.EventSource;

namespace Cars.Testing.Shared.StubApplication.Domain.Bar
{
    public class SubBar : Aggregate
    {
        private List<string> _messages = new List<string>();

        public string LastText { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyList<string> Messages
        {
            get => _messages.AsReadOnly();

	        private set => _messages = new List<string>(value);
        }
        
        public SubBar()
        {
        }

        private SubBar(Guid id)
        {
            Emit(new BarCreated(id));
        }

        public static SubBar Create(Guid id)
        {
            return new SubBar(id);
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
                UpdatedAt = DateTime.Now;
            });

            SubscribeTo<SpokeSomething>(e =>
            {
                LastText = e.Text;
                UpdatedAt = DateTime.Now;

                _messages.Add(e.Text);
            });
        }
    }
}