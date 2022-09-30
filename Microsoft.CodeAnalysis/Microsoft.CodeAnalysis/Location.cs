using System;
using System.Diagnostics;

using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    public abstract class Location
    {
        public abstract LocationKind Kind { get; }

        [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "SourceTree")]
        public bool IsInSource
        {
            [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, "SourceTree")]
            get
            {
                return SourceTree != null;
            }
        }

        public bool IsInMetadata => MetadataModuleInternal != null;

        public virtual SyntaxTree? SourceTree => null;

        public IModuleSymbol? MetadataModule => (IModuleSymbol)(MetadataModuleInternal?.GetISymbol());

        internal virtual IModuleSymbolInternal? MetadataModuleInternal => null;

        public virtual TextSpan SourceSpan => default(TextSpan);

        public static Location None => NoLocation.Singleton;

        internal Location()
        {
        }

        public virtual FileLinePositionSpan GetLineSpan()
        {
            return default(FileLinePositionSpan);
        }

        public virtual FileLinePositionSpan GetMappedLineSpan()
        {
            return default(FileLinePositionSpan);
        }

        public abstract override bool Equals(object? obj);

        public abstract override int GetHashCode();

        public override string ToString()
        {
            string text = Kind.ToString();
            if (IsInSource)
            {
                text = text + "(" + SourceTree?.FilePath + SourceSpan.ToString() + ")";
            }
            else if (IsInMetadata)
            {
                if (MetadataModuleInternal != null)
                {
                    text = text + "(" + MetadataModuleInternal!.Name + ")";
                }
            }
            else
            {
                FileLinePositionSpan lineSpan = GetLineSpan();
                if (lineSpan.Path != null)
                {
                    text = text + "(" + lineSpan.Path + "@" + (lineSpan.StartLinePosition.Line + 1) + ":" + (lineSpan.StartLinePosition.Character + 1) + ")";
                }
            }
            return text;
        }

        public static bool operator ==(Location? left, Location? right)
        {
            return left?.Equals(right) ?? ((object)right == null);
        }

        public static bool operator !=(Location? left, Location? right)
        {
            return !(left == right);
        }

        protected virtual string GetDebuggerDisplay()
        {
            string text = GetType().Name;
            FileLinePositionSpan lineSpan = GetLineSpan();
            if (lineSpan.Path != null)
            {
                text = text + "(" + lineSpan.Path + "@" + (lineSpan.StartLinePosition.Line + 1) + ":" + (lineSpan.StartLinePosition.Character + 1) + ")";
            }
            return text;
        }

        public static Location Create(SyntaxTree syntaxTree, TextSpan textSpan)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }
            return new SourceLocation(syntaxTree, textSpan);
        }

        public static Location Create(string filePath, TextSpan textSpan, LinePositionSpan lineSpan)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            return new ExternalFileLocation(filePath, textSpan, lineSpan);
        }
    }
}
