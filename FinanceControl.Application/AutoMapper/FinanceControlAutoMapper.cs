using Microsoft.Extensions.DependencyInjection;
using System;
using FinanceControl.Application.Services.Cards.Mapper;
using FinanceControl.Application.Services.Transactions.Mapper;
using FinanceControl.Application.Services.User.Mapper;
using FinanceControl.Application.Services.Wallet.Mapper;
using FinanceControl.Application.Services.BankSlip.Mapper;
using FinanceControl.Application.Services.Pix.Mapper;
using FinanceControl.Application.Services.CardBill.Mapper;

namespace FinanceControl.Application.AutoMapper
{
    public static class FinanceControlAutoMapper
    {
        public static void FinanceControlAutoMapperConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(UserMapper));
            services.AddAutoMapper(typeof(WalletMapper));
            services.AddAutoMapper(typeof(CardMapper));
            services.AddAutoMapper(typeof(CardBillMapper));
            services.AddAutoMapper(typeof(TransactionsMapper));
            services.AddAutoMapper(typeof(BankSlipMapper));
            services.AddAutoMapper(typeof(PixMapper));
        }
    }
}
