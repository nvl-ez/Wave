using System;

namespace Wave.Infrastructure.Exceptions;

public class ServerAlreadyRunningException : Exception
{
    public ServerAlreadyRunningException(string message) : base(message)
    {

    }
}
