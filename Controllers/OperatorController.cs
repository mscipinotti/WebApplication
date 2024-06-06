using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApp.Infrastructure.Models.dto;
using WebAPP.Extensions;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.MiddlewareFactory;
using WebAPP.Utilities;

namespace WebAPP.Controllers
{
    [Route("[Controller]")]
    public class OperatorController : Controller
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, object> _configLogger;
        private readonly CancellationTokenSource _ct;
        public OperatorController(ILogger logger, HttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
            _ct = new CancellationTokenSource();
        }

        [HttpPost("Index")]
        public async Task<IActionResult> IndexAsync(Tokens token)
        {
            try
            {
                _httpClient.SetTokens(token);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}User/Index", token);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<LibraryDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
                return View(token);
            }
        }
    }
}
