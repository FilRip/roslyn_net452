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
    /// Used for logging compiler diagnostics to a stream in the unstandardized SARIF
    /// (Static Analysis Results Interchange Format) v1.0.0 format.
    /// https://github.com/sarif-standard/sarif-spec
    /// https://rawgit.com/sarif-standard/sarif-spec/main/Static%20Analysis%20Results%20Interchange%20Format%20(SARIF).html
    /// </summary>
    /// <remarks>
    /// To log diagnostics in the standardized SARIF v2.1.0 format, use the SarifV2ErrorLogger.
    /// </remarks>
    internal sealed class SarifV1ErrorLogger : SarifErrorLogger
    {
        private readonly DiagnosticDescriptorSet _descriptors;
        public SarifV1ErrorLogger(Stream stream, string toolName, string toolFileVersion, Version toolAssemblyVersion, CultureInfo culture)
            : base(stream, culture)
        {
            _descriptors = new DiagnosticDescriptorSet();

            My_writer.WriteObjectStart(); // root
            My_writer.Write("$schema", "http://json.schemastore.org/sarif-1.0.0");
            My_writer.Write("version", "1.0.0");
            My_writer.WriteArrayStart("runs");
            My_writer.WriteObjectStart(); // run

            My_writer.WriteObjectStart("tool");
            My_writer.Write("name", toolName);
            My_writer.Write("version", toolAssemblyVersion.ToString());
            My_writer.Write("fileVersion", toolFileVersion);
            My_writer.Write("semanticVersion", toolAssemblyVersion.ToString(fieldCount: 3));
            My_writer.Write("language", culture.Name);
            My_writer.WriteObjectEnd(); // tool

            My_writer.WriteArrayStart("results");
        }

        protected override string PrimaryLocationPropertyName => "resultFile";

        public override void LogDiagnostic(Diagnostic diagnostic, SuppressionInfo? suppressionInfo)
        {
            My_writer.WriteObjectStart(); // result
            My_writer.Write("ruleId", diagnostic.Id);

            string ruleKey = _descriptors.Add(diagnostic.Descriptor);
            if (ruleKey != diagnostic.Id)
            {
                My_writer.Write("ruleKey", ruleKey);
            }

            My_writer.Write("level", GetLevel(diagnostic.Severity));

            string? message = diagnostic.GetMessage(My_culture);
            if (!RoslynString.IsNullOrEmpty(message))
            {
                My_writer.Write("message", message);
            }

            if (diagnostic.IsSuppressed)
            {
                My_writer.WriteArrayStart("suppressionStates");
                My_writer.Write("suppressedInSource");
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

                foreach (var additionalLocation in additionalLocations.Where(al => HasPath(al)))
                {
                    My_writer.WriteObjectStart(); // annotatedCodeLocation
                    My_writer.WriteKey("physicalLocation");

                    WritePhysicalLocation(additionalLocation);

                    My_writer.WriteObjectEnd(); // annotatedCodeLocation
                }

                My_writer.WriteArrayEnd(); // relatedLocations
            }
        }

        protected override void WritePhysicalLocation(Location diagnosticLocation)
        {
            Debug.Assert(HasPath(diagnosticLocation));

            FileLinePositionSpan span = diagnosticLocation.GetLineSpan();

            My_writer.WriteObjectStart();
            My_writer.Write("uri", GetUri(span.Path));

            WriteRegion(span);

            My_writer.WriteObjectEnd();
        }

        private void WriteRules()
        {
            if (_descriptors.Count > 0)
            {
                My_writer.WriteObjectStart("rules");

                foreach (var pair in _descriptors.ToSortedList())
                {
                    DiagnosticDescriptor descriptor = pair.Value;

                    My_writer.WriteObjectStart(pair.Key); // rule
                    My_writer.Write("id", descriptor.Id);

                    string? shortDescription = descriptor.Title.ToString(My_culture);
                    if (!RoslynString.IsNullOrEmpty(shortDescription))
                    {
                        My_writer.Write("shortDescription", shortDescription);
                    }

                    string? fullDescription = descriptor.Description.ToString(My_culture);
                    if (!RoslynString.IsNullOrEmpty(fullDescription))
                    {
                        My_writer.Write("fullDescription", fullDescription);
                    }

                    My_writer.Write("defaultLevel", GetLevel(descriptor.DefaultSeverity));

                    if (!string.IsNullOrEmpty(descriptor.HelpLinkUri))
                    {
                        My_writer.Write("helpUri", descriptor.HelpLinkUri);
                    }

                    My_writer.WriteObjectStart("properties");

                    if (!string.IsNullOrEmpty(descriptor.Category))
                    {
                        My_writer.Write("category", descriptor.Category);
                    }

                    My_writer.Write("isEnabledByDefault", descriptor.IsEnabledByDefault);

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
                    My_writer.WriteObjectEnd(); // rule
                }

                My_writer.WriteObjectEnd(); // rules
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                My_writer.WriteArrayEnd();  // results

                WriteRules();

                My_writer.WriteObjectEnd(); // run
                My_writer.WriteArrayEnd();  // runs
                My_writer.WriteObjectEnd(); // root
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Represents a distinct set of <see cref="DiagnosticDescriptor"/>s and provides unique string keys 
        /// to distinguish them.
        ///
        /// The first <see cref="DiagnosticDescriptor"/> added with a given <see cref="DiagnosticDescriptor.Id"/>
        /// value is given that value as its unique key. Subsequent adds with the same ID will have .NNN
        /// appended to their with an auto-incremented numeric value.
        /// </summary>
        private sealed class DiagnosticDescriptorSet
        {
            // DiagnosticDescriptor.Id -> auto-incremented counter
            private readonly Dictionary<string, int> _counters = new();

            // DiagnosticDescriptor -> unique key
            private readonly Dictionary<DiagnosticDescriptor, string> _keys = new(SarifDiagnosticComparer.Instance);

            /// <summary>
            /// The total number of descriptors in the set.
            /// </summary>
            public int Count => _keys.Count;

            /// <summary>
            /// Adds a descriptor to the set if not already present.
            /// </summary>
            /// <returns>
            /// The unique key assigned to the given descriptor.
            /// </returns>
            public string Add(DiagnosticDescriptor descriptor)
            {
                // Case 1: Descriptor has already been seen -> retrieve key from cache.
                if (_keys.TryGetValue(descriptor, out string? key))
                {
                    return key;
                }

                // Case 2: First time we see a descriptor with a given ID -> use its ID as the key.
                if (!_counters.TryGetValue(descriptor.Id, out int counter))
                {
                    _counters.Add(descriptor.Id, 0);
                    _keys.Add(descriptor, descriptor.Id);
                    return descriptor.Id;
                }

                // Case 3: We've already seen a different descriptor with the same ID -> generate a key.
                //
                // This will only need to loop in the corner case where there is an actual descriptor 
                // with non-generated ID=X.NNN and more than one descriptor with ID=X.
                do
                {
                    _counters[descriptor.Id] = ++counter;
                    key = descriptor.Id + "-" + counter.ToString("000", CultureInfo.InvariantCulture);
                } while (_counters.ContainsKey(key));

                _keys.Add(descriptor, key);
                return key;
            }

            /// <summary>
            /// Converts the set to a list of (key, descriptor) pairs sorted by key.
            /// </summary>
            public List<KeyValuePair<string, DiagnosticDescriptor>> ToSortedList()
            {
                Debug.Assert(Count > 0);

                var list = new List<KeyValuePair<string, DiagnosticDescriptor>>(Count);

                foreach (var pair in _keys)
                {
                    Debug.Assert(list.Capacity > list.Count);
                    list.Add(new KeyValuePair<string, DiagnosticDescriptor>(pair.Value, pair.Key));
                }

                Debug.Assert(list.Capacity == list.Count);
                list.Sort((x, y) => string.CompareOrdinal(x.Key, y.Key));
                return list;
            }
        }
    }
}
