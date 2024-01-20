using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightingale.Editor
{
    public class XmlClassificationTypes
    {
        public static IClassificationType XmlEntity { get; } = new ClassificationType("XmlEntity", "Xml Entity");
        public static IClassificationType XmlDelimiter { get; } = new ClassificationType("XmlDelimiter", "Xml Delimiter");
        public static IClassificationType XmlName { get; } = new ClassificationType("XmlName", "Xml EnNametity");
        public static IClassificationType XmlAttribute { get; } = new ClassificationType("XmlAttribute", "Xml Attribute");
        public static IClassificationType XmlAttributeValue { get; } = new ClassificationType("XmlAttributeValue", "Xml Attribute Value");
        public static IClassificationType XmlComment { get; } = new ClassificationType("XmlComment", "Xml Comment");
        public static IClassificationType XmlProcessingInstruction { get; } = new ClassificationType("XmlProcessingInstruction", "Xml Processing Instruction");
        public static IClassificationType XmlDeclaration { get; } = new ClassificationType("XmlDeclaration", "Xml Declaration");
        public static IClassificationType ServerSideScript { get; } = new ClassificationType("ServerSideScript", "Server-Side Script");
    }
}
