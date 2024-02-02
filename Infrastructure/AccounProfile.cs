using AutoMapper;
using WebAPP.Infrastructure.Models;

namespace WebAPP.Infrastructure.Infrastructure
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
