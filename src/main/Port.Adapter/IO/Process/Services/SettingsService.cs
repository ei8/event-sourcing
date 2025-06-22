using ei8.EventSourcing.Application;
using ei8.EventSourcing.Port.Adapter.Common;
using System;

namespace ei8.EventSourcing.Port.Adapter.IO.Process.Services
{
    public class SettingsService : ISettingsService
    {
        public string DatabasePath { get; set; }
            = Helper.ValidateDatabasePath(
                Environment.GetEnvironmentVariable(EnvironmentVariableKeys.DatabasePath)
            );
    }
}
