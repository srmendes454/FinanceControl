using AutoMapper;
using FinanceControl.Application.Services.CardBill.DTO_s.Request;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Enuns;

namespace FinanceControl.Application.Services.CardBill.Mapper
{
    public class CardBillMapper : Profile
    {
        public CardBillMapper()
        {
            #region [ Response ]

            CreateMap<CardBillModel, CardBillResponse>()
                .ForPath(dest => dest.Status, src => src.MapFrom(x => x.Status.GetEnumDescription()));

            #endregion
        }
    }
}
