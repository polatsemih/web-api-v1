namespace SUPBank.Domain.Contstants
{
    public class ExceptionMessages
    {
        // Exceptions
        public const string Exception = "Unexpected error occurred";
        public const string SqlException = "SQL error occurred";
        public const string OperationCanceledException = "Operation was canceled while opening the SQL connection.";

        // Database Exceptions
        public const string SqlConnectionStringInvalid = "Connection string not found";
        public const string RedisConnectionStringInvalid = "Redis connection string not found";
        public const string RedisConnectionError = "Redis connection cannot established";
    }
}
