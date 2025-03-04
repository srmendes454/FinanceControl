using FinanceControl.Application.Services.BankSlip.Service;
using FinanceControl.Application.Services.CardBill.Service;
using FinanceControl.Application.Services.Cards.Service;
using FinanceControl.Application.Services.Division.Service;
using FinanceControl.Application.Services.AccountBank.Service;
using FinanceControl.Application.Services.Transactions.Service;
using FinanceControl.Application.Services.User.Service;
using FinanceControl.Application.Services.Wallet.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinanceControl.Application.DependencyInjection;

public static class FinanceControlDependencyInjectionService
{
    public static void ConfigureServicesDependencyInjection(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<ICardService, CardService>();
        services.AddScoped<ICardBillService, CardBillService>();
        services.AddScoped<IBankSlipService, BankSlipService>();
        services.AddScoped<IAccountBankService, AccountBankService>();
        services.AddScoped<ITransactionsService, TransactionsService>();
        services.AddScoped<IDivisionService, DivisionService>();
    }
}