using System;
using System.Net;
using System.Text.Json;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Mods;
using Wave.Domain.Utils;
using Wave.Infrastructure.Out.ModSupplier.Modrinth.Api.Dtos;
using Markdig;

namespace Wave.Infrastructure.Out.ModSupplier.Modrinth.Api;

public class ApiModrinthModSupplier : IModSupplierIntegration
{
    private readonly HttpClient client;

    public ModSupplierType ModSupplierType { get => ModSupplierType.Modrinth; }
    public bool RequiresToken => false;
    public bool HasToken => true;

    public void SetToken(string? token) { }

    public ApiModrinthModSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.modrinth.com")
        };
        client.DefaultRequestHeaders.Add("User-Agent", "nvl-ez/Wave (nahuelvazquezlevrino@gmail.com)");
    }

    public bool CanHandle(ModSupplierType modSupplierType)
    {
        return modSupplierType == ModSupplierType;
    }

    public async Task<ModInfoSupplierResponse> SearchModsAsync(ModInfoSupplierQuery modSupplierQuery, CancellationToken ct = default)
    {
        //Build Query Arguments
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(modSupplierQuery.TextQuery))
        {
            queryParameters.Add("query", modSupplierQuery.TextQuery.Trim());
        }

        if (!string.IsNullOrWhiteSpace(modSupplierQuery.Author))
        {
            string author = modSupplierQuery.Author.Trim();

            if (queryParameters.TryGetValue("query", out string? textQuery))
            {
                queryParameters["query"] = $"{textQuery} {author}";
            }
            else
            {
                queryParameters.Add("query", author);
            }
        }
        queryParameters.Add("offset", modSupplierQuery.PaginationState.Index.ToString());
        queryParameters.Add("limit", modSupplierQuery.PaginationState.PageSize.ToString());

        string loaderString = Mapper.ToDtoModloaderType(modSupplierQuery.ModloaderType);
        string facets = $"[[\"categories:{loaderString}\"],[\"versions:{modSupplierQuery.MinecraftVersion}\"],[\"project_type:mod\"]]";
        queryParameters.Add("facets", facets);

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModInfo> mods = new List<ModInfo>();
        PaginationState paginationState = new PaginationState();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v2/search?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModsResponseDto dto = JsonSerializer.Deserialize<SearchModsResponseDto>(rootElement) ?? new SearchModsResponseDto();
            foreach (ProjectDto modDto in dto.Projects)
            {
                mods.Add(Mapper.ToDomain(modDto));
            }
            paginationState = Mapper.ToDomain(dto);
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Modrinth");
        }
        return new()
        {
            Mods = mods,
            PaginationState = paginationState
        };
    }

    public async Task<ModDetails> GetModDetailsAsync(ModBase modBase, CancellationToken ct = default)
    {

        string jsonResponse = await client.GetStringAsync($"/v2/project/{modBase.ModId}", ct);
        JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement rootElement = doc.RootElement;

        ProjectDetailsDto dto = JsonSerializer.Deserialize<ProjectDetailsDto>(rootElement) ?? new ProjectDetailsDto();

        return new(modBase, BuildHtml(dto.Body), ModDescriptionType.Html);
    }

    public async Task<ModInfo> GetModInfoAsync(string modId, CancellationToken ct = default)
    {
        string jsonResponse = await client.GetStringAsync($"/v2/project/{modId}", ct);
        ProjectDetailsDto dto = JsonSerializer.Deserialize<ProjectDetailsDto>(jsonResponse) ?? new ProjectDetailsDto();
        return new(dto.ProjectId, dto.Title, dto.Slug, ModSupplierType, dto.Description, dto.IconUrl);
    }

    public async Task<ModVersionSupplierResponse> GetModVersionsAsync(ModVersionSupplierQuery modInfo, CancellationToken ct = default)
    {
        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
        string loaderString = Mapper.ToDtoModloaderType(modInfo.ModloaderType);
        queryParameters.Add("loaders", $"[\"{loaderString}\"]");
        queryParameters.Add("game_versions", $"[\"{modInfo.MinecraftVersion}\"]");

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        List<ModVersion> versions = new List<ModVersion>();
        PaginationState paginationState = new PaginationState();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v2/project/{modInfo.ModId}/version?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            List<ProjectVersionDto> dto = JsonSerializer.Deserialize<List<ProjectVersionDto>>(rootElement) ?? new List<ProjectVersionDto>();
            foreach (ProjectVersionDto versionDto in dto)
            {
                versions.Add(Mapper.ToDomain(versionDto, modInfo));
            }
            paginationState = new()
            {
                Index = 0,
                PageSize = versions.Count,
                ResultCount = versions.Count,
                TotalCount = versions.Count
            };
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting Modrinth");
        }
        return new()
        {
            Versions = versions,
            PaginationState = paginationState
        };
    }

    public async Task DownloadMod(ModFile modFile, string modsPath, CancellationToken ct = default)
    {
        foreach (var modArtifact in modFile.Artifacts)
        {
            using (var response = await client.GetAsync(modArtifact.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, ct))
            {
                response.EnsureSuccessStatusCode();

                string filePath = Path.Combine(modsPath, modArtifact.FileName);
                using (var fileStream = File.Create(filePath))
                {
                    using (var httpStream = await response.Content.ReadAsStreamAsync(ct))
                    {
                        await httpStream.CopyToAsync(fileStream, ct);
                    }
                }
            }
        }
    }

    private string BuildHtml(string body)
    {

        var html = Markdown.ToHtml(body);
        var decodedHtml = WebUtility.HtmlDecode(html);

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
