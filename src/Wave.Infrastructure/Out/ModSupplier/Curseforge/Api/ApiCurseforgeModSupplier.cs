using System;
using System.Net;
using System.Text.Json;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Domain.ServerManager.Modloader;
using Wave.Domain.Utils;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api;

public class ApiCurseforgeModSupplier : IModSupplierIntegration
{
    private readonly HttpClient client;
    private const int MinecraftGameId = 432;
    private const int ModClassId = 6;

    public ModSupplierType ModSupplierType { get => ModSupplierType.Curseforge; }

    public ApiCurseforgeModSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.curseforge.com")
        };
        client.DefaultRequestHeaders.Add("x-api-key", "$2a$10$BGG5jB6kIf.QgqGtFOKEWuscWzRGs.YsZ3YXp1YJ7.0PW9i4CzmAe");
    }
    public bool CanHandle(ModSupplierType modSupplierType)
    {
        return modSupplierType == ModSupplierType;
    }

    public async Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modInfoSupplierQuery, CancellationToken ct = default)
    {
        //Build Query Arguments
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();

        if (modInfoSupplierQuery.TextQuery is not null)
            queryParameters.Add("searchFilter", modInfoSupplierQuery.TextQuery);

        if (modInfoSupplierQuery.Author is not null)
        {
            if (queryParameters.ContainsKey("searchFilter"))
            {
                queryParameters["searchFilter"] = $"{queryParameters["searchFilter"]} {modInfoSupplierQuery.Author}";
            }
            else
            {
                queryParameters.Add("searchFilter", modInfoSupplierQuery.Author);
            }
        }
        queryParameters.Add("gameVersion", modInfoSupplierQuery.MinecraftVersion);

        ModloaderType loader = modInfoSupplierQuery.ModloaderType;
        queryParameters.Add("modLoaderType", Mapper.ToDtoModloaderType(loader).ToString());

        queryParameters.Add("index", modInfoSupplierQuery.PaginationState.Index.ToString());
        queryParameters.Add("pageSize", modInfoSupplierQuery.PaginationState.PageSize.ToString());
        queryParameters.Add("gameId", $"{MinecraftGameId}");
        queryParameters.Add("classId", $"{ModClassId}");

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        //HTTP Request
        List<ModInfo> mods = new List<ModInfo>();
        PaginationState paginationState = new();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v1/mods/search?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModsResponseDto dto = JsonSerializer.Deserialize<SearchModsResponseDto>(rootElement) ?? new SearchModsResponseDto();
            foreach (ModInfoDto modDto in dto.Mods)
            {
                mods.Add(Mapper.ToDomain(modDto, modInfoSupplierQuery));
            }
            paginationState = Mapper.ToDomain(dto.Pagination);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Curseforge");
        }
        return new()
        {
            Mods = mods,
            PaginationState = paginationState
        };
    }

    public async Task<ModDetails> GetModDetailsAsync(string modId, CancellationToken ct = default)
    {


        string jsonResponse = await client.GetStringAsync($"/v1/mods/{modId}/description", ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement rootElement = doc.RootElement;

        ModDescriptionDto dto = JsonSerializer.Deserialize<ModDescriptionDto>(rootElement) ?? new ModDescriptionDto();

        var htmlDescription = BuildHtml(dto.Data);

        return new()
        {
            ModDescription = htmlDescription,
            ModDescriptionType = ModDescriptionType.Html
        };
    }

    public async Task<ModVersionSupplierResponse> GetModVersionsAsync(ModVersionSupplierQuery modVersionSupplierQuery, CancellationToken ct = default)
    {
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        queryParameters.Add("gameVersion", modVersionSupplierQuery.MinecraftVersion);
        ModloaderType loader = modVersionSupplierQuery.ModloaderType;
        queryParameters.Add("modLoaderType", Mapper.ToDtoModloaderType(loader).ToString());


        queryParameters.Add("index", "0");
        queryParameters.Add("pageSize", "50");

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModVersion> versions = new List<ModVersion>();
        PaginationState paginationState = new();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v1/mods/{modVersionSupplierQuery.ModId}/files?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModFileDto dto = JsonSerializer.Deserialize<SearchModFileDto>(rootElement) ?? new SearchModFileDto();
            foreach (ModFileDto modDto in dto.Data)
            {
                versions.Add(Mapper.ToDomain(modDto, modVersionSupplierQuery));
            }
            paginationState = Mapper.ToDomain(dto.Pagination);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Curseforge");
        }
        return new()
        {
            Versions = versions,
            PaginationState = paginationState
        };
    }

    public Task DownloadMod(ModVersion modVersion, string modsPath, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    private string BuildHtml(string? html)
    {
        var decodedHtml = WebUtility.HtmlDecode(html ?? string.Empty);

        return $"""
    <!DOCTYPE html>
    <html>
    <head>
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
    </head>
    <body>
        {decodedHtml}
    </body>
    </html>
    """;
    }
}
