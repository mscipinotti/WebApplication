using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class AccountDto : Tokens
    {
        [Required]
        public string Login { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public int? Age { get; set; }

        public string Cf { get; set; } = null!;

        public string Email { get; set; } = null!;
        public bool RememeberMe { get; set; }
    }
}
