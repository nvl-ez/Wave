using System;
using System.Diagnostics;
using Wave.Application.Out.ServerManager;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager.Executor;

public class WindowsServerExecutor : IServerExecutor
{
    public IServerSession Start(Server server, IJavaInstallation javaInstallation, CancellationToken ct = default)
    {
        if (server.Info.ServerDirectory is null) throw new NullReferenceException("Server Directory cannot be null.");
        if (server.Details.ServerFilename is null) throw new NullReferenceException("Server Filename cannot be null.");

        string serverJar = Path.Combine(server.Info.ServerDirectory, server.Details.ServerFilename);

        if (!File.Exists(serverJar)) throw new IOException($"File '{serverJar}' does not exist.");

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = javaInstallation.ExecutableFile,
                WorkingDirectory = server.Info.ServerDirectory,
                Arguments = $"-jar \"{serverJar}\"",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();

        return new ServerSession(process, server.Id);
    }
}
