﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.Net;
using WebApp.Infrastructure.Models.dto;
using WebAPP.Extensions;
using WebAPP.Infrastructure;
using WebAPP.MiddlewareFactory;
using WebAPP.Services;
using WebAPP.Infrastructure.Utilities;
using WebApp.Infrastructure.Utilities;

namespace WebAPP.Controllers
{
    // Il routing è indispensabile per i bottoni che specificano asp-controller. Se non fosse presente l'interpretazione in HTML5 sarebbe con la action ma senza il controller andando a puntare quindi in HomeController e restituendo un NotFound.
    [Route("[Controller]")]
    public class UserController : Controller, IActionFilter
    {
        protected readonly ILogger _logger;
        protected readonly HttpClient _httpClient;
        protected readonly IMapper _mapper;
        protected readonly Dictionary<string, object> _configLogger;
        protected readonly IStringLocalizer<UserController> _localizer;
        protected readonly CancellationTokenSource _ct;
        protected readonly ILanguage _language;

    public UserController(ILogger logger, HttpClientFactory httpClientFactory, IMapper mapper, IStringLocalizer<UserController> localizer, ILanguage language)
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
            _language = language;
        }

        [HttpPost("Index")]
        public async Task<IActionResult> IndexAsync(Tokens token, CancellationToken ct)
        {
            try
            {
                _httpClient.SetTokens(token);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}User/Index", token, ct);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream(ct).ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<LibraryDto>(ct)));
            }
            catch(Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                token.Errors = [ex.Message];
                return View(token);
            }
        }

        [HttpPost("Biography")]
        public async Task<IActionResult> BiographyAsync(BiographyDto biographyDto, int? Author)
        {
            // Con asp-route- è possibile passare solo tipi primitivi
            try
            {
                if (Author is not null)
                {
                    biographyDto.Author = new()
                    {
                        Id = (int)Author
                    };
                }
                biographyDto.Language = _language.UserLanguage;
                _httpClient.SetTokens(biographyDto);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}User/Biography", biographyDto);  
                if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound) throw new BadHttpRequestException((await httpResponseMessage.Content.ReadFromJsonAsync<BiographyDto>())!.Errors![0]);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<BiographyDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                return View(biographyDto);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Request.Cookies[".AspNetCore.Culture"]
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
