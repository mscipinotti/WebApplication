using System.Net.Http.Headers;
using System.Net.Http;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.Infrastructure.Models;

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
