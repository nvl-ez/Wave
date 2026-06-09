using System;

namespace Wave.Domain.Utils;

public record class PaginationState
{
    public int Index { get; set; } = -1;
    public int PageSize { get; set; } = 20;
    public int ResultCount { get; set; } = 0;
    public int TotalCount { get; set; } = 0;
}
