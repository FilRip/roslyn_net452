// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Used for logging compiler diagnostics to a stream in the standardized SARIF
    /// (Static Analysis Results Interchange Format) v2.1.0 format.
    /// http://docs.oasis-open.org/sarif/sarif/v2.1.0/sarif-v2.1.0.html
    /// </summary>
    internal sealed class SarifV2ErrorLogger : SarifErrorLogger, IDisposable
    {
        private readonly DiagnosticDescriptorSet _descriptors;

        private readonly string _toolName;
        private readonly string _toolFileVersion;
        private readonly Version _toolAssemblyVersion;

        public SarifV2ErrorLogger(Stream stream, string toolName, string toolFileVersion, Version toolAssemblyVersion, CultureInfo culture)
            : base(stream, culture)
        {
            _descriptors = new DiagnosticDescriptorSet();

            _toolName = toolName;
            _toolFileVersion = toolFileVersion;
            _toolAssemblyVersion = toolAssemblyVersion;

            My_writer.WriteObjectStart(); // root
            My_writer.Write("$schema", "http://json.schemastore.org/sarif-2.1.0");
            My_writer.Write("version", "2.1.0");
            My_writer.WriteArrayStart("runs");
            My_writer.WriteObjectStart(); // run

            My_writer.WriteArrayStart("results");
        }

        protected override string PrimaryLocationPropertyName => "physicalLocation";

        public override void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo)
        {
            My_writer.WriteObjectStart(); // result
            My_writer.Write("ruleId", diagnostic.Id);
            int ruleIndex = _descriptors.Add(diagnostic.Descriptor);
            My_writer.Write("ruleIndex", ruleIndex);

            My_writer.Write("level", GetLevel(diagnostic.Severity));

            string? message = diagnostic.GetMessage(My_culture);
            if (!RoslynString.IsNullOrEmpty(message))
            {
                My_writer.WriteObjectStart("message");
                My_writer.Write("text", message);
                My_writer.WriteObjectEnd();
            }

            if (diagnostic.IsSuppressed)
            {
                My_writer.WriteArrayStart("suppressions");
                My_writer.WriteObjectStart(); // suppression
                My_writer.Write("kind", "inSource");
                string? justification = suppressionInfo?.Attribute?.DecodeNamedArgument<string>("Justification", SpecialType.System_String);
                if (justification != null)
                {
                    My_writer.Write("justification", justification);
                }

                My_writer.WriteObjectEnd(); // suppression
                My_writer.WriteArrayEnd();
            }

            WriteLocations(diagnostic.Location, diagnostic.AdditionalLocations);

            WriteResultProperties(diagnostic);

            My_writer.WriteObjectEnd(); // result
        }

        private void WriteLocations(Location location, IReadOnlyList<Location> additionalLocations)
        {
            if (HasPath(location))
            {
                My_writer.WriteArrayStart("locations");
                My_writer.WriteObjectStart(); // location
                My_writer.WriteKey(PrimaryLocationPropertyName);

                WritePhysicalLocation(location);

                My_writer.WriteObjectEnd(); // location
                My_writer.WriteArrayEnd(); // locations
            }

            // See https://github.com/dotnet/roslyn/issues/11228 for discussion around
            // whether this is the correct treatment of Diagnostic.AdditionalLocations
            // as SARIF relatedLocations.
            if (additionalLocations != null &&
                additionalLocations.Count > 0 &&
                additionalLocations.Any(l => HasPath(l)))
            {
                My_writer.WriteArrayStart("relatedLocations");

                foreach (var additionalLocation in additionalLocations)
                {
                    if (HasPath(additionalLocation))
                    {
                        My_writer.WriteObjectStart(); // annotatedCodeLocation
                        My_writer.WriteKey("physicalLocation");

                        WritePhysicalLocation(additionalLocation);

                        My_writer.WriteObjectEnd(); // annotatedCodeLocation
                    }
                }

                My_writer.WriteArrayEnd(); // relatedLocations
            }
        }

        protected override void WritePhysicalLocation(Location diagnosticLocation)
        {
            Debug.Assert(HasPath(diagnosticLocation));

            FileLinePositionSpan span = diagnosticLocation.GetLineSpan();

            My_writer.WriteObjectStart(); // physicalLocation

            My_writer.WriteObjectStart("artifactLocation");
            My_writer.Write("uri", GetUri(span.Path));
            My_writer.WriteObjectEnd(); // artifactLocation

            WriteRegion(span);

            My_writer.WriteObjectEnd();
        }

        public override void Dispose()
        {
            My_writer.WriteArrayEnd(); //results

            WriteTool();

            My_writer.Write("columnKind", "utf16CodeUnits");

            My_writer.WriteObjectEnd(); // run
            My_writer.WriteArrayEnd();  // runs
            My_writer.WriteObjectEnd(); // root
            base.Dispose();
        }

        private void WriteTool()
        {
            My_writer.WriteObjectStart("tool");
            My_writer.WriteObjectStart("driver");
            My_writer.Write("name", _toolName);
            My_writer.Write("version", _toolFileVersion);
            My_writer.Write("dottedQuadFileVersion", _toolAssemblyVersion.ToString());
            My_writer.Write("semanticVersion", _toolAssemblyVersion.ToString(fieldCount: 3));
            My_writer.Write("language", My_culture.Name);

            WriteRules();

            My_writer.WriteObjectEnd(); // driver
            My_writer.WriteObjectEnd(); // tool
        }

        private void WriteRules()
        {
            if (_descriptors.Count > 0)
            {
                My_writer.WriteArrayStart("rules");

                foreach (var pair in _descriptors.ToSortedList())
                {
                    DiagnosticDescriptor descriptor = pair.Value;

                    My_writer.WriteObjectStart(); // rule
                    My_writer.Write("id", descriptor.Id);

                    string? shortDescription = descriptor.Title.ToString(My_culture);
                    if (!RoslynString.IsNullOrEmpty(shortDescription))
                    {
                        My_writer.WriteObjectStart("shortDescription");
                        My_writer.Write("text", shortDescription);
                        My_writer.WriteObjectEnd();
                    }

                    string? fullDescription = descriptor.Description.ToString(My_culture);
                    if (!RoslynString.IsNullOrEmpty(fullDescription))
                    {
                        My_writer.WriteObjectStart("fullDescription");
                        My_writer.Write("text", fullDescription);
                        My_writer.WriteObjectEnd();
                    }

                    WriteDefaultConfiguration(descriptor);

                    if (!string.IsNullOrEmpty(descriptor.HelpLinkUri))
                    {
                        My_writer.Write("helpUri", descriptor.HelpLinkUri);
                    }

                    if (!string.IsNullOrEmpty(descriptor.Category) || descriptor.CustomTags.Any())
                    {
                        My_writer.WriteObjectStart("properties");

                        if (!string.IsNullOrEmpty(descriptor.Category))
                        {
                            My_writer.Write("category", descriptor.Category);
                        }

                        if (descriptor.CustomTags.Any())
                        {
                            My_writer.WriteArrayStart("tags");

                            foreach (string tag in descriptor.CustomTags)
                            {
                                My_writer.Write(tag);
                            }

                            My_writer.WriteArrayEnd(); // tags
                        }

                        My_writer.WriteObjectEnd(); // properties
                    }

                    My_writer.WriteObjectEnd(); // rule
                }

                My_writer.WriteArrayEnd(); // rules
            }
        }

        private void WriteDefaultConfiguration(DiagnosticDescriptor descriptor)
        {
            string defaultLevel = GetLevel(descriptor.DefaultSeverity);

            // Don't bother to emit default values.
            bool emitLevel = defaultLevel != "warning";

            // The default value for "enabled" is "true".
            bool emitEnabled = !descriptor.IsEnabledByDefault;

            if (emitLevel || emitEnabled)
            {
                My_writer.WriteObjectStart("defaultConfiguration");

                if (emitLevel)
                {
                    My_writer.Write("level", defaultLevel);
                }

                if (emitEnabled)
                {
                    My_writer.Write("enabled", descriptor.IsEnabledByDefault);
                }

                My_writer.WriteObjectEnd(); // defaultConfiguration
            }
        }

        /// <summary>
        /// Represents a distinct set of <see cref="DiagnosticDescriptor"/>s and provides unique integer indices
        /// to distinguish them.
        /// </summary>
        private sealed class DiagnosticDescriptorSet
        {
            // DiagnosticDescriptor -> integer index
            private readonly Dictionary<DiagnosticDescriptor, int> _distinctDescriptors = new Dictionary<DiagnosticDescriptor, int>(SarifDiagnosticComparer.Instance);

            /// <summary>
            /// The total number of descriptors in the set.
            /// </summary>
            public int Count => _distinctDescriptors.Count;

            /// <summary>
            /// Adds a descriptor to the set if not already present.
            /// </summary>
            /// <returns>
            /// The unique key assigned to the given descriptor.
            /// </returns>
            public int Add(DiagnosticDescriptor descriptor)
            {
                if (_distinctDescriptors.TryGetValue(descriptor, out int index))
                {
                    // Descriptor has already been seen.
                    return index;
                }
                else
                {
                    _distinctDescriptors.Add(descriptor, Count);
                    return Count - 1;
                }
            }

            /// <summary>
            /// Converts the set to a list, sorted by index.
            /// </summary>
            public List<KeyValuePair<int, DiagnosticDescriptor>> ToSortedList()
            {
                Debug.Assert(Count > 0);

                var list = new List<KeyValuePair<int, DiagnosticDescriptor>>(Count);

                foreach (var pair in _distinctDescriptors)
                {
                    Debug.Assert(list.Capacity > list.Count);
                    list.Add(new KeyValuePair<int, DiagnosticDescriptor>(pair.Value, pair.Key));
                }

                Debug.Assert(list.Capacity == list.Count);
                list.Sort((x, y) => x.Key.CompareTo(y.Key));
                return list;
            }
        }
    }
}
