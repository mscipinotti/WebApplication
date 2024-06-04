using System.Globalization;
using System.Net.Http.Headers;
using WebApp.Infrastructure.Models.dto;

namespace WebAPP.Extensions
{
    public static class HttpClientExtension
    {
        public static void SetTokens(this HttpClient client, Tokens token)
        {
            // HttpClient.DefaultRequestHeaders(and BaseAddress) should only be set once, before you make any requests.HttpClient is only safe to use as a singleton if you don't modify it once it's in use.
            // Si potevano usare gli Headers di HttpRequestMessage.Headers (ad es. request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token)).

            // Antiforgery token
            client.DefaultRequestHeaders.Add("X-XSRF-TOKEN", $"{token.RequestVerificationToken}");
            client.DefaultRequestHeaders.Add("Cookie", $"{token.Cookie}");

            // Jwt token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.JwtToken);

            // Per trasferire al server la scelta della lingua e rispondere con eventuali errori nella lingua specificata
            client.DefaultRequestHeaders.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
        }
    }
}
