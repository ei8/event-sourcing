using ei8.EventSourcing.Domain.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Application.Keys
{
    public class KeyApplicationService : IKeyApplicationService
    {
        private readonly IKeyService keyService;

        public KeyApplicationService(IKeyService keyService)
        {
            this.keyService = keyService;
        }

        public async Task Load(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            await this.keyService.Load(keys, cancellationToken);
        }
    }
}
