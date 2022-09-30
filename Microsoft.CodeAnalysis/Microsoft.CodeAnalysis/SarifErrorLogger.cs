using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public abstract class SarifErrorLogger : ErrorLogger, IDisposable
    {
        private static readonly Uri s_fileRoot = new("file:///");

        protected JsonWriter m_writer { get; }

        protected CultureInfo m_culture { get; }

        protected abstract string PrimaryLocationPropertyName { get; }

        protected SarifErrorLogger(Stream stream, CultureInfo culture)
        {
            m_writer = new JsonWriter(new StreamWriter(stream));
            m_culture = culture;
        }

        protected abstract void WritePhysicalLocation(Location diagnosticLocation);

        public virtual void Dispose()
        {
            m_writer.Dispose();
        }

        protected void WriteRegion(FileLinePositionSpan span)
        {
            m_writer.WriteObjectStart("region");
            m_writer.Write("startLine", span.StartLinePosition.Line + 1);
            m_writer.Write("startColumn", span.StartLinePosition.Character + 1);
            m_writer.Write("endLine", span.EndLinePosition.Line + 1);
            m_writer.Write("endColumn", span.EndLinePosition.Character + 1);
            m_writer.WriteObjectEnd();
        }

        protected static string GetLevel(DiagnosticSeverity severity)
        {
            return severity switch
            {
                DiagnosticSeverity.Info => "note",
                DiagnosticSeverity.Error => "error",
                _ => "warning",
            };
        }

        protected void WriteResultProperties(Diagnostic diagnostic)
        {
            if (diagnostic.WarningLevel <= 0 && diagnostic.Properties.Count <= 0)
            {
                return;
            }
            m_writer.WriteObjectStart("properties");
            if (diagnostic.WarningLevel > 0)
            {
                m_writer.Write("warningLevel", diagnostic.WarningLevel);
            }
            if (diagnostic.Properties.Count > 0)
            {
                m_writer.WriteObjectStart("customProperties");
                foreach (KeyValuePair<string, string> item in diagnostic.Properties.OrderBy<KeyValuePair<string, string>, string>((KeyValuePair<string, string> x) => x.Key, StringComparer.Ordinal))
                {
                    m_writer.Write(item.Key, item.Value);
                }
                m_writer.WriteObjectEnd();
            }
            m_writer.WriteObjectEnd();
        }

        protected static bool HasPath(Location location)
        {
            return !string.IsNullOrEmpty(location.GetLineSpan().Path);
        }

        protected static string GetUri(string path)
        {
            if (Path.IsPathRooted(path))
            {
                if (Uri.TryCreate(Path.GetFullPath(path), UriKind.Absolute, out var result))
                {
                    return result.AbsoluteUri;
                }
            }
            else
            {
                if (!PathUtilities.IsUnixLikePlatform)
                {
                    path = path.Replace("\\\\", "\\");
                    path = PathUtilities.NormalizeWithForwardSlash(path);
                }
                if (Uri.TryCreate(path, UriKind.Relative, out var result2))
                {
                    return s_fileRoot.MakeRelativeUri(new Uri(s_fileRoot, result2)).ToString();
                }
            }
            return WebUtility.UrlEncode(path);
        }
    }
}
