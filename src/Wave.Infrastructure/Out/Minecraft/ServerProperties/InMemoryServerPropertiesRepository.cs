using System;
using Wave.Application.Out.Minecraft;
using Wave.Domain.ServerManager.Properties;

namespace Wave.Infrastructure.Out.Minecraft.ServerProperties;

public class InMemoryServerPropertyDefinitionRepository : IServerPropertyDefinitionRepository
{
    public static readonly List<PropertyDefinition> serverProperties = new()
{
    new() { DisplayName = "Accepts Transfers", Key = "accepts-transfers", Type = PropertyType.Boolean },
    new() { DisplayName = "Allow Flight", Key = "allow-flight", Type = PropertyType.Boolean },
    new() { DisplayName = "Broadcast Console To Ops", Key = "broadcast-console-to-ops", Type = PropertyType.Boolean },
    new() { DisplayName = "Broadcast RCON To Ops", Key = "broadcast-rcon-to-ops", Type = PropertyType.Boolean },
    new() { DisplayName = "Bug Report Link", Key = "bug-report-link", Type = PropertyType.String },
    new() { DisplayName = "Debug", Key = "debug", Type = PropertyType.Boolean },

    new()
    {
        DisplayName = "Difficulty",
        Key = "difficulty",
        Type = PropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["peaceful"] = "Peaceful",
            ["easy"] = "Easy",
            ["normal"] = "Normal",
            ["hard"] = "Hard"
        }
    },

    new() { DisplayName = "Enable Code Of Conduct", Key = "enable-code-of-conduct", Type = PropertyType.Boolean },
    new() { DisplayName = "Enable JMX Monitoring", Key = "enable-jmx-monitoring", Type = PropertyType.Boolean },
    new() { DisplayName = "Enable Query", Key = "enable-query", Type = PropertyType.Boolean },
    new() { DisplayName = "Enable RCON", Key = "enable-rcon", Type = PropertyType.Boolean },
    new() { DisplayName = "Enable Status", Key = "enable-status", Type = PropertyType.Boolean },
    new() { DisplayName = "Enforce Secure Profile", Key = "enforce-secure-profile", Type = PropertyType.Boolean },
    new() { DisplayName = "Enforce Whitelist", Key = "enforce-whitelist", Type = PropertyType.Boolean },
    new() { DisplayName = "Entity Broadcast Range Percentage", Key = "entity-broadcast-range-percentage", Type = PropertyType.Integer },
    new() { DisplayName = "Force Gamemode", Key = "force-gamemode", Type = PropertyType.Boolean },
    new() { DisplayName = "Function Permission Level", Key = "function-permission-level", Type = PropertyType.Integer },

