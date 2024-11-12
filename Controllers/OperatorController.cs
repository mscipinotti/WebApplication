using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Net;
using WebApp.Infrastructure.Models.dto;
using WebApp.Infrastructure.Utilities;
using WebAPP.Extensions;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.MiddlewareFactory;
using WebAPP.Services;

namespace WebAPP.Controllers
{
    [Route("[Controller]")]
    public class OperatorController(ILogger logger, HttpClientFactory httpClientFactory, IMapper mapper, IStringLocalizer<UserController> localizer, ILanguage language) : UserController(logger, httpClientFactory, mapper, localizer, language)
    {
        [HttpPost("UpdateBiography")]
        public async Task<IActionResult> UpdateBiographyAsync(BiographyDto biography) => await CallAsync(biography, "UpdateBiography");

        [HttpPost("DeleteResponsibility")]
        public async Task<IActionResult> DeleteResponsibilityAsync(BiographyDto biography) => await CallAsync(biography, "DeleteResponsibility");

        [HttpPost("DeleteWork")]
        public async Task<IActionResult> DeleteWorkAsync(BiographyDto biography) => await CallAsync(biography, "DeleteWork");

        private async Task<IActionResult> CallAsync(BiographyDto biography, string action)
        {
            try
            {
                _httpClient.SetTokens(biography);
                var httpResponseMessage = await _httpClient.PostAsJsonAsync($"{GlobalParameters.Config.GetValue<string>("apiURL")!}Operator/{action}", biography);
                if (httpResponseMessage.StatusCode != HttpStatusCode.OK) throw new BadHttpRequestException(httpResponseMessage.Content.ReadAsStream().ToString() ?? string.Empty);
                return await Task.Run(async () => View("Biography", await httpResponseMessage.Content.ReadFromJsonAsync<BiographyDto>()));
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(_logger, _configLogger, ex.Message);
                biography.Errors = [ex.Message];
                return View($"Biography", biography);
            }
        }
    }
}
