using System.Diagnostics;
using System.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal sealed class MetadataImageReference : PortableExecutableReference
    {
        private readonly string? _display;

        private readonly Metadata _metadata;

        public override string Display
        {
            get
            {
                string text = _display;
                if (text == null)
                {
                    text = base.FilePath;
                    if (text == null)
                    {
                        if (base.Properties.Kind != 0)
                        {
                            return CodeAnalysisResources.InMemoryModule;
                        }
                        text = CodeAnalysisResources.InMemoryAssembly;
                    }
                }
                return text;
            }
        }

        internal MetadataImageReference(Metadata metadata, MetadataReferenceProperties properties, DocumentationProvider? documentation, string? filePath, string? display)
            : base(properties, filePath, documentation ?? Microsoft.CodeAnalysis.DocumentationProvider.Default)
        {
            _display = display;
            _metadata = metadata;
        }

        protected override Metadata GetMetadataImpl()
        {
            return _metadata;
        }

        protected override DocumentationProvider CreateDocumentationProvider()
        {
            throw ExceptionUtilities.Unreachable;
        }

        protected override PortableExecutableReference WithPropertiesImpl(MetadataReferenceProperties properties)
        {
            return new MetadataImageReference(_metadata, properties, base.DocumentationProvider, base.FilePath, _display);
        }

        private string GetDebuggerDisplay()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append((base.Properties.Kind == MetadataImageKind.Module) ? "Module" : "Assembly");
            if (!base.Properties.Aliases.IsEmpty)
            {
                stringBuilder.Append(" Aliases={");
                stringBuilder.Append(string.Join(", ", base.Properties.Aliases));
                stringBuilder.Append("}");
            }
            if (base.Properties.EmbedInteropTypes)
            {
                stringBuilder.Append(" Embed");
            }
            if (base.FilePath != null)
            {
                stringBuilder.Append(" Path='");
                stringBuilder.Append(base.FilePath);
                stringBuilder.Append("'");
            }
            if (_display != null)
            {
                stringBuilder.Append(" Display='");
                stringBuilder.Append(_display);
                stringBuilder.Append("'");
            }
            return stringBuilder.ToString();
        }
    }
}
