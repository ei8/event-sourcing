using ei8.EventSourcing.Application;
using ei8.EventSourcing.Port.Adapter.Common;
using Microsoft.AspNetCore.DataProtection;
using neurUL.Common.Domain.Model;
using System;

namespace ei8.EventSourcing.Port.Adapter.IO.Process.Services
{
    public class SettingsService : ISettingsService
    {
        private string privateKeyPath;

        public SettingsService(IDataProtector dataProtector)
        {
            this.EventsKey = null;
            this.DatabasePath = Helper.ValidateDatabasePath(
                Environment.GetEnvironmentVariable(EnvironmentVariableKeys.DatabasePath)
            );
            this.IsKeyProtected = false;
            this.EncryptionEnabled = bool.TryParse(
                Environment.GetEnvironmentVariable(EnvironmentVariableKeys.EncryptionEnabled), 
                out bool bee
            ) ? bee : false;
            this.PrivateKeyPath = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.PrivateKeyPath);
        }

        public byte[] EventsKey { get; set; }

        public string DatabasePath { get; set; }

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
