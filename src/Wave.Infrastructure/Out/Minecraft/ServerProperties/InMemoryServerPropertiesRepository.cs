using System;
using Wave.Application.Out.Minecraft;
using Wave.Domain.Minecraft;

namespace Wave.Infrastructure.Out.Minecraft.ServerProperties;

public class InMemoryServerPropertyDefinitionRepository : IServerPropertyDefinitionRepository
{
    public static readonly List<ServerPropertyDefinition> serverProperties = new()
{
    new() { DisplayName = "Accepts Transfers", Key = "accepts-transfers", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Allow Flight", Key = "allow-flight", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Broadcast Console To Ops", Key = "broadcast-console-to-ops", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Broadcast RCON To Ops", Key = "broadcast-rcon-to-ops", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Bug Report Link", Key = "bug-report-link", Type = ServerPropertyType.String },
    new() { DisplayName = "Debug", Key = "debug", Type = ServerPropertyType.Boolean },

    new()
    {
        DisplayName = "Difficulty",
        Key = "difficulty",
        Type = ServerPropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["peaceful"] = "Peaceful",
            ["easy"] = "Easy",
            ["normal"] = "Normal",
            ["hard"] = "Hard"
        }
    },

    new() { DisplayName = "Enable Code Of Conduct", Key = "enable-code-of-conduct", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Enable JMX Monitoring", Key = "enable-jmx-monitoring", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Enable Query", Key = "enable-query", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Enable RCON", Key = "enable-rcon", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Enable Status", Key = "enable-status", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Enforce Secure Profile", Key = "enforce-secure-profile", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Enforce Whitelist", Key = "enforce-whitelist", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Entity Broadcast Range Percentage", Key = "entity-broadcast-range-percentage", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Force Gamemode", Key = "force-gamemode", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Function Permission Level", Key = "function-permission-level", Type = ServerPropertyType.Integer },

    new()
    {
        DisplayName = "Gamemode",
        Key = "gamemode",
        Type = ServerPropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["survival"] = "Survival",
            ["creative"] = "Creative",
            ["adventure"] = "Adventure",
            ["spectator"] = "Spectator"
        }
    },

    new() { DisplayName = "Generate Structures", Key = "generate-structures", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Generator Settings", Key = "generator-settings", Type = ServerPropertyType.String },
    new() { DisplayName = "Hardcore", Key = "hardcore", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Hide Online Players", Key = "hide-online-players", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Initial Disabled Packs", Key = "initial-disabled-packs", Type = ServerPropertyType.String },
    new() { DisplayName = "Initial Enabled Packs", Key = "initial-enabled-packs", Type = ServerPropertyType.String },
    new() { DisplayName = "Level Name", Key = "level-name", Type = ServerPropertyType.String },
    new() { DisplayName = "Level Seed", Key = "level-seed", Type = ServerPropertyType.String },

    new()
    {
        DisplayName = "Level Type",
        Key = "level-type",
        Type = ServerPropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["minecraft:normal"] = "Minecraft: Normal",
            ["minecraft:flat"] = "Minecraft: Flat",
            ["minecraft:large_biomes"] = "Minecraft: Large Biomes",
            ["minecraft:amplified"] = "Minecraft: Amplified",
            ["minecraft:single_biome_surface"] = "Minecraft: Single Biome Surface"
        }
    },

    new() { DisplayName = "Log IPs", Key = "log-ips", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Management Server Allowed Origins", Key = "management-server-allowed-origins", Type = ServerPropertyType.String },
    new() { DisplayName = "Management Server Enabled", Key = "management-server-enabled", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Management Server Host", Key = "management-server-host", Type = ServerPropertyType.String },
    new() { DisplayName = "Management Server Port", Key = "management-server-port", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Management Server Secret", Key = "management-server-secret", Type = ServerPropertyType.String },
    new() { DisplayName = "Management Server TLS Enabled", Key = "management-server-tls-enabled", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Management Server TLS Keystore", Key = "management-server-tls-keystore", Type = ServerPropertyType.String },
    new() { DisplayName = "Management Server TLS Keystore Password", Key = "management-server-tls-keystore-password", Type = ServerPropertyType.String },
    new() { DisplayName = "Max Chained Neighbor Updates", Key = "max-chained-neighbor-updates", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Max Players", Key = "max-players", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Max Tick Time", Key = "max-tick-time", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Max World Size", Key = "max-world-size", Type = ServerPropertyType.Integer },
    new() { DisplayName = "MOTD", Key = "motd", Type = ServerPropertyType.String },
    new() { DisplayName = "Network Compression Threshold", Key = "network-compression-threshold", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Online Mode", Key = "online-mode", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Op Permission Level", Key = "op-permission-level", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Pause When Empty Seconds", Key = "pause-when-empty-seconds", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Player Idle Timeout", Key = "player-idle-timeout", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Prevent Proxy Connections", Key = "prevent-proxy-connections", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Query Port", Key = "query.port", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Rate Limit", Key = "rate-limit", Type = ServerPropertyType.Integer },
    new() { DisplayName = "RCON Password", Key = "rcon.password", Type = ServerPropertyType.String },
    new() { DisplayName = "RCON Port", Key = "rcon.port", Type = ServerPropertyType.Integer },

    new()
    {
        DisplayName = "Region File Compression",
        Key = "region-file-compression",
        Type = ServerPropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["deflate"] = "Deflate",
            ["lz4"] = "Lz4",
            ["none"] = "None"
        }
    },

    new() { DisplayName = "Require Resource Pack", Key = "require-resource-pack", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Resource Pack", Key = "resource-pack", Type = ServerPropertyType.String },
    new() { DisplayName = "Resource Pack Id", Key = "resource-pack-id", Type = ServerPropertyType.String },
    new() { DisplayName = "Resource Pack Prompt", Key = "resource-pack-prompt", Type = ServerPropertyType.String },
    new() { DisplayName = "Resource Pack Sha1", Key = "resource-pack-sha1", Type = ServerPropertyType.String },
    new() { DisplayName = "Server IP", Key = "server-ip", Type = ServerPropertyType.String },
    new() { DisplayName = "Server Port", Key = "server-port", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Simulation Distance", Key = "simulation-distance", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Spawn Protection", Key = "spawn-protection", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Status Heartbeat Interval", Key = "status-heartbeat-interval", Type = ServerPropertyType.Integer },
    new() { DisplayName = "Sync Chunk Writes", Key = "sync-chunk-writes", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "Text Filtering Config", Key = "text-filtering-config", Type = ServerPropertyType.String },

    new()
    {
        DisplayName = "Text Filtering Version",
        Key = "text-filtering-version",
        Type = ServerPropertyType.Options,
        Options = new Dictionary<string, string>
        {
            ["0"] = "0",
            ["1"] = "1"
        }
    },

    new() { DisplayName = "Use Native Transport", Key = "use-native-transport", Type = ServerPropertyType.Boolean },
    new() { DisplayName = "View Distance", Key = "view-distance", Type = ServerPropertyType.Integer },
    new() { DisplayName = "White List", Key = "white-list", Type = ServerPropertyType.Boolean }
};
    public IEnumerable<ServerPropertyDefinition> GetAllServerProperties()
    {
        return serverProperties;
    }

    public Task<IEnumerable<ServerPropertyDefinition>> GetAllServerPropertiesAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ServerPropertyDefinition> GetServerProperty(string key)
    {
        return serverProperties.Where(sp => sp.Key == key);
    }

    public Task<IEnumerable<ServerPropertyDefinition>> GetServerPropertyAsync(string key, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
