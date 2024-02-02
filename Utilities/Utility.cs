namespace WebAPP.Utilities
{
    public static class WriteLog
    {
        public static void WriteInfoLog(this ILogger logger, Dictionary<string, object> scope, string message)
        {
            using (logger.BeginScope(scope)) logger.LogInformation(message);
        }

        public static void WriteWarnLog(this ILogger logger, Dictionary<string, object> scope, string message)
        {
            using (logger.BeginScope(scope)) logger.LogWarning(message);
        }

        public static void WriteErrorLog(this ILogger logger, Dictionary<string, object> scope, string message)
        {
            using (logger.BeginScope(scope)) logger.LogError(message);
        }
    }
}
