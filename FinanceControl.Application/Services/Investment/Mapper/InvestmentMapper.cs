using AutoMapper;
using FinanceControl.Application.Services.Investment.DTO_s.Response;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;

namespace FinanceControl.Application.Services.Investment.Mapper
{
    public class InvestmentMapper : Profile
    {
        public InvestmentMapper()
        {
            #region [ Request ]

            #endregion

            #region [ Response ]

            CreateMap<InvestmentModel, InvestmentResponse>()
                .ForPath(dest => dest.Type, src => src.MapFrom(x => x.Type.GetEnumDescription()));

            #endregion
        }
    }
}
