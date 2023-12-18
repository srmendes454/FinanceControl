using AutoMapper;
using FinanceControl.Application.Extensions.Enum;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Cards.DTO_s;
using System;

namespace FinanceControl.Application.Services.Cards.Mapper;

public class CardMapper : Profile
{
    public CardMapper()
    {
        #region [ Request ]



        #endregion

        #region [ Response ]

        CreateMap<CardModel, CardResponse>()
            .ForPath(dest => dest.Type, src => src.MapFrom(x => x.Type.GetEnumDescription()));

        CreateMap<CardWalletModel, CardWalletResponse>();

        #endregion
    }
}