    new()
    {
        DisplayName = "Gamemode",
        Key = "gamemode",
        Type = PropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["survival"] = "Survival",
            ["creative"] = "Creative",
            ["adventure"] = "Adventure",
            ["spectator"] = "Spectator"
        }
    },

    new() { DisplayName = "Generate Structures", Key = "generate-structures", Type = PropertyType.Boolean },
    new() { DisplayName = "Generator Settings", Key = "generator-settings", Type = PropertyType.String },
    new() { DisplayName = "Hardcore", Key = "hardcore", Type = PropertyType.Boolean },
    new() { DisplayName = "Hide Online Players", Key = "hide-online-players", Type = PropertyType.Boolean },
    new() { DisplayName = "Initial Disabled Packs", Key = "initial-disabled-packs", Type = PropertyType.String },
    new() { DisplayName = "Initial Enabled Packs", Key = "initial-enabled-packs", Type = PropertyType.String },
    new() { DisplayName = "Level Name", Key = "level-name", Type = PropertyType.String },
    new() { DisplayName = "Level Seed", Key = "level-seed", Type = PropertyType.String },

    new()
    {
        DisplayName = "Level Type",
        Key = "level-type",
        Type = PropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["minecraft:normal"] = "Minecraft: Normal",
            ["minecraft:flat"] = "Minecraft: Flat",
            ["minecraft:large_biomes"] = "Minecraft: Large Biomes",
            ["minecraft:amplified"] = "Minecraft: Amplified",
            ["minecraft:single_biome_surface"] = "Minecraft: Single Biome Surface"
        }
    },

    new() { DisplayName = "Log IPs", Key = "log-ips", Type = PropertyType.Boolean },
    new() { DisplayName = "Management Server Allowed Origins", Key = "management-server-allowed-origins", Type = PropertyType.String },
    new() { DisplayName = "Management Server Enabled", Key = "management-server-enabled", Type = PropertyType.Boolean },
    new() { DisplayName = "Management Server Host", Key = "management-server-host", Type = PropertyType.String },
    new() { DisplayName = "Management Server Port", Key = "management-server-port", Type = PropertyType.Integer },
    new() { DisplayName = "Management Server Secret", Key = "management-server-secret", Type = PropertyType.String },
    new() { DisplayName = "Management Server TLS Enabled", Key = "management-server-tls-enabled", Type = PropertyType.Boolean },
    new() { DisplayName = "Management Server TLS Keystore", Key = "management-server-tls-keystore", Type = PropertyType.String },
    new() { DisplayName = "Management Server TLS Keystore Password", Key = "management-server-tls-keystore-password", Type = PropertyType.String },
    new() { DisplayName = "Max Chained Neighbor Updates", Key = "max-chained-neighbor-updates", Type = PropertyType.Integer },
    new() { DisplayName = "Max Players", Key = "max-players", Type = PropertyType.Integer },
    new() { DisplayName = "Max Tick Time", Key = "max-tick-time", Type = PropertyType.Integer },
    new() { DisplayName = "Max World Size", Key = "max-world-size", Type = PropertyType.Integer },
    new() { DisplayName = "MOTD", Key = "motd", Type = PropertyType.String },
    new() { DisplayName = "Network Compression Threshold", Key = "network-compression-threshold", Type = PropertyType.Integer },
    new() { DisplayName = "Online Mode", Key = "online-mode", Type = PropertyType.Boolean },
    new() { DisplayName = "Op Permission Level", Key = "op-permission-level", Type = PropertyType.Integer },
    new() { DisplayName = "Pause When Empty Seconds", Key = "pause-when-empty-seconds", Type = PropertyType.Integer },
    new() { DisplayName = "Player Idle Timeout", Key = "player-idle-timeout", Type = PropertyType.Integer },
    new() { DisplayName = "Prevent Proxy Connections", Key = "prevent-proxy-connections", Type = PropertyType.Boolean },
    new() { DisplayName = "Query Port", Key = "query.port", Type = PropertyType.Integer },
    new() { DisplayName = "Rate Limit", Key = "rate-limit", Type = PropertyType.Integer },
    new() { DisplayName = "RCON Password", Key = "rcon.password", Type = PropertyType.String },
    new() { DisplayName = "RCON Port", Key = "rcon.port", Type = PropertyType.Integer },

    new()
    {
        DisplayName = "Region File Compression",
        Key = "region-file-compression",
        Type = PropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["deflate"] = "Deflate",
            ["lz4"] = "Lz4",
            ["none"] = "None"
        }
    },

    new() { DisplayName = "Require Resource Pack", Key = "require-resource-pack", Type = PropertyType.Boolean },
    new() { DisplayName = "Resource Pack", Key = "resource-pack", Type = PropertyType.String },
    new() { DisplayName = "Resource Pack Id", Key = "resource-pack-id", Type = PropertyType.String },
    new() { DisplayName = "Resource Pack Prompt", Key = "resource-pack-prompt", Type = PropertyType.String },
    new() { DisplayName = "Resource Pack Sha1", Key = "resource-pack-sha1", Type = PropertyType.String },
    new() { DisplayName = "Server IP", Key = "server-ip", Type = PropertyType.String },
    new() { DisplayName = "Server Port", Key = "server-port", Type = PropertyType.Integer },
    new() { DisplayName = "Simulation Distance", Key = "simulation-distance", Type = PropertyType.Integer },
    new() { DisplayName = "Spawn Protection", Key = "spawn-protection", Type = PropertyType.Integer },
    new() { DisplayName = "Status Heartbeat Interval", Key = "status-heartbeat-interval", Type = PropertyType.Integer },
    new() { DisplayName = "Sync Chunk Writes", Key = "sync-chunk-writes", Type = PropertyType.Boolean },
    new() { DisplayName = "Text Filtering Config", Key = "text-filtering-config", Type = PropertyType.String },

    new()
    {
        DisplayName = "Text Filtering Version",
        Key = "text-filtering-version",
        Type = PropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["0"] = "0",
            ["1"] = "1"
        }
    },

    new() { DisplayName = "Use Native Transport", Key = "use-native-transport", Type = PropertyType.Boolean },
    new() { DisplayName = "View Distance", Key = "view-distance", Type = PropertyType.Integer },
    new() { DisplayName = "White List", Key = "white-list", Type = PropertyType.Boolean }
};
    public IEnumerable<PropertyDefinition> GetAllServerProperties()
    {
        return serverProperties;
    }

    public async Task<IEnumerable<PropertyDefinition>> GetAllServerPropertiesAsync(CancellationToken ct = default)
    {
        return GetAllServerProperties();
    }

    public IEnumerable<PropertyDefinition> GetServerProperty(string key)
    {
        return serverProperties.Where(sp => sp.Key == key);
    }

    public Task<IEnumerable<PropertyDefinition>> GetServerPropertyAsync(string key, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
