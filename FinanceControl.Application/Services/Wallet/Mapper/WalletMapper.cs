using AutoMapper;
using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Application.Services.Wallet.DTO_s.Response;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Application.Services.Wallet.Mapper;

public class WalletMapper : Profile
{
    public WalletMapper()
    {
        #region [ Request ]

        CreateMap<WalletInsertRequest, WalletModel>();

        #endregion

        #region [ Response ]

        CreateMap<WalletModel, WalletResponse>();
        CreateMap<WalletModel, OptimizeIncomeResponse>();
        CreateMap<OptimizeIncomeModel, OptimizeIncomeResponse>();

        #endregion
    }
}