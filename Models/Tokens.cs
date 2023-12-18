namespace WebAPP.Models
{
    public class Tokens
    {
        // Antiforgery token
        public string? RequestVerificationToken { get; set; }
        public string? Cookie { get; set; }

        // Jwt token
        public string? JwtToken { get; set; }
    }
}
