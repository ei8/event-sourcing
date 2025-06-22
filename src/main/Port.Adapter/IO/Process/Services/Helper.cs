using neurUL.Common.Domain.Model;

namespace ei8.EventSourcing.Port.Adapter.IO.Process.Services
{
    public static class Helper
    {
        public static string ValidateDatabasePath(string value)
        {
            AssertionConcern.AssertArgumentNotEmpty(
                value,
                "Path specified cannot be null or empty.",
                nameof(value)
            );

            if (!value.Contains(":memory:"))
                AssertionConcern.AssertPathValid(value, nameof(value));

            return value;
        }
    }
}
