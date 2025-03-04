using System.ComponentModel;

namespace FinanceControl.Domain.Enuns;

public enum ExpenseType
{
    [Description("Açougue")]
    BUTCHER,

    [Description("Casa")]
    HOUSE,

    [Description("Carro")]
    CAR,

    [Description("Carro por Aplicativo")]
    CAR_FOR_APP,

    [Description("Doação")]
    DONATION,

    [Description("Farmácia")]
    PHARMACY,

    [Description("Feira")]
    FAIR,

    [Description("Games")]
    GAME,

    [Description("Lanches")]
    LANCHE,

    [Description("Mercado")]
    MARKET,

    [Description("Escritório")]
    OFFICE,

    [Description("Imposto")]
    TAX,

    [Description("Internet")]
    INTERNET,

    [Description("Investimento")]
    INVESTMENT,

    [Description("Reserva de Emergência")]
    EMERGENCY_RESERVE,

    [Description("Saúde")]
    HEALTH,

    [Description("Streaming")]
    STREAMING,

    [Description("Viagem")]
    TRIP,

    [Description("Salão de Beleza")]
    BEAUTY_SALON,

    [Description("Moda")]
    FASHION,

    [Description("Esportes")]
    SPORTS,

    [Description("Bebê")]
    BABY,

    [Description("Educação")]
    EDUCATION,

    [Description("Lazer")]
    LEISURE,

    [Description("Outros")]
    OTHERS,

    [Description("Segurança")]
    SECURITY,

    [Description("Transporte")]
    TRANSPORT,

    [Description("Igreja")]
    CHURCH,

    [Description("Eletronicos")]
    ELECTRONICS,

    [Description("Presente")]
    GIFT,

    [Description("Documento")]
    DOCUMENT,

    [Description("Casamento")]
    WEDDING,

    [Description("Mobília")]
    FURNITURE,

    [Description("Construção")]
    CONSTRUCTION,

    [Description("Oficina Mecânica")]
    MECHANIC,

    [Description("Parcelamentos")]
    INSTALLMENTS,

    [Description("Barbearia")]
    BARBER_SHOP,

    [Description("Salário")]
    SALARY,

    [Description("Pró-Labore")]
    PRO_LABORE,

    [Description("Provento")]
    INCOME,

    [Description("Resgate de Investimentos")]
    INVESTMENT_RESCUE
}