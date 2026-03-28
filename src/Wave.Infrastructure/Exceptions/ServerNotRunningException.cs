using System;

namespace Wave.Infrastructure.Exceptions;

public class ServerNotRunningException : Exception
{
    public ServerNotRunningException(string message) : base(message) { }
}
