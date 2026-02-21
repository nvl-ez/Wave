using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public class PaginationDto
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }
    [JsonPropertyName("resultCount")]
    public int ResultCount { get; set; }
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}
