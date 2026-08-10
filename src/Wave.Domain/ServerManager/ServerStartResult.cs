namespace Wave.Domain.ServerManager;

public enum ServerStartFailure
{
    None,
    JavaNotFound,
    PortInUse,
    PortMappingFailed
}

public sealed record ServerStartResult(IServerSession? Session, ServerStartFailure Failure, int? RequiredJavaVersion, int? Port)
{
    public bool Started => Session is not null;

    public static ServerStartResult Success(IServerSession session, int port) => new(session, ServerStartFailure.None, null, port);
    public static ServerStartResult JavaNotFound(int requiredJavaVersion) => new(null, ServerStartFailure.JavaNotFound, requiredJavaVersion, null);
    public static ServerStartResult PortInUse(int port) => new(null, ServerStartFailure.PortInUse, null, port);
    public static ServerStartResult PortMappingFailed(int port) => new(null, ServerStartFailure.PortMappingFailed, null, port);
}
