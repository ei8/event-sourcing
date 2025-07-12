using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace ei8.EventSourcing.Domain.Model
{
    public interface IKeyService
    {
        Task Load(IEnumerable<string> keys, CancellationToken cancellationToken = default);
    }
}
