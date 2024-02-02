using AutoMapper;
using WebAPP.Models;

namespace WebAPP.Infrastructure
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Tokens, SingersDto>();
            CreateMap<SingersDto, Tokens>();

            CreateMap<Tokens, SongsDto>();
            CreateMap<SongsDto, Tokens>();
        }
    }
}
