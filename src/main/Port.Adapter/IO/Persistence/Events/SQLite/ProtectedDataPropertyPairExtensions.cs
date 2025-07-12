using Microsoft.AspNetCore.DataProtection;
using System;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    // TODO:0 transfer to an appropriate namespace
    public static class ProtectedDataPropertyPairExtensions
    {
        public static void ProtectedInvoke<T>(
            this ProtectedDataPropertyPair<T, byte[]> dataPropertyPair,
            Action action,
            IDataProtector dataProtector,
            T instance
        )
        {
            dataPropertyPair.Protect(
                dataProtector,
                instance,
                false
            );

            action();

            dataPropertyPair.Protect(
                dataProtector,
                instance,
                true
            );
        }

        public static void Protect<T>(
            this ProtectedDataPropertyPair<T, byte[]> dataPropertyPair,
            IDataProtector dataProtector,
            T instance,
            bool applyProtection = true
        )
        {
            if (dataPropertyPair.IsDataProtectedProperty.Getter(instance) != applyProtection)
            {
                var data = dataPropertyPair.DataProperty.Getter(instance);
                dataPropertyPair.DataProperty.Setter(
                    instance,
                    applyProtection ?
                        dataProtector.Protect(data) :
                        dataProtector.Unprotect(data)
                );
                dataPropertyPair.IsDataProtectedProperty.Setter(instance, applyProtection);
            }
        }
    }
}
