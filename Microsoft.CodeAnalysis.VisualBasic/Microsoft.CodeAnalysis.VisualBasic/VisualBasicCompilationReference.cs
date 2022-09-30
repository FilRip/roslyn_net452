using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
	internal sealed class VisualBasicCompilationReference : CompilationReference
	{
		private readonly VisualBasicCompilation _compilation;

		public new VisualBasicCompilation Compilation => _compilation;

		internal override Compilation CompilationCore => _compilation;

		public VisualBasicCompilationReference(VisualBasicCompilation compilation, ImmutableArray<string> aliases = default(ImmutableArray<string>), bool embedInteropTypes = false)
			: base(CompilationReference.GetProperties(compilation, aliases, embedInteropTypes))
		{
			VisualBasicCompilation visualBasicCompilation = null;
			visualBasicCompilation = (_compilation = compilation);
		}

		private VisualBasicCompilationReference(VisualBasicCompilation compilation, MetadataReferenceProperties properties)
			: base(properties)
		{
			_compilation = compilation;
		}

		internal override CompilationReference WithPropertiesImpl(MetadataReferenceProperties properties)
		{
			return new VisualBasicCompilationReference(_compilation, properties);
		}

		private string GetDebuggerDisplay()
		{
			return VBResources.CompilationVisualBasic + _compilation.AssemblyName;
		}
	}
}
