using System;
using System.Xml.Serialization;

namespace Wave.Infrastructure.Out.Modloader.Forge.Api.Dtos;

public class Versioning
{
    [XmlArray("versions")]
    [XmlArrayItem("version")]
    public required List<string> Versions { get; set; }
}
