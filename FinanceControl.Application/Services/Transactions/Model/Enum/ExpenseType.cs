using System.ComponentModel;

namespace FinanceControl.Application.Services.Transactions.Model.Enum;

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

    [Description("Eletronico")]
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
    CONSTRUCTION
}