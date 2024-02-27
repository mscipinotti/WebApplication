using WebAPP.Infrastructure.GlobalParameters;

namespace WebAPP.MiddlewareFactory
{
    public class HttpClientFactory
    {
        public HttpClient Client
        {
            get => new()
            {
                BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!),
                Timeout = TimeSpan.FromSeconds(300)
            };
        }
    }
}
