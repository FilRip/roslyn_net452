using System;
using System.IO;
using System.Xml;

namespace Microsoft.CodeAnalysis
{
    internal struct AssemblyPortabilityPolicy : IEquatable<AssemblyPortabilityPolicy>
    {
        public readonly bool SuppressSilverlightPlatformAssembliesPortability;

        public readonly bool SuppressSilverlightLibraryAssembliesPortability;

        private static readonly XmlReaderSettings s_xmlSettings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit
        };

        public AssemblyPortabilityPolicy(bool suppressSilverlightPlatformAssembliesPortability, bool suppressSilverlightLibraryAssembliesPortability)
        {
            SuppressSilverlightLibraryAssembliesPortability = suppressSilverlightLibraryAssembliesPortability;
            SuppressSilverlightPlatformAssembliesPortability = suppressSilverlightPlatformAssembliesPortability;
        }

        public override bool Equals(object obj)
        {
            if (obj is AssemblyPortabilityPolicy)
            {
                return Equals((AssemblyPortabilityPolicy)obj);
            }
            return false;
        }

        public bool Equals(AssemblyPortabilityPolicy other)
        {
            if (SuppressSilverlightLibraryAssembliesPortability == other.SuppressSilverlightLibraryAssembliesPortability)
            {
                return SuppressSilverlightPlatformAssembliesPortability == other.SuppressSilverlightPlatformAssembliesPortability;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (SuppressSilverlightLibraryAssembliesPortability ? 1 : 0) | (SuppressSilverlightPlatformAssembliesPortability ? 2 : 0);
        }

        private static bool ReadToChild(XmlReader reader, int depth, string elementName, string elementNamespace = "")
        {
            if (reader.ReadToDescendant(elementName, elementNamespace))
            {
                return reader.Depth == depth;
            }
            return false;
        }

        internal static AssemblyPortabilityPolicy LoadFromXml(Stream input)
        {
            using XmlReader xmlReader = XmlReader.Create(input, s_xmlSettings);
            if (!ReadToChild(xmlReader, 0, "configuration") || !ReadToChild(xmlReader, 1, "runtime") || !ReadToChild(xmlReader, 2, "assemblyBinding", "urn:schemas-microsoft-com:asm.v1") || !ReadToChild(xmlReader, 3, "supportPortability", "urn:schemas-microsoft-com:asm.v1"))
            {
                return default(AssemblyPortabilityPolicy);
            }
            bool suppressSilverlightLibraryAssembliesPortability = false;
            bool suppressSilverlightPlatformAssembliesPortability = false;
            do
            {
                string attribute = xmlReader.GetAttribute("PKT");
                string attribute2 = xmlReader.GetAttribute("enable");
                bool? flag = (string.Equals(attribute2, "false", StringComparison.OrdinalIgnoreCase) ? new bool?(false) : (string.Equals(attribute2, "true", StringComparison.OrdinalIgnoreCase) ? new bool?(true) : null));
                if (flag.HasValue)
                {
                    if (string.Equals(attribute, "31bf3856ad364e35", StringComparison.OrdinalIgnoreCase))
                    {
                        suppressSilverlightLibraryAssembliesPortability = !flag.Value;
                    }
                    else if (string.Equals(attribute, "7cec85d7bea7798e", StringComparison.OrdinalIgnoreCase))
                    {
                        suppressSilverlightPlatformAssembliesPortability = !flag.Value;
                    }
                }
            }
            while (xmlReader.ReadToNextSibling("supportPortability", "urn:schemas-microsoft-com:asm.v1"));
            return new AssemblyPortabilityPolicy(suppressSilverlightPlatformAssembliesPortability, suppressSilverlightLibraryAssembliesPortability);
        }
    }
}
