using System;

namespace Cars.Commands
{
	public class DefaultResponse
	{
		public DefaultResponse(Guid aggregateId)
		{
			AggregateId = aggregateId;
		}

		public Guid AggregateId { get; }
	}
}