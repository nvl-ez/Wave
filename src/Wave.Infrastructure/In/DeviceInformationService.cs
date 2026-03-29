using System;
using Wave.Application.In;
using Wave.Application.Out.Platform;
using Wave.Domain.System;
using Wave.Infrastructure.Out.Platform;

namespace Wave.Infrastructure.In;

public class DeviceInformationService : IDeviceInformationService
{
    private IDeviceInformationRepository deviceInformationRepository;
    public DeviceInformationService(IDeviceInformationRepository deviceInformationRepository)
    {
        this.deviceInformationRepository = deviceInformationRepository;
    }
    public DeviceInformation GetDeviceInformation()
    {
        return deviceInformationRepository.GetDeviceInformation();
    }
}
