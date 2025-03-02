using AutoMapper;
using FinanceControl.Cards.DTO_s;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;

namespace FinanceControl.Application.Services.Cards.Mapper;

public class CardMapper : Profile
{
    public CardMapper()
    {
        #region [ Request ]



        #endregion

        #region [ Response ]

        CreateMap<CardModel, CardResponse>()
            .ForPath(dest => dest.Type, src => src.MapFrom(x => x.Type.GetEnumDescription()))
            .ForPath(dest => dest.StatusCardBill, src => src.MapFrom(x => Status.OPEN.GetEnumDescription()));

        CreateMap<CardWalletModel, CardWalletResponse>();

        #endregion
    }
}