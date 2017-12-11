using System.Threading.Tasks;

namespace Cars.Handlers
{
    public interface IEventController
    {
        bool IsOffline { get; set; }
    }
}