namespace WebAPP.Infrastructure
{
    public static class GlobalParameters
    {
        public static IConfiguration Config { get; } = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                                                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
                                                                                 .Build();
    }
}
