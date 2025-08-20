using ei8.EventSourcing.Domain.Model;
using Microsoft.AspNetCore.DataProtection;
using neurUL.Common.Domain.Model;
using neurUL.Common.Security.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    public class KeyService : IKeyService
    {
        private readonly IDataProtector dataProtector;

        public KeyService(IDataProtector dataProtector)
        {
            AssertionConcern.AssertArgumentNotNull(dataProtector, nameof(dataProtector));

            this.dataProtector = dataProtector;
        }

        public async Task Load<T>(
            string encryptedKey, 
            string privateKeyPath,
            T instance, 
            ProtectedDataPropertyPair<T, byte[]> propertyPair, 
            CancellationToken cancellationToken = default
        )
        {
            var privateKeyXml = File.ReadAllText(privateKeyPath);
            var eventsKey = KeyService.Decrypt(encryptedKey, privateKeyXml);
            propertyPair.DataProperty.Setter(instance, Convert.FromBase64String(eventsKey));
            propertyPair.Protect(this.dataProtector, instance);
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
