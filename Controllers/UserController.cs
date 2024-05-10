using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebAPP.Extensions;
using WebAPP.Infrastructure.Models;
using WebAPP.MiddlewareFactory;
using WebAPP.Utilities;

namespace WebAPP.Infrastructure.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, object> _configLogger;
        private readonly CancellationTokenSource _ct;

    public UserController(ILogger logger, HttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
            _ct = new CancellationTokenSource();
        }

        [HttpPost("Recent")]
        public async Task<IActionResult> RecentAsync(Tokens token)
        {
            try
            {
                _httpClient.SetTokens(token);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.GlobalParameters.Config.GetValue<string>("apiURL")!}User/Recent", token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<AccountsDto>()));
            }
            catch(Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
                return View(token);
            }
        }

    }
}
