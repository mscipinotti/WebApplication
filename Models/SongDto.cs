using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class SongDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int? Year { get; set; } = null!;

        public int? RecordCompany { get; set; }

        public int? Format { get; set; }
    }
}
