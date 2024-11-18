using WebAPP.Infrastructure;

namespace WebAPP.MiddlewareFactory
{
    public class HttpClientFactory
    {
        public HttpClient Client
        {
            get
            {
                HttpClient _httpClient = new ()
                {
                    BaseAddress = new Uri(GlobalParameters.Config.GetValue<string>("apiURL")!),
                    Timeout = TimeSpan.FromSeconds(300),

                };
                //_httpClient.DefaultRequestHeaders.Add("Accept-Language", "it");
                //_httpClient.DefaultRequestHeaders.Add("Content-Type", "application/json; charset=UTF-8");
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                return _httpClient;
            }
        }
    }
}
