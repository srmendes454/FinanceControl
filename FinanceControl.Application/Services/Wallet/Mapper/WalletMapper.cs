﻿using AutoMapper;
using FinanceControl.Application.Services.Wallet.DTO_s.Request;
using FinanceControl.Application.Services.Wallet.DTO_s.Response;
using FinanceControl.Application.Services.Wallet.Model;

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

        #endregion
    }
}