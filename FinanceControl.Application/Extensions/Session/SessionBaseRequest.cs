using System;

namespace FinanceControl.WebApi.Extensions.Session;

public class SessionBaseRequest
{
    public Guid AuthenticatedUserId { get; set; }
    public Guid CurrentAccountId { get; set; }
}