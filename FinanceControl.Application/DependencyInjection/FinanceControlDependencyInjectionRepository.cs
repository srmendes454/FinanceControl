using FinanceControl.Application.Services.AccountBank.Repository;
using FinanceControl.Application.Services.BankSlip.Repository;
using FinanceControl.Application.Services.CardBill.Repository;
using FinanceControl.Application.Services.Cards.Repository;
using FinanceControl.Application.Services.Division.Repository;
using FinanceControl.Application.Services.Transactions.Repository;
using FinanceControl.Application.Services.User.Repository;
using FinanceControl.Application.Services.Wallet.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinanceControl.Application.DependencyInjection;

public static class FinanceControlDependencyInjectionRepository
{
    public static void ConfigureRepositoriesDependencyInjection(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardBillRepository, CardBillRepository>();
        services.AddScoped<IBankSlipRepository, BankSlipRepository>();
        services.AddScoped<IAccountBankRepository, AccountBankRepository>();
        services.AddScoped<ITransactionsRepository, TransactionsRepository>();
        services.AddScoped<IDivisionRepository, DivisionRepository>();
    }
}