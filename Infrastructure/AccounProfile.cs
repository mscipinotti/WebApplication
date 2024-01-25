using AutoMapper;
using WebAPP.Models;

namespace WebAPP.Infrastructure
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountDto, SingersDto>();
            CreateMap<SingersDto, AccountDto>();
        }
    }
}
