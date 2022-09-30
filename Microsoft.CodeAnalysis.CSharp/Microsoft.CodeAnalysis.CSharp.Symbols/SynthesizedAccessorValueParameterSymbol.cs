using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedAccessorValueParameterSymbol : SourceComplexParameterSymbol
    {
        internal override FlowAnalysisAnnotations FlowAnalysisAnnotations
        {
            get
            {
                FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations.None;
                if (ContainingSymbol is SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol && sourcePropertyAccessorSymbol.AssociatedSymbol is SourcePropertySymbolBase sourcePropertySymbolBase)
                {
                    if (sourcePropertySymbolBase.HasDisallowNull)
                    {
                        flowAnalysisAnnotations |= FlowAnalysisAnnotations.DisallowNull;
                    }
                    if (sourcePropertySymbolBase.HasAllowNull)
                    {
                        flowAnalysisAnnotations |= FlowAnalysisAnnotations.AllowNull;
                    }
                }
                return flowAnalysisAnnotations;
            }
        }

        internal override ImmutableHashSet<string> NotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public override bool IsImplicitlyDeclared => true;

        protected override IAttributeTargetSymbol AttributeOwner => (SourceMemberMethodSymbol)ContainingSymbol;

        public SynthesizedAccessorValueParameterSymbol(SourceMemberMethodSymbol accessor, TypeWithAnnotations paramType, int ordinal)
            : base(accessor, ordinal, paramType, RefKind.None, "value", accessor.Locations, null, isParams: false, isExtensionMethodThis: false)
        {
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return ((SourceMemberMethodSymbol)ContainingSymbol).GetAttributeDeclarations();
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            if (ContainingSymbol is SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol && sourcePropertyAccessorSymbol.AssociatedSymbol is SourcePropertySymbolBase sourcePropertySymbolBase)
            {
                FlowAnalysisAnnotations flowAnalysisAnnotations = FlowAnalysisAnnotations;
                if ((flowAnalysisAnnotations & FlowAnalysisAnnotations.DisallowNull) != 0)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, new SynthesizedAttributeData(sourcePropertySymbolBase.DisallowNullAttributeIfExists));
                }
                if ((flowAnalysisAnnotations & FlowAnalysisAnnotations.AllowNull) != 0)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, new SynthesizedAttributeData(sourcePropertySymbolBase.AllowNullAttributeIfExists));
                }
            }
        }
    }
}
