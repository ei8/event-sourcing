using ei8.EventSourcing.Application;
using neurUL.Common.Domain.Model;

namespace ei8.EventSourcing.Port.Adapter.IO.Process.Services
{
    public class InProcessSettingsService : ISettingsService
    {
        private string databasePath;
        private string privateKeyPath;

        public InProcessSettingsService()
        {
            this.databasePath = string.Empty;
        }

        public string DatabasePath
        {
            get
            {
                return this.databasePath;
            }
            set
            {
                if (this.databasePath != value)
                {
                    Helper.ValidateDatabasePath(value);
                    this.databasePath = value;
                }
            }
        }

        public byte[] EventsKey { get; set; }

        public bool IsKeyProtected { get; set; }

        public bool EncryptionEnabled { get; set; }

        public string PrivateKeyPath 
        { 
            get => privateKeyPath;
            set
            {
                if (this.privateKeyPath != value)
                {
                    AssertionConcern.AssertArgumentNotEmpty(
                        value,
                        "Path specified cannot be null or empty.",
                        nameof(value)
                    );
                    AssertionConcern.AssertPathValid(value, nameof(value));
                    this.privateKeyPath = value;
                }
            }
        }
    }
}
