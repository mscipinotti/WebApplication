﻿using System.ComponentModel.DataAnnotations;

namespace WebAPP.Models
{
    public class SingerDto : Tokens
    {
        public long Id { get; set; }

        [Required]
        public string StageName { get; set; } = null!;

        [Required]
        public string Firstname { get; set; } = null!;

        [Required]
        public string Surname { get; set; } = null!;

        [Required]
        public int Age { get; set; }

        public string? Email { get; set; }

        [Required]
        public string Account { get; set; } = null!;
    }
}
