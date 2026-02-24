using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public class SearchModFileDto
{
    [JsonPropertyName("id")]
    public List<ModFileDto> Data { get; set; } = new List<ModFileDto>();
    [JsonPropertyName("id")]
    public PaginationDto Pagination { get; set; } = new PaginationDto()
    {
        Index = -1,
        PageSize = 0,
        ResultCount = 0,
        TotalCount = 0
    };
}
