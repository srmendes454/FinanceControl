using System.Collections.Generic;

namespace FinanceControl.Extensions.Paginated;

public class PaginatedResponse<T>
{
    public List<T> Records { get; set; }
    public long Total { get; set; }
}