using neurUL.Common.Domain.Model;
using neurUL.Common.Security.Cryptography;

namespace ei8.EventSourcing.Application
{
    public static class SettingsServiceExtensions
    {
        public static bool ValidateEncryptionEnabled(this ISettingsService settingsService)
        {
            var result = false;

            if (settingsService.EncryptionEnabled)
            {
                AssertionConcern.AssertStateTrue(
                    settingsService.EventsKey != null && settingsService.EventsKey.Length > 0,
                    $"'{nameof(ISettingsService.EventsKey)}' is required when '{nameof(ISettingsService.EncryptionEnabled)}' is set to 'true'."
                );
                result = true;
            }

            return result;
        }

        private static ProtectedDataPropertyPair<ISettingsService, byte[]> settingsKeyProperty = null;
        public static ProtectedDataPropertyPair<ISettingsService, byte[]> GetKeyPropertyPair(this ISettingsService settingsService)
        {
            if (SettingsServiceExtensions.settingsKeyProperty == null)
                SettingsServiceExtensions.settingsKeyProperty = new ProtectedDataPropertyPair<ISettingsService, byte[]>(
                    s => s.EventsKey,
                    s => s.IsKeyProtected
                );

            return SettingsServiceExtensions.settingsKeyProperty;
        }
    }
}
