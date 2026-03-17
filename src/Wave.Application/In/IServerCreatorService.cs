using System;
using Wave.Domain.ServerManager;

namespace Wave.Application.In;

public interface IServerCreatorService
{
    public void CreateServer(ServerRequest serverDefinition);
    // Crear Archivo > Descargar Server Jar > Descargar Modloader > TODO

    //TODO Curseforge Integration

}
