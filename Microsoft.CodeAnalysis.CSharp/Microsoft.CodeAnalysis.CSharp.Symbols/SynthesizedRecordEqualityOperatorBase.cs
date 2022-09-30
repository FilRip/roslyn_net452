using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedRecordEqualityOperatorBase : SourceUserDefinedOperatorSymbolBase
    {
        private readonly int _memberOffset;

        public sealed override bool IsImplicitlyDeclared => true;

        protected sealed override Location ReturnTypeLocation => Locations[0];

        protected sealed override SourceMemberMethodSymbol? BoundAttributesSource => null;

        internal sealed override bool GenerateDebugInfo => false;

        internal sealed override bool SynthesizesLoweredBoundBody => true;

        protected SynthesizedRecordEqualityOperatorBase(SourceMemberContainerTypeSymbol containingType, string name, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(MethodKind.UserDefinedOperator, name, containingType, containingType.Locations[0], (CSharpSyntaxNode)containingType.SyntaxReferences[0].GetSyntax(), DeclarationModifiers.Static | DeclarationModifiers.Public, hasBody: true, isExpressionBodied: false, isIterator: false, isNullableAnalysisEnabled: false, diagnostics)
        {
            _memberOffset = memberOffset;
        }

        internal sealed override LexicalSortKey GetLexicalSortKey()
        {
            return LexicalSortKey.GetSynthesizedMemberKey(_memberOffset);
        }

        internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(default(SyntaxList<AttributeListSyntax>));
        }

        public sealed override string? GetDocumentationCommentXml(CultureInfo? preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        protected sealed override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            NullableAnnotation nullableAnnotation = (ContainingType.IsRecordStruct ? NullableAnnotation.Oblivious : NullableAnnotation.Annotated);
            return (TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Boolean, returnTypeLocation, diagnostics)), ImmutableArray.Create(new SourceSimpleParameterSymbol(this, TypeWithAnnotations.Create(ContainingType, nullableAnnotation), 0, RefKind.None, "left", Locations), (ParameterSymbol)new SourceSimpleParameterSymbol(this, TypeWithAnnotations.Create(ContainingType, nullableAnnotation), 1, RefKind.None, "right", Locations)));
        }

        protected override int GetParameterCountFromSyntax()
        {
            return 2;
        }
    }
}
