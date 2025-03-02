using System;

namespace FinanceControl.Infra.RequestContainer;

public interface IRequestContainer
{
    Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}