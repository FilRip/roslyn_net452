using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceUserDefinedOperatorSymbol : SourceUserDefinedOperatorSymbolBase
    {
        protected override Location ReturnTypeLocation => GetSyntax().ReturnType.Location;

        internal override bool GenerateDebugInfo => true;

        public static SourceUserDefinedOperatorSymbol CreateUserDefinedOperatorSymbol(SourceMemberContainerTypeSymbol containingType, OperatorDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
        {
            Location location = syntax.OperatorToken.GetLocation();
            string name = OperatorFacts.OperatorNameFromDeclaration(syntax);
            return new SourceUserDefinedOperatorSymbol(containingType, name, location, syntax, isNullableAnalysisEnabled, diagnostics);
        }

        private SourceUserDefinedOperatorSymbol(SourceMemberContainerTypeSymbol containingType, string name, Location location, OperatorDeclarationSyntax syntax, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(MethodKind.UserDefinedOperator, name, containingType, location, syntax, SourceUserDefinedOperatorSymbolBase.MakeDeclarationModifiers(syntax, location, diagnostics), syntax.HasAnyBody(), syntax.Body == null && syntax.ExpressionBody != null, SyntaxFacts.HasYieldOperations(syntax.Body), isNullableAnalysisEnabled, diagnostics)
        {
            Symbol.CheckForBlockAndExpressionBody(syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
            if (name != "op_Equality" && name != "op_Inequality")
            {
                CheckFeatureAvailabilityAndRuntimeSupport(syntax, location, syntax.Body != null || syntax.ExpressionBody != null, diagnostics);
            }
        }

        internal OperatorDeclarationSyntax GetSyntax()
        {
            return (OperatorDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
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
            OperatorDeclarationSyntax syntax = GetSyntax();
            return MakeParametersAndBindReturnType(syntax, syntax.ReturnType, diagnostics);
        }
    }
}
