using System;
using System.Text.Json;
using Wave.Application.Out.ModSupplier;
using Wave.Domain.Modloaders;
using Wave.Domain.Mods;
using Wave.Domain.ModSupplier;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Dtos;
using Wave.Infrastructure.Out.ModSupplier.Curseforge.Api.Mappers;

namespace Wave.Infrastructure.Out.ModSupplier.Curseforge.Api;

public class CurseforgeModSupplier : IModSupplierIntegration
{
    private readonly HttpClient client;

    public CurseforgeModSupplier()
    {
        client = new HttpClient()
        {
            BaseAddress = new Uri("https://api.curseforge.com")
        };
        client.DefaultRequestHeaders.Add("x-api-key", "$2a$10$BGG5jB6kIf.QgqGtFOKEWuscWzRGs.YsZ3YXp1YJ7.0PW9i4CzmAe");
    }
    public async Task<Mod> SearchModAsync(Mod mod, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Mod>> SearchModsAsync(ModSupplierQuery modSupplierQuery, CancellationToken ct)
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
        queryParameters.Add("gameVersion", modSupplierQuery.MinecraftVersion.Version);

        ModloaderType loader = modSupplierQuery.ModloaderType;
        if (loader == ModloaderType.Vanilla) throw new NotSupportedException("Mods cannot be searched for Vanilla Minecraft.");
        queryParameters.Add("modLoaderType",
            loader == ModloaderType.Forge ? "Forge" :
                (loader == ModloaderType.Fabric ? "Fabric" : throw new NotImplementedException("Missing implementation for modloader"))
            );

        queryParameters.Add("index", modSupplierQuery.Offset.ToString());
        queryParameters.Add("pageSize", modSupplierQuery.PageSize.ToString());
        queryParameters.Add("gameId", $"{432}");

        string queryString = string.Join("&", queryParameters.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

        //HTTP Request
        List<Mod> mods = new List<Mod>();
        try
        {
            string jsonResponse = await client.GetStringAsync($"/v1/mods/search?{queryString}", ct);
            JsonDocument doc = JsonDocument.Parse(jsonResponse);
            JsonElement rootElement = doc.RootElement;

            SearchModsResponseDto dto = JsonSerializer.Deserialize<SearchModsResponseDto>(rootElement) ?? new SearchModsResponseDto();
            foreach (ModDto modDto in dto.Mods)
            {
                mods.Add(ModDtoMapper.ToDomain(modDto, modSupplierQuery));
            }
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("Error when contacting ");
        }
        return mods;
    }
}
