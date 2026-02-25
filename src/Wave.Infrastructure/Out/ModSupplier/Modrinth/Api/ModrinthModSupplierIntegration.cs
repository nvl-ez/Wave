using System;
using System.Text.Json;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Mappers;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api;

public class ModrinthModSupplierIntegration : IModSupplierIntegration
{
    private readonly HttpClient client;

    public ModrinthModSupplierIntegration()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.modrinth.com")
        };
        client.DefaultRequestHeaders.Add("User-Agent", "nvl-ez/Wave (nahuelvazquezlevrino@gmail.com)");
    }

    public async Task<IEnumerable<Mod>> GetModFilesAsync(ModInfoResult mod, CancellationToken ct)
    {
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        ModloaderType loader = mod.ModloaderType;
        string loaderString = loader == ModloaderType.Forge ? "forge" :
                (loader == ModloaderType.Fabric ? "fabric" : throw new NotImplementedException("Missing implementation for modloader"));
        string facets = $"[[\"loaders:{loaderString}\"],[\"game_versions:{mod.MinecraftVersion.Version}\"]]";
        queryParameters.Add("facets", facets);

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<Mod> mods = new List<Mod>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v2/project/{mod.ExternalId}/version?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            List<ProjectVersionDto> dto = JsonSerializer.Deserialize<List<ProjectVersionDto>>(rootElement) ?? new List<ProjectVersionDto>();
            foreach (ProjectVersionDto versionDto in dto)
            {
                foreach (FileDto fileDto in versionDto.Files)
                {

                }
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Modrinth");
        }
    }

    public async Task<IEnumerable<ModInfoResult>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct)
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

        ModloaderType loader = modSupplierQuery.ModloaderType;
        string loaderString = loader == ModloaderType.Forge ? "forge" :
                (loader == ModloaderType.Fabric ? "fabric" : throw new NotImplementedException("Missing implementation for modloader"));
        string facets = $"[[\"categories:{loaderString}\"],[\"versions:{modSupplierQuery.MinecraftVersion.Version}\"],[\"project_type:mod\"]]";
        queryParameters.Add("facets", facets);

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModInfoResult> mods = new List<ModInfoResult>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v2/search?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModsResponseDto dto = JsonSerializer.Deserialize<SearchModsResponseDto>(rootElement) ?? new SearchModsResponseDto();
            foreach (ProjectDto modDto in dto.Projects)
            {
                mods.Add(ProjectDtoMapper.ToDomain(modDto, modSupplierQuery));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Modrinth");
        }
        return mods;
    }
}
