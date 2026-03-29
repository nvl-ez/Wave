using System;
using Wave.Domain.System;

namespace Wave.Application.In;

public interface IDeviceInformationService
{
    public DeviceInformation GetDeviceInformation();
}
