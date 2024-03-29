// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Base class for the <see cref="SarifV1ErrorLogger"/> and <see cref="SarifV2ErrorLogger"/> classes.
    /// The SarifV2ErrorLogger produces the standardized SARIF v2.1.0. The SarifV1ErrorLogger produces
    /// the non-standardized SARIF v1.0.0. It is retained (and in fact, is retained as the default)
    /// for compatibility with previous versions of the compiler. Customers who want to integrate
    /// with standardized SARIF tooling should specify /errorlog:logFilePath;version=2 on the command
    /// line to produce SARIF v2.1.0.
    /// </summary>
    public abstract class SarifErrorLogger : ErrorLogger, IDisposable
    {
        protected JsonWriter My_writer { get; }
        protected CultureInfo My_culture { get; }

        protected SarifErrorLogger(Stream stream, CultureInfo culture)
        {
            Debug.Assert(stream != null);
            Debug.Assert(stream!.Position == 0);

            My_writer = new JsonWriter(new StreamWriter(stream));
            My_culture = culture;
        }

        protected abstract string PrimaryLocationPropertyName { get; }

        protected abstract void WritePhysicalLocation(Location diagnosticLocation);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                My_writer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void WriteRegion(FileLinePositionSpan span)
        {
            // Note that SARIF lines and columns are 1-based, but FileLinePositionSpan is 0-based
            My_writer.WriteObjectStart("region");
            My_writer.Write("startLine", span.StartLinePosition.Line + 1);
            My_writer.Write("startColumn", span.StartLinePosition.Character + 1);
            My_writer.Write("endLine", span.EndLinePosition.Line + 1);
            My_writer.Write("endColumn", span.EndLinePosition.Character + 1);
            My_writer.WriteObjectEnd(); // region
        }

        protected static string GetLevel(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Info:
                    return "note";

                case DiagnosticSeverity.Error:
                    return "error";

                default:
                    // hidden diagnostics are not reported on the command line and therefore not currently given to
                    // the error logger. We could represent it with a custom property in the SARIF log if that changes.
                    if (severity != DiagnosticSeverity.Warning)
                        Debug.Assert(false);
                    return "warning";
            }
        }

        protected void WriteResultProperties(Diagnostic diagnostic)
        {
            // Currently, the following are always inherited from the descriptor and therefore will be
            // captured as rule metadata and need not be logged here. IsWarningAsError is also omitted
            // because it can be inferred from level vs. defaultLevel in the log.
            Debug.Assert(diagnostic.CustomTags.SequenceEqual(diagnostic.Descriptor.CustomTags));
            Debug.Assert(diagnostic.Category == diagnostic.Descriptor.Category);
            Debug.Assert(diagnostic.DefaultSeverity == diagnostic.Descriptor.DefaultSeverity);
            Debug.Assert(diagnostic.IsEnabledByDefault == diagnostic.Descriptor.IsEnabledByDefault);

            if (diagnostic.WarningLevel > 0 || diagnostic.Properties.Count > 0)
            {
                My_writer.WriteObjectStart("properties");

                if (diagnostic.WarningLevel > 0)
                {
                    My_writer.Write("warningLevel", diagnostic.WarningLevel);
                }

                if (diagnostic.Properties.Count > 0)
                {
                    My_writer.WriteObjectStart("customProperties");

                    foreach (var pair in diagnostic.Properties.OrderBy(x => x.Key, StringComparer.Ordinal))
                    {
                        My_writer.Write(pair.Key, pair.Value);
                    }

                    My_writer.WriteObjectEnd();
                }

                My_writer.WriteObjectEnd(); // properties
            }
        }

        protected static bool HasPath(Location location)
        {
            return !string.IsNullOrEmpty(location.GetLineSpan().Path);
        }

        private static readonly Uri s_fileRoot = new("file:///");

        protected static string GetUri(string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));

            // Note that in general, these "paths" are opaque strings to be
            // interpreted by resolvers (see SyntaxTree.FilePath documentation).

            // Common case: absolute path -> absolute URI
            if (Path.IsPathRooted(path))
            {
                // N.B. URI does not handle multiple backslashes or `..` well, so call GetFullPath
                // to normalize before going to URI
                var fullPath = Path.GetFullPath(path);
                if (Uri.TryCreate(fullPath, UriKind.Absolute, out var uri))
                {
                    // We use Uri.AbsoluteUri and not Uri.ToString() because Uri.ToString()
                    // is unescaped (e.g. spaces remain unreplaced by %20) and therefore
                    // not well-formed.
                    return uri.AbsoluteUri;
                }
            }
            else
            {
                // Attempt to normalize the directory separators
                if (!PathUtilities.IsUnixLikePlatform)
                {
                    path = path.Replace(@"\\", @"\");
                    path = PathUtilities.NormalizeWithForwardSlash(path);
                }

                if (Uri.TryCreate(path, UriKind.Relative, out var uri))
                {
                    // First fallback attempt: attempt to interpret as relative path/URI.
                    // (Perhaps the resolver works that way.)
                    // There is no AbsoluteUri equivalent for relative URI references and ToString()
                    // won't escape without this relative -> absolute -> relative trick.
                    return s_fileRoot.MakeRelativeUri(new Uri(s_fileRoot, uri)).ToString();
                }
            }

            // Last resort: UrlEncode the whole opaque string.
            return System.Net.WebUtility.UrlEncode(path);
        }
    }
}
