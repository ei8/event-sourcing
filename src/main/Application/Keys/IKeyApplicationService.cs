using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Application.Keys
{
    public interface IKeyApplicationService
    {
        Task Load(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    }
}
