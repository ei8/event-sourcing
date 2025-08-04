using neurUL.Common.Domain.Model;

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
    }
}
