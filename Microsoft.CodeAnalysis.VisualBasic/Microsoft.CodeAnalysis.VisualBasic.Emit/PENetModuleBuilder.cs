using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Emit
{
	internal sealed class PENetModuleBuilder : PEModuleBuilder
	{
		internal override bool AllowOmissionOfConditionalCalls => true;

		public override int CurrentGenerationOrdinal => 0;

		public override ISourceAssemblySymbolInternal SourceAssemblyOpt => null;

		internal PENetModuleBuilder(SourceModuleSymbol sourceModule, EmitOptions emitOptions, ModulePropertiesForSerialization serializationProperties, IEnumerable<ResourceDescription> manifestResources)
			: base(sourceModule, emitOptions, OutputKind.NetModule, serializationProperties, manifestResources)
		{
		}

		protected override void AddEmbeddedResourcesFromAddedModules(ArrayBuilder<ManagedResource> builder, DiagnosticBag diagnostics)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override IEnumerable<IFileReference> GetFiles(EmitContext context)
		{
			return SpecializedCollections.EmptyEnumerable<IFileReference>();
		}
	}
}
