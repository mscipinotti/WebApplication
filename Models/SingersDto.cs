﻿namespace WebAPP.Models
{
    public class SingersDto : Tokens
    {
        public List<SingerDto> Singers { get; set; } = new List<SingerDto>();
    }
}
