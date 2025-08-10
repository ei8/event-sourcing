using ei8.EventSourcing.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Application.Keys
{
    public class KeyApplicationService : IKeyApplicationService
    {
        private readonly IKeyService keyService;
        private readonly ISettingsService settingsService;

        public KeyApplicationService(IKeyService keyService, ISettingsService settingsService)
        {
            this.keyService = keyService;
            this.settingsService = settingsService;
        }

        public async Task Load(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            await this.keyService.Load(
                keys.First(),
                this.settingsService.PrivateKeyPath,
                this.settingsService,
                this.settingsService.GetKeyPropertyPair(),
                cancellationToken
            );
        }
    }
}
