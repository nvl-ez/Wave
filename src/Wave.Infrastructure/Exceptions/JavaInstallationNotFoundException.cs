using System;

namespace Wave.Infrastructure.Exceptions;

public class JavaInstallationNotFoundException : Exception
{
    public JavaInstallationNotFoundException(string message) : base(message)
    {

    }

}
