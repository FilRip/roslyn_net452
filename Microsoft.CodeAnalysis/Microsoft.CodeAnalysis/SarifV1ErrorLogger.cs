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
    internal sealed class SarifV1ErrorLogger : SarifErrorLogger, IDisposable
    {
        private sealed class DiagnosticDescriptorSet
        {
            private readonly Dictionary<string, int> _counters = new Dictionary<string, int>();

            private readonly Dictionary<DiagnosticDescriptor, string> _keys = new Dictionary<DiagnosticDescriptor, string>(SarifDiagnosticComparer.Instance);

            public int Count => _keys.Count;

            public string Add(DiagnosticDescriptor descriptor)
            {
                if (_keys.TryGetValue(descriptor, out var value))
                {
                    return value;
                }
                if (!_counters.TryGetValue(descriptor.Id, out var value2))
                {
                    _counters.Add(descriptor.Id, 0);
                    _keys.Add(descriptor, descriptor.Id);
                    return descriptor.Id;
                }
                do
                {
                    value2 = (_counters[descriptor.Id] = value2 + 1);
                    value = descriptor.Id + "-" + value2.ToString("000", CultureInfo.InvariantCulture);
                }
                while (_counters.ContainsKey(value));
                _keys.Add(descriptor, value);
                return value;
            }

            public List<KeyValuePair<string, DiagnosticDescriptor>> ToSortedList()
            {
                List<KeyValuePair<string, DiagnosticDescriptor>> list = new List<KeyValuePair<string, DiagnosticDescriptor>>(Count);
                foreach (KeyValuePair<DiagnosticDescriptor, string> key in _keys)
                {
                    list.Add(new KeyValuePair<string, DiagnosticDescriptor>(key.Value, key.Key));
                }
                list.Sort((KeyValuePair<string, DiagnosticDescriptor> x, KeyValuePair<string, DiagnosticDescriptor> y) => string.CompareOrdinal(x.Key, y.Key));
                return list;
            }
        }

        private readonly DiagnosticDescriptorSet _descriptors;

        protected override string PrimaryLocationPropertyName => "resultFile";

        public SarifV1ErrorLogger(Stream stream, string toolName, string toolFileVersion, Version toolAssemblyVersion, CultureInfo culture)
            : base(stream, culture)
        {
            _descriptors = new DiagnosticDescriptorSet();
            base.m_writer.WriteObjectStart();
            base.m_writer.Write("$schema", "http://json.schemastore.org/sarif-1.0.0");
            base.m_writer.Write("version", "1.0.0");
            base.m_writer.WriteArrayStart("runs");
            base.m_writer.WriteObjectStart();
            base.m_writer.WriteObjectStart("tool");
            base.m_writer.Write("name", toolName);
            base.m_writer.Write("version", toolAssemblyVersion.ToString());
            base.m_writer.Write("fileVersion", toolFileVersion);
            base.m_writer.Write("semanticVersion", toolAssemblyVersion.ToString(3));
            base.m_writer.Write("language", culture.Name);
            base.m_writer.WriteObjectEnd();
            base.m_writer.WriteArrayStart("results");
        }

        public override void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo)
        {
            base.m_writer.WriteObjectStart();
            base.m_writer.Write("ruleId", diagnostic.Id);
            string text = _descriptors.Add(diagnostic.Descriptor);
            if (text != diagnostic.Id)
            {
                base.m_writer.Write("ruleKey", text);
            }
            base.m_writer.Write("level", SarifErrorLogger.GetLevel(diagnostic.Severity));
            string message = diagnostic.GetMessage(base.m_culture);
            if (!RoslynString.IsNullOrEmpty(message))
            {
                base.m_writer.Write("message", message);
            }
            if (diagnostic.IsSuppressed)
            {
                base.m_writer.WriteArrayStart("suppressionStates");
                base.m_writer.Write("suppressedInSource");
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

        protected override void WritePhysicalLocation(Location location)
        {
            FileLinePositionSpan lineSpan = location.GetLineSpan();
            base.m_writer.WriteObjectStart();
            base.m_writer.Write("uri", SarifErrorLogger.GetUri(lineSpan.Path));
            WriteRegion(lineSpan);
            base.m_writer.WriteObjectEnd();
        }

        private void WriteRules()
        {
            if (_descriptors.Count <= 0)
            {
                return;
            }
            base.m_writer.WriteObjectStart("rules");
            foreach (KeyValuePair<string, DiagnosticDescriptor> item in _descriptors.ToSortedList())
            {
                DiagnosticDescriptor value = item.Value;
                base.m_writer.WriteObjectStart(item.Key);
                base.m_writer.Write("id", value.Id);
                string value2 = value.Title.ToString(base.m_culture);
                if (!RoslynString.IsNullOrEmpty(value2))
                {
                    base.m_writer.Write("shortDescription", value2);
                }
                string value3 = value.Description.ToString(base.m_culture);
                if (!RoslynString.IsNullOrEmpty(value3))
                {
                    base.m_writer.Write("fullDescription", value3);
                }
                base.m_writer.Write("defaultLevel", SarifErrorLogger.GetLevel(value.DefaultSeverity));
                if (!string.IsNullOrEmpty(value.HelpLinkUri))
                {
                    base.m_writer.Write("helpUri", value.HelpLinkUri);
                }
                base.m_writer.WriteObjectStart("properties");
                if (!string.IsNullOrEmpty(value.Category))
                {
                    base.m_writer.Write("category", value.Category);
                }
                base.m_writer.Write("isEnabledByDefault", value.IsEnabledByDefault);
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
                base.m_writer.WriteObjectEnd();
            }
            base.m_writer.WriteObjectEnd();
        }

        public override void Dispose()
        {
            base.m_writer.WriteArrayEnd();
            WriteRules();
            base.m_writer.WriteObjectEnd();
            base.m_writer.WriteArrayEnd();
            base.m_writer.WriteObjectEnd();
            base.Dispose();
        }
    }
}
