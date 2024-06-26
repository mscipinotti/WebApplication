﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;
using WebApp.Infrastructure.Models.dto;
using WebAPP.Extensions;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.MiddlewareFactory;
using WebAPP.Utilities;

namespace WebAPP.Controllers
{
    [Route("[Controller]")]
    public class OperatorController(ILogger logger, HttpClientFactory httpClientFactory, IMapper mapper, IStringLocalizer<UserController> localizer) : UserController(logger, httpClientFactory, mapper, localizer)
    {
        [HttpPost("UpdateBiography")]
        public async Task<IActionResult> UpdateBiographyAsync(BiographyDto biography)
        {
            try
            {
                _httpClient.SetTokens(biography);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}Operator/updateBiography", biography);
                if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View(await httpResponseMessage.Content.ReadFromJsonAsync<LibraryDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                biography.Errors = [ex.Message];
                return View(biography);
            }
        }


    }
}
