using System;
using System.Text.Json;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api;

public class ApiModrinthModSupplier : IModSupplierIntegration
{
    private readonly HttpClient client;

    public ApiModrinthModSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.modrinth.com")
        };
        client.DefaultRequestHeaders.Add("User-Agent", "nvl-ez/Wave (nahuelvazquezlevrino@gmail.com)");
    }

    public Task DownloadMod(ModVersion modVersion, string modsPath, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ModVersion>> GetModVersionsAsync(ModInfo modInfo, CancellationToken ct = default)
    {
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        string loaderString = Mapper.ToDtoModloaderType(modInfo.ModloaderType);
        queryParameters.Add("loaders", $"[\"{loaderString}\"]");
        queryParameters.Add("game_versions", $"[\"{modInfo.MinecraftVersion}\"]");

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModVersion> mods = new List<ModVersion>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v2/project/{modInfo.ModId}/version?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            List<ProjectVersionDto> dto = JsonSerializer.Deserialize<List<ProjectVersionDto>>(rootElement) ?? new List<ProjectVersionDto>();
            foreach (ProjectVersionDto versionDto in dto)
            {
                mods.Add(Mapper.ToDomain(versionDto, modInfo));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Modrinth");
        }
        return mods;
    }

    public async Task<IEnumerable<ModInfo>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct = default)
    {
        //Build Query Arguments
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();

        if (modSupplierQuery.TextQuery is not null)
            queryParameters.Add("query", modSupplierQuery.TextQuery);
        if (modSupplierQuery.Author is not null)
        {
            if (queryParameters.ContainsKey("query"))
            {
                queryParameters["query"] = $"{queryParameters["query"]} {modSupplierQuery.Author}";
            }
            else
            {
                queryParameters.Add("query", modSupplierQuery.Author);
            }
        }
        queryParameters.Add("offset", modSupplierQuery.Offset.ToString());
        queryParameters.Add("limit", modSupplierQuery.PageSize.ToString());

        string loaderString = Mapper.ToDtoModloaderType(modSupplierQuery.ModloaderType);
        string facets = $"[[\"categories:{loaderString}\"],[\"versions:{modSupplierQuery.MinecraftVersion}\"],[\"project_type:mod\"]]";
        queryParameters.Add("facets", facets);

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModInfo> mods = new List<ModInfo>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v2/search?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModsResponseDto dto = JsonSerializer.Deserialize<SearchModsResponseDto>(rootElement) ?? new SearchModsResponseDto();
            foreach (ProjectDto modDto in dto.Projects)
            {
                mods.Add(Mapper.ToDomain(modDto, modSupplierQuery));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Modrinth");
        }
        return mods;
    }
}
