using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class PEAssemblyBuilder : PEAssemblyBuilderBase
	{
		internal override bool AllowOmissionOfConditionalCalls => true;

		public override int CurrentGenerationOrdinal => 0;

		public PEAssemblyBuilder(SourceAssemblySymbol sourceAssembly, EmitOptions emitOptions, OutputKind outputKind, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources, ImmutableArray<NamedTypeSymbol> additionalTypes = default(ImmutableArray<NamedTypeSymbol>))
			: base(sourceAssembly, emitOptions, outputKind, serializationProperties, manifestResources, additionalTypes)
		{
		}
	}
}
