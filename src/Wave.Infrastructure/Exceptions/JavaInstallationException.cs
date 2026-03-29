using System;

namespace Wave.Infrastructure.Exceptions;

public class JavaInstallationException : Exception
{
    public JavaInstallationException(string message) : base(message) { }
}
