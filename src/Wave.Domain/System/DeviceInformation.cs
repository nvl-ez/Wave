using System;

namespace Wave.Domain.System;

public class DeviceInformation
{
    public required OsType Os { get; set; }
    public required ArchitectureType Architecture { get; set; }
    public required int ArchitectureBit { get; set; }
}
