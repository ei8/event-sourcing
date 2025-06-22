using ei8.EventSourcing.Application;

namespace ei8.EventSourcing.Port.Adapter.IO.Process.Services
{
    public class InProcessSettingsService : ISettingsService
    {
        private string databasePath;

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
    }
}
