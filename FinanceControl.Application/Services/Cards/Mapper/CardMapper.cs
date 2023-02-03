using AutoMapper;
using FinanceControl.Application.Services.Cards.Model;
using FinanceControl.Cards.DTO_s;

namespace FinanceControl.Application.Services.Cards.Mapper;

public class CardMapper : Profile
{
    public CardMapper()
    {
        #region [ Request ]



        #endregion

        #region [ Response ]

        CreateMap<CardModel, CardResponse>();
        CreateMap<CardWalletModel, CardWalletResponse>();

        #endregion
    }
}