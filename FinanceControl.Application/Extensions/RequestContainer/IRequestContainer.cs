using System;

namespace FinanceControl.Application.Extensions.RequestContainer;

public interface IRequestContainer
{
    Guid UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}