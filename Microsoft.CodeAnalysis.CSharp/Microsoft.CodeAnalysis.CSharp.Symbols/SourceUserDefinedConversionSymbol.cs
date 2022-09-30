using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceUserDefinedConversionSymbol : SourceUserDefinedOperatorSymbolBase
    {
        protected override Location ReturnTypeLocation => GetSyntax().Type.Location;

        internal override bool GenerateDebugInfo => true;

        public static SourceUserDefinedConversionSymbol CreateUserDefinedConversionSymbol(SourceMemberContainerTypeSymbol containingType, ConversionOperatorDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
        {
            Location location = syntax.Type.Location;
            string name = (syntax.ImplicitOrExplicitKeyword.IsKind(SyntaxKind.ImplicitKeyword) ? "op_Implicit" : "op_Explicit");
            return new SourceUserDefinedConversionSymbol(containingType, name, location, syntax, isNullableAnalysisEnabled, diagnostics);
        }

        private SourceUserDefinedConversionSymbol(SourceMemberContainerTypeSymbol containingType, string name, Location location, ConversionOperatorDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(MethodKind.Conversion, name, containingType, location, syntax, SourceUserDefinedOperatorSymbolBase.MakeDeclarationModifiers(syntax, location, diagnostics), syntax.HasAnyBody(), syntax.Body == null && syntax.ExpressionBody != null, SyntaxFacts.HasYieldOperations(syntax.Body), isNullableAnalysisEnabled, diagnostics)
        {
            Symbol.CheckForBlockAndExpressionBody(syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
            if (syntax.ParameterList.Parameters.Count != 1)
            {
                diagnostics.Add(ErrorCode.ERR_OvlUnaryOperatorExpected, syntax.ParameterList.GetLocation());
            }
        }

        internal ConversionOperatorDeclarationSyntax GetSyntax()
        {
            return (ConversionOperatorDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        protected override int GetParameterCountFromSyntax()
        {
            return GetSyntax().ParameterList.ParameterCount;
        }

        internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(GetSyntax().AttributeLists);
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            ConversionOperatorDeclarationSyntax syntax = GetSyntax();
            return MakeParametersAndBindReturnType(syntax, syntax.Type, diagnostics);
        }
    }
}
