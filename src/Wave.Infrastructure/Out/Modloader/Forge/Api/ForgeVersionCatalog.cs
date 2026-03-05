using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using Wave.Application.Out.Modloader.Api;
using Wave.Domain.Minecraft;
using Wave.Domain.Modloaders;
using Wave.Infrastructure.Out.Modloader.Forge.Api.Dtos;
using Wave.Infrastructure.Out.Modloader.Forge.Api.Mappers;

namespace Wave.Infrastructure.Out.Modloader.Forge.Api;

public class ForgeVersionCatalog : IModloaderVersionCatalog
{
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("https://maven.minecraftforge.net/net/minecraftforge/forge/maven-metadata.xml")
    };

    public async Task<IEnumerable<ModloaderVersion>> GetModloaderVersionsAsync(MinecraftVersion minecraftVersion, CancellationToken ct)
    {
        List<ForgeVersion> forgeVersions = new List<ForgeVersion>();
        try
        {
            string xmlResponse = await client.GetStringAsync("", ct);

            XDocument doc = XDocument.Parse(xmlResponse);

            XmlSerializer serializer = new XmlSerializer(typeof(Metadata));
            Metadata metadata = (Metadata)serializer.Deserialize(doc.CreateReader())!;

            List<string> versionBundles = metadata.Versioning?.Versions ?? new List<string>();



            foreach (string versionBundle in versionBundles)
            {
                forgeVersions.Add(Mapper.ToDomain(versionBundle));
            }

            forgeVersions = forgeVersions.Where(f => minecraftVersion.Version == f.Version).ToList();
        }
        catch (HttpRequestException)
        {
            Console.WriteLine("No Forge versions were found.");
        }

        return forgeVersions;
    }
}
