using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class CSharpCompilationReference : CompilationReference
    {
        public new CSharpCompilation Compilation { get; }

        public override Compilation CompilationCore => Compilation;

        public CSharpCompilationReference(CSharpCompilation compilation, ImmutableArray<string> aliases = default(ImmutableArray<string>), bool embedInteropTypes = false)
            : base(CompilationReference.GetProperties(compilation, aliases, embedInteropTypes))
        {
            Compilation = compilation;
        }

        private CSharpCompilationReference(CSharpCompilation compilation, MetadataReferenceProperties properties)
            : base(properties)
        {
            Compilation = compilation;
        }

        public override CompilationReference WithPropertiesImpl(MetadataReferenceProperties properties)
        {
            return new CSharpCompilationReference(Compilation, properties);
        }

        private string GetDebuggerDisplay()
        {
            return CSharpResources.CompilationC + Compilation.AssemblyName;
        }
    }
}
