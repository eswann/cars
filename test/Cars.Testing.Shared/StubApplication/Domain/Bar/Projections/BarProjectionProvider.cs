using System.Collections.Generic;
using Cars.EventSource;
using Cars.EventSource.Projections;

namespace Cars.Testing.Shared.StubApplication.Domain.Bar.Projections
{
    public class BarProjectionProvider : IProjectionProvider
    {
        public object CreateProjection(IStream stream)
        {
            var target = (Bar)stream;

            return new BarProjection
            {
                Id = target.Id,
                LastText = target.LastText,
                UpdatedAt = target.UpdatedAt,
                Messages = new List<string>(target.Messages)
            };
        }
    }

    public class BarWithoutMessagesProjectionProvider : IProjectionProvider
    {
        public object CreateProjection(IStream stream)
        {
            var target = (Bar)stream;

            return new BarWithoutMessagesProjection
            {
                Id = target.Id,
                LastText = target.LastText,
                UpdatedAt = target.UpdatedAt
            };
        }
    }

    public class BarOnlyIdProjectionProvider : ProjectionProvider<Bar, BarWithIdOnlyProjection>
    {
        public override BarWithIdOnlyProjection CreateProjection(Bar stream)
        {
            return new BarWithIdOnlyProjection
            {
                Id = stream.Id
            };
        }
    }
}
