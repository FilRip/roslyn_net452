using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedRecordOrdinaryMethod : SourceOrdinaryMethodSymbolBase
    {
        private readonly int _memberOffset;

        protected sealed override bool HasAnyBody => true;

        internal sealed override bool IsExpressionBodied => false;

        public sealed override bool IsImplicitlyDeclared => true;

        protected sealed override Location ReturnTypeLocation => Locations[0];

        protected sealed override TypeSymbol? ExplicitInterfaceType => null;

        protected sealed override SourceMemberMethodSymbol? BoundAttributesSource => null;

        public sealed override bool IsVararg => false;

        public sealed override RefKind RefKind => RefKind.None;

        internal sealed override bool GenerateDebugInfo => false;

        internal sealed override bool SynthesizesLoweredBoundBody => true;

        protected SynthesizedRecordOrdinaryMethod(SourceMemberContainerTypeSymbol containingType, string name, bool hasBody, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, name, containingType.Locations[0], (CSharpSyntaxNode)containingType.SyntaxReferences[0].GetSyntax(), MethodKind.Ordinary, isIterator: false, isExtensionMethod: false, isPartial: false, hasBody, isNullableAnalysisEnabled: false, diagnostics)
        {
            _memberOffset = memberOffset;
        }

        protected sealed override MethodSymbol? FindExplicitlyImplementedMethod(BindingDiagnosticBag diagnostics)
        {
            return null;
        }

        internal sealed override LexicalSortKey GetLexicalSortKey()
        {
            return LexicalSortKey.GetSynthesizedMemberKey(_memberOffset);
        }

        protected sealed override ImmutableArray<TypeParameterSymbol> MakeTypeParameters(CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
        {
            return ImmutableArray<TypeParameterSymbol>.Empty;
        }

        public sealed override ImmutableArray<ImmutableArray<TypeWithAnnotations>> GetTypeParameterConstraintTypes()
        {
            return ImmutableArray<ImmutableArray<TypeWithAnnotations>>.Empty;
        }

        public sealed override ImmutableArray<TypeParameterConstraintKind> GetTypeParameterConstraintKinds()
        {
            return ImmutableArray<TypeParameterConstraintKind>.Empty;
        }

        protected sealed override void PartialMethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        protected sealed override void ExtensionMethodChecks(BindingDiagnosticBag diagnostics)
        {
        }

        protected sealed override void CompleteAsyncMethodChecksBetweenStartAndFinish()
        {
        }

        protected sealed override void CheckConstraintsForExplicitInterfaceType(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
        }

        internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
        }

        public sealed override string? GetDocumentationCommentXml(CultureInfo? preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }
    }
}
