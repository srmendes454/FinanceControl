using AutoMapper;
using FinanceControl.Application.Services.Transactions.DTO_s.Request;
using FinanceControl.Application.Services.Transactions.DTO_s.Response;
using FinanceControl.Application.Services.Transactions.Model;

namespace FinanceControl.Application.Services.Transactions.Mapper
{
    public class TransactionsMapper : Profile
    {
        public TransactionsMapper()
        {
            #region [ Request ]

            CreateMap<TransactionsInsertRequest, TransactionsModel>();
            CreateMap<TransactionsRepetitionInsertRequest, RepetitionModel>();

            #endregion

            #region [ Response ]

            CreateMap<TransactionsModel, TransactionsResponse>();
            CreateMap<RepetitionModel, TransactionsRepetitionResponse>();
            CreateMap<PaymentDetailsModel, TransactionsPaymentDetailsResponse>();

            #endregion
        }
    }
}
