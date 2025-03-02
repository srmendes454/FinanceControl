using FinanceControl.Application.Services.BankSlip.Service;
using FinanceControl.Application.Services.CardBill.Service;
using FinanceControl.Application.Services.Cards.Service;
using FinanceControl.Application.Services.Pix.Service;
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
        services.AddScoped<IPixService, PixService>();
        services.AddScoped<ITransactionsService, TransactionsService>();
    }
}