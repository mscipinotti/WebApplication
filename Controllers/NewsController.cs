using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using WebApp.Infrastructure.Models.dto;
using WebApp.Infrastructure.Utilities;
using WebAPP.MiddlewareFactory;

namespace WebAPP.Controllers
{
    // Il routing è indispensabile per i bottoni che specificano asp-controller. Se non fosse presente l'interpretazione in HTML5 sarebbe con la action ma senza il controller andando a puntare quindi in HomeController e restituendo un NotFound.
    [Route("[Controller]")]
    public class NewsController : Controller, IActionFilter
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly Dictionary<string, object> _configLogger;
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly CancellationTokenSource _ct;

        public NewsController(ILogger logger, HttpClientFactory httpClientFactory, IMapper mapper, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _httpClient = httpClientFactory.Client;
            _mapper = mapper;
            _configLogger = new()
            {
                { "OperationId", Guid.NewGuid() }
            };
            _ct = new CancellationTokenSource();
            _localizer = localizer;
        }

        [HttpPost("Index")]
        public async Task<IActionResult> IndexAsync(Tokens token)
        {
            try
            {
                return await Task.Run(() => View(token));
            }
            catch(Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
                return View(token);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Questi due metodi permettono di scrivere nel log quando comincia una chiamata e quando termina senza intasare il codice della action stessa e di validare il contesto. Il log è centralizzato
            try
            {
                if (!context.ModelState.IsValid)
                {
                    WriteLog.WriteErrorLog(_logger, _configLogger, $"Invalid context for {context.ActionDescriptor.DisplayName} action");
                }

                _configLogger["Action"] = context.ActionDescriptor.DisplayName ?? string.Empty;

                using (_logger.BeginScope(_configLogger)) WriteLog.WriteInfoLog(_logger, _configLogger, "Calling {Action} ...");
            }
            catch (ArgumentException ex)
            {
                WriteLog.WriteInfoLog(_logger, _configLogger, $"Something went wrong, probably token validation: {ex.Message}");
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            using (_logger.BeginScope(_configLogger)) WriteLog.WriteInfoLog(_logger, _configLogger, "{Action} call ended");
        }
    }
}
