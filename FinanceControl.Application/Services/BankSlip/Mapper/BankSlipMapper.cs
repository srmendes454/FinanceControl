using AutoMapper;
using FinanceControl.Application.Services.BankSlip.DTO_s.Response;
using FinanceControl.Application.Services.BankSlip.Model;

namespace FinanceControl.Application.Services.BankSlip.Mapper
{
    public class BankSlipMapper : Profile
    {
        public BankSlipMapper()
        {
            #region [ Request ]

            #endregion

            #region [ Response ]

            CreateMap<BankSlipModel, BankSlipResponse>();

            #endregion
        }
    }
}
