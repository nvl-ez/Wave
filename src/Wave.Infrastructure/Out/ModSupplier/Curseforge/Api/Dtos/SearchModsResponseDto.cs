using System;
using System.Text.Json.Serialization;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

public record class SearchModsResponseDto
{
    [JsonPropertyName("data")]
    public List<ModDto> Mods { get; set; } = new List<ModDto>();
    [JsonPropertyName("pagination")]
    public PaginationDto Pagination { get; set; } = new PaginationDto()
    {
        Index = -1,
        PageSize = 0,
        ResultCount = 0,
        TotalCount = 0
    };
}
