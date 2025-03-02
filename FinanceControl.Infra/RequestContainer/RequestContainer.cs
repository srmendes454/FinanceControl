using System;

namespace FinanceControl.Infra.RequestContainer;

public class RequestContainer : IRequestContainer
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}