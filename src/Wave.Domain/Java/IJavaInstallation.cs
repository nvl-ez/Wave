using System;

namespace Wave.Domain.Java;

public interface IJavaInstallation
{
    public string ExecutableFile { get; set; }
    public string UninstallerPath { get; set; }
    public int Version { get; set; }
    public string Name { get; set; }
    public JavaSupplierType JavaSupplierType { get; set; }

}
