using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal sealed class SarifV2ErrorLogger : SarifErrorLogger, IDisposable
    {
        private sealed class DiagnosticDescriptorSet
        {
            private readonly Dictionary<DiagnosticDescriptor, int> _distinctDescriptors = new Dictionary<DiagnosticDescriptor, int>(SarifDiagnosticComparer.Instance);

            public int Count => _distinctDescriptors.Count;

            public int Add(DiagnosticDescriptor descriptor)
            {
                if (_distinctDescriptors.TryGetValue(descriptor, out var value))
                {
                    return value;
                }
                _distinctDescriptors.Add(descriptor, Count);
                return Count - 1;
            }

            public List<KeyValuePair<int, DiagnosticDescriptor>> ToSortedList()
            {
                List<KeyValuePair<int, DiagnosticDescriptor>> list = new List<KeyValuePair<int, DiagnosticDescriptor>>(Count);
                foreach (KeyValuePair<DiagnosticDescriptor, int> distinctDescriptor in _distinctDescriptors)
                {
                    list.Add(new KeyValuePair<int, DiagnosticDescriptor>(distinctDescriptor.Value, distinctDescriptor.Key));
                }
                list.Sort((KeyValuePair<int, DiagnosticDescriptor> x, KeyValuePair<int, DiagnosticDescriptor> y) => x.Key.CompareTo(y.Key));
                return list;
            }
        }

        private readonly DiagnosticDescriptorSet _descriptors;

        private readonly string _toolName;

        private readonly string _toolFileVersion;

        private readonly Version _toolAssemblyVersion;

        protected override string PrimaryLocationPropertyName => "physicalLocation";

        public SarifV2ErrorLogger(Stream stream, string toolName, string toolFileVersion, Version toolAssemblyVersion, CultureInfo culture)
            : base(stream, culture)
        {
            _descriptors = new DiagnosticDescriptorSet();
            _toolName = toolName;
            _toolFileVersion = toolFileVersion;
            _toolAssemblyVersion = toolAssemblyVersion;
            base.m_writer.WriteObjectStart();
            base.m_writer.Write("$schema", "http://json.schemastore.org/sarif-2.1.0");
            base.m_writer.Write("version", "2.1.0");
            base.m_writer.WriteArrayStart("runs");
            base.m_writer.WriteObjectStart();
            base.m_writer.WriteArrayStart("results");
        }

        public override void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo)
        {
            base.m_writer.WriteObjectStart();
            base.m_writer.Write("ruleId", diagnostic.Id);
            int value = _descriptors.Add(diagnostic.Descriptor);
            base.m_writer.Write("ruleIndex", value);
            base.m_writer.Write("level", SarifErrorLogger.GetLevel(diagnostic.Severity));
            string message = diagnostic.GetMessage(base.m_culture);
            if (!RoslynString.IsNullOrEmpty(message))
            {
                base.m_writer.WriteObjectStart("message");
                base.m_writer.Write("text", message);
                base.m_writer.WriteObjectEnd();
            }
            if (diagnostic.IsSuppressed)
            {
                base.m_writer.WriteArrayStart("suppressions");
                base.m_writer.WriteObjectStart();
                base.m_writer.Write("kind", "inSource");
                string text = suppressionInfo?.Attribute?.DecodeNamedArgument<string>("Justification", SpecialType.System_String);
                if (text != null)
                {
                    base.m_writer.Write("justification", text);
                }
                base.m_writer.WriteObjectEnd();
                base.m_writer.WriteArrayEnd();
            }
            WriteLocations(diagnostic.Location, diagnostic.AdditionalLocations);
            WriteResultProperties(diagnostic);
            base.m_writer.WriteObjectEnd();
        }

        private void WriteLocations(Location location, IReadOnlyList<Location> additionalLocations)
        {
            if (SarifErrorLogger.HasPath(location))
            {
                base.m_writer.WriteArrayStart("locations");
                base.m_writer.WriteObjectStart();
                base.m_writer.WriteKey(PrimaryLocationPropertyName);
                WritePhysicalLocation(location);
                base.m_writer.WriteObjectEnd();
                base.m_writer.WriteArrayEnd();
            }
            if (additionalLocations == null || additionalLocations.Count <= 0 || !additionalLocations.Any((Location l) => SarifErrorLogger.HasPath(l)))
            {
                return;
            }
            base.m_writer.WriteArrayStart("relatedLocations");
            foreach (Location additionalLocation in additionalLocations)
            {
                if (SarifErrorLogger.HasPath(additionalLocation))
                {
                    base.m_writer.WriteObjectStart();
                    base.m_writer.WriteKey("physicalLocation");
                    WritePhysicalLocation(additionalLocation);
                    base.m_writer.WriteObjectEnd();
                }
            }
            base.m_writer.WriteArrayEnd();
        }

        protected override void WritePhysicalLocation(Location diagnosticLocation)
        {
            FileLinePositionSpan lineSpan = diagnosticLocation.GetLineSpan();
            base.m_writer.WriteObjectStart();
            base.m_writer.WriteObjectStart("artifactLocation");
            base.m_writer.Write("uri", SarifErrorLogger.GetUri(lineSpan.Path));
            base.m_writer.WriteObjectEnd();
            WriteRegion(lineSpan);
            base.m_writer.WriteObjectEnd();
        }

        public override void Dispose()
        {
            base.m_writer.WriteArrayEnd();
            WriteTool();
            base.m_writer.Write("columnKind", "utf16CodeUnits");
            base.m_writer.WriteObjectEnd();
            base.m_writer.WriteArrayEnd();
            base.m_writer.WriteObjectEnd();
            base.Dispose();
        }

        private void WriteTool()
        {
            base.m_writer.WriteObjectStart("tool");
            base.m_writer.WriteObjectStart("driver");
            base.m_writer.Write("name", _toolName);
            base.m_writer.Write("version", _toolFileVersion);
            base.m_writer.Write("dottedQuadFileVersion", _toolAssemblyVersion.ToString());
            base.m_writer.Write("semanticVersion", _toolAssemblyVersion.ToString(3));
            base.m_writer.Write("language", base.m_culture.Name);
            WriteRules();
            base.m_writer.WriteObjectEnd();
            base.m_writer.WriteObjectEnd();
        }

        private void WriteRules()
        {
            if (_descriptors.Count <= 0)
            {
                return;
            }
            base.m_writer.WriteArrayStart("rules");
            foreach (KeyValuePair<int, DiagnosticDescriptor> item in _descriptors.ToSortedList())
            {
                DiagnosticDescriptor value = item.Value;
                base.m_writer.WriteObjectStart();
                base.m_writer.Write("id", value.Id);
                string value2 = value.Title.ToString(base.m_culture);
                if (!RoslynString.IsNullOrEmpty(value2))
                {
                    base.m_writer.WriteObjectStart("shortDescription");
                    base.m_writer.Write("text", value2);
                    base.m_writer.WriteObjectEnd();
                }
                string value3 = value.Description.ToString(base.m_culture);
                if (!RoslynString.IsNullOrEmpty(value3))
                {
                    base.m_writer.WriteObjectStart("fullDescription");
                    base.m_writer.Write("text", value3);
                    base.m_writer.WriteObjectEnd();
                }
                WriteDefaultConfiguration(value);
                if (!string.IsNullOrEmpty(value.HelpLinkUri))
                {
                    base.m_writer.Write("helpUri", value.HelpLinkUri);
                }
                if (!string.IsNullOrEmpty(value.Category) || value.CustomTags.Any())
                {
                    base.m_writer.WriteObjectStart("properties");
                    if (!string.IsNullOrEmpty(value.Category))
                    {
                        base.m_writer.Write("category", value.Category);
                    }
                    if (value.CustomTags.Any())
                    {
                        base.m_writer.WriteArrayStart("tags");
                        foreach (string customTag in value.CustomTags)
                        {
                            base.m_writer.Write(customTag);
                        }
                        base.m_writer.WriteArrayEnd();
                    }
                    base.m_writer.WriteObjectEnd();
                }
                base.m_writer.WriteObjectEnd();
            }
            base.m_writer.WriteArrayEnd();
        }

        private void WriteDefaultConfiguration(DiagnosticDescriptor descriptor)
        {
            string level = SarifErrorLogger.GetLevel(descriptor.DefaultSeverity);
            bool flag = level != "warning";
            bool flag2 = !descriptor.IsEnabledByDefault;
            if (flag || flag2)
            {
                base.m_writer.WriteObjectStart("defaultConfiguration");
                if (flag)
                {
                    base.m_writer.Write("level", level);
                }
                if (flag2)
                {
                    base.m_writer.Write("enabled", descriptor.IsEnabledByDefault);
                }
                base.m_writer.WriteObjectEnd();
            }
        }
    }
}
