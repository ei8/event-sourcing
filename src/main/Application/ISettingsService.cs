namespace ei8.EventSourcing.Application
{
    public interface ISettingsService
    {
        bool IsKeyProtected { get; set; }
        byte[] EventsKey { get; set; }
        bool EncryptionEnabled { get; set; }
        string DatabasePath { get; set; }
        string PrivateKeyPath { get; set; }
    }
}
