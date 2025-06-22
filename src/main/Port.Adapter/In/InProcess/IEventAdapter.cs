using ei8.EventSourcing.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Port.Adapter.In.InProcess
{
    public interface IEventAdapter
    {
        Task Save(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    }
}
