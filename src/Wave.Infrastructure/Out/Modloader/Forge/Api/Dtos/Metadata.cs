using System;
using System.Xml.Serialization;

namespace Wave.Infrastructure.Out.Modloader.Forge.Api.Dtos;

[XmlRoot("metadata")]
public class Metadata
{
    [XmlElement("versioning")]
    public required Versioning Versioning { get; set; }
}
