using System;
using Wave.Domain.System;

namespace Wave.Application.Out.Platform;

public interface IDeviceInformationRepository
{
    public DeviceInformation GetDeviceInformation();
}
