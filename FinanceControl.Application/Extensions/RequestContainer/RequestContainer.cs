using System;

namespace FinanceControl.Application.Extensions.RequestContainer;

public class RequestContainer : IRequestContainer
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}