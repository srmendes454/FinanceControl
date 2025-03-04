using AutoMapper;
using FinanceControl.Application.Services.AccountBank.DTO_s.Response;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;

namespace FinanceControl.Application.Services.AccountBank.Mapper
{
    public class AccountBankMapper : Profile
    {
        public AccountBankMapper()
        {
            #region [ Request ]

            #endregion

            #region [ Response ]

            CreateMap<AccountBankModel, AccountBankResponse>()
                .ForPath(dest => dest.Type, src => src.MapFrom(x => x.Type.GetEnumDescription()));

            #endregion
        }
    }
}
