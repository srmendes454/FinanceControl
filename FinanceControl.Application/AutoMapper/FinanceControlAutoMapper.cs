using Microsoft.Extensions.DependencyInjection;
using System;
using FinanceControl.Application.Services.Cards.Mapper;
using FinanceControl.Application.Services.Transactions.Mapper;
using FinanceControl.Application.Services.User.Mapper;
using FinanceControl.Application.Services.Wallet.Mapper;

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
            services.AddAutoMapper(typeof(TransactionsMapper));
        }
    }
}
