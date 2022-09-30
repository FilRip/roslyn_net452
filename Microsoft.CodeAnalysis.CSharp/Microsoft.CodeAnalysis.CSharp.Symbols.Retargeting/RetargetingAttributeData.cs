using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting
{
    internal sealed class RetargetingAttributeData : SourceAttributeData
    {
        internal RetargetingAttributeData(SyntaxReference applicationNode, NamedTypeSymbol attributeClass, MethodSymbol attributeConstructor, ImmutableArray<TypedConstant> constructorArguments, ImmutableArray<int> constructorArgumentsSourceIndices, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, bool hasErrors, bool isConditionallyOmitted)
            : base(applicationNode, attributeClass, attributeConstructor, constructorArguments, constructorArgumentsSourceIndices, namedArguments, hasErrors, isConditionallyOmitted)
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
