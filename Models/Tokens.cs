using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class Tokens
    {
        [Required]
        public string Login { get; set; } = null!;

        // Antiforgery token
        public string? RequestVerificationToken { get; set; }
        public string? Cookie { get; set; }

        // Jwt token
        public string? JwtToken { get; set; }
    }
}
