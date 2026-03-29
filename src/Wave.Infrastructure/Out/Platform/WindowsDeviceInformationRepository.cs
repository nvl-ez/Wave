using System.Runtime.InteropServices;
using Wave.Application.Out.Platform;
using Wave.Domain.System;

namespace Wave.Infrastructure.Out.Platform;

public class WindowsDeviceInformationRepository : IDeviceInformationRepository
{
    private readonly DeviceInformation deviceInformation;
    public WindowsDeviceInformationRepository()
    {
        var architecture = RuntimeInformation.ProcessArchitecture;

        ArchitectureType myArchitecture;
        int myArchitectureBit;
        switch (architecture)
        {
            case Architecture.X64:
                myArchitecture = ArchitectureType.X86;
                myArchitectureBit = 64;
                break;
            case Architecture.X86:
                myArchitecture = ArchitectureType.X86;
                myArchitectureBit = 32;
                break;
            case Architecture.Arm64:
                myArchitecture = ArchitectureType.Arm;
                myArchitectureBit = 64;
                break;
            case Architecture.Arm:
                myArchitecture = ArchitectureType.Arm;
                myArchitectureBit = 32;
                break;
            default:
                myArchitecture = ArchitectureType.Other;
                myArchitectureBit = 0;
                break;
        }

        deviceInformation = new()
        {
            Architecture = myArchitecture,
            ArchitectureBit = myArchitectureBit,
            Os = OsType.Windows
        };
    }
    public DeviceInformation GetDeviceInformation()
    {
        return deviceInformation;
    }
}
