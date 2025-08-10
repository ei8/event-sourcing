using neurUL.Common.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Domain.Model
{
    public interface IKeyService
    {
        Task Load<T>(
            string encryptedKey,
            string privateKeyPath,
            T instance,
            ProtectedDataPropertyPair<T, byte[]> propertyPair,
            CancellationToken cancellationToken = default
        );
    }
}
