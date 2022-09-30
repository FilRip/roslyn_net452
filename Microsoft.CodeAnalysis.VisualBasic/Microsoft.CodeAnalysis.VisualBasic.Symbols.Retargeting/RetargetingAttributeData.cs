using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal class RetargetingAttributeData : SourceAttributeData
	{
		internal RetargetingAttributeData(SyntaxReference applicationNode, NamedTypeSymbol attributeClass, MethodSymbol attributeConstructor, ImmutableArray<TypedConstant> constructorArguments, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, bool isConditionallyOmitted, bool hasErrors)
			: base(applicationNode, attributeClass, attributeConstructor, constructorArguments, namedArguments, isConditionallyOmitted, hasErrors)
		{
		}

		internal override TypeSymbol GetSystemType(Symbol targetSymbol)
		{
			RetargetingAssemblySymbol obj = (RetargetingAssemblySymbol)((targetSymbol.Kind == SymbolKind.Assembly) ? targetSymbol : targetSymbol.ContainingAssembly);
			TypeSymbol wellKnownType = obj.UnderlyingAssembly.DeclaringCompilation.GetWellKnownType(WellKnownType.System_Type);
			return ((RetargetingModuleSymbol)obj.Modules[0]).RetargetingTranslator.Retarget(wellKnownType, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
		}
	}
}
