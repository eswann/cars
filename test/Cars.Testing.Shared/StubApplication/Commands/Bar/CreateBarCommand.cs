using System;
using Cars.Commands;

namespace Cars.Testing.Shared.StubApplication.Commands.Bar
{
    public class CreateBarCommand : CommandBase
    {
        public CreateBarCommand(Guid aggregateId) : base(aggregateId)
        {
        }
    }
}