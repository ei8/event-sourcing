using ei8.EventSourcing.Application;
using ei8.EventSourcing.Domain.Model;
using Microsoft.AspNetCore.DataProtection;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    public class KeyService : IKeyService
    {
        private readonly IDataProtector dataProtector;
        private readonly ISettingsService settingsService;

        public KeyService(IDataProtector dataProtector, ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(dataProtector, nameof(dataProtector));
            AssertionConcern.AssertArgumentNotNull(settingsService, nameof(settingsService));

            this.dataProtector = dataProtector;
            this.settingsService = settingsService;
        }

        public async Task Load(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            var privateKeyXml = File.ReadAllText(this.settingsService.PrivateKeyPath);
            var eventsKey = KeyService.Decrypt(keys.First(), privateKeyXml);
            this.settingsService.EventsKey = Convert.FromBase64String(eventsKey);
            EventStore.GetSettingsKeyPropertyPair().Protect(this.dataProtector, this.settingsService);
        }

        private static string Decrypt(string value, string privateKey)
        {
            var bytesToDescrypt = Encoding.UTF8.GetBytes(value);

            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // server decrypting data with private key
                    rsa.FromXmlString(privateKey);

                    var resultBytes = Convert.FromBase64String(value);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
    }
}
