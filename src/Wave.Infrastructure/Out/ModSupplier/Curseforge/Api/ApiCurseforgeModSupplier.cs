using System;
using System.Text.Json;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager.Modloader;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api;

public class ApiCurseforgeModSupplier : IModSupplierIntegration
{
    private readonly HttpClient client;
    private const int MinecraftGameId = 432;
    private const int ModClassId = 6;

    public ApiCurseforgeModSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.curseforge.com")
        };
        client.DefaultRequestHeaders.Add("x-api-key", "$2a$10$BGG5jB6kIf.QgqGtFOKEWuscWzRGs.YsZ3YXp1YJ7.0PW9i4CzmAe");
    }

    public async Task<IEnumerable<ModVersion>> GetModVersionsAsync(ModInfo modInfo, CancellationToken ct = default)
    {
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        queryParameters.Add("gameVersion", modInfo.MinecraftVersion);
        ModloaderType loader = modInfo.ModloaderType;
        queryParameters.Add("modLoaderType", Mapper.ToDtoModloaderType(loader).ToString());
        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModVersion> mods = new List<ModVersion>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v1/mods/{modInfo.ModId}/files?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModFileDto dto = JsonSerializer.Deserialize<SearchModFileDto>(rootElement) ?? new SearchModFileDto();
            foreach (ModFileDto modDto in dto.Data)
            {
                mods.Add(Mapper.ToDomain(modDto, modInfo));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Curseforge");
        }
        return mods;
    }

    public async Task<IEnumerable<ModInfo>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct = default)
    {
        //Build Query Arguments
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();

        if (modSupplierQuery.TextQuery is not null)
            queryParameters.Add("searchFilter", modSupplierQuery.TextQuery);

        if (modSupplierQuery.Author is not null)
        {
            if (queryParameters.ContainsKey("searchFilter"))
            {
                queryParameters["searchFilter"] = $"{queryParameters["searchFilter"]} {modSupplierQuery.Author}";
            }
            else
            {
                queryParameters.Add("searchFilter", modSupplierQuery.Author);
            }
        }
        queryParameters.Add("gameVersion", modSupplierQuery.MinecraftVersion);

        ModloaderType loader = modSupplierQuery.ModloaderType;
        queryParameters.Add("modLoaderType", Mapper.ToDtoModloaderType(loader).ToString());

        queryParameters.Add("index", modSupplierQuery.Offset.ToString());
        queryParameters.Add("pageSize", modSupplierQuery.PageSize.ToString());
        queryParameters.Add("gameId", $"{MinecraftGameId}");
        queryParameters.Add("classId", $"{ModClassId}");

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        //HTTP Request
        List<ModInfo> mods = new List<ModInfo>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v1/mods/search?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModsResponseDto dto = JsonSerializer.Deserialize<SearchModsResponseDto>(rootElement) ?? new SearchModsResponseDto();
            foreach (ModInfoDto modDto in dto.Mods)
            {
                mods.Add(Mapper.ToDomain(modDto, modSupplierQuery));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Curseforge");
        }
        return mods;
    }

    public Task DownloadMod(ModVersion modVersion, string modsPath, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
