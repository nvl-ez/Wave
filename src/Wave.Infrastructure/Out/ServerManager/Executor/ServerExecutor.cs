using System;
using System.Diagnostics;
using Wave.Application.Out.ServerManager;
using Wave.Domain.Java;
using Wave.Domain.ServerManager;

namespace Wave.Infrastructure.Out.ServerManager.Executor;

public class WindowsServerExecutor : IServerExecutor
{
    public IServerSession Start(Guid serverId, string serverDirectory, string jarPath, JavaInstallation javaInstallation, CancellationToken ct = default)
    {
        if (!File.Exists(jarPath)) throw new IOException($"File '{jarPath}' does not exist.");

        Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = javaInstallation.ExecutableFile,
                WorkingDirectory = serverDirectory,

                Arguments = $"-jar \"{jarPath}\" nogui",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };
        process.Start();

        return new ServerSession(process, serverId);
    }
}
