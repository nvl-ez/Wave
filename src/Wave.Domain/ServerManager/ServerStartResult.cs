namespace Wave.Domain.ServerManager;

public sealed record ServerStartResult(IServerSession? Session, int? RequiredJavaVersion)
{
    public bool Started => Session is not null;

    public static ServerStartResult Success(IServerSession session) => new(session, null);
    public static ServerStartResult JavaNotFound(int requiredJavaVersion) => new(null, requiredJavaVersion);
}
