using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceCustomEventAccessorSymbol : SourceEventAccessorSymbol
    {
        public override Accessibility DeclaredAccessibility => AssociatedSymbol.DeclaredAccessibility;

        public override bool IsImplicitlyDeclared => false;

        internal override bool GenerateDebugInfo => true;

        internal override bool IsExpressionBodied
        {
            get
            {
                AccessorDeclarationSyntax syntax = GetSyntax();
                bool flag = syntax.Body != null;
                bool flag2 = syntax.ExpressionBody != null;
                return !flag && flag2;
            }
        }

        internal SourceCustomEventAccessorSymbol(SourceEventSymbol @event, AccessorDeclarationSyntax syntax, EventSymbol explicitlyImplementedEventOpt, string aliasQualifierOpt, bool isNullableAnalysisEnabled, BindingDiagnosticBag diagnostics)
            : base(@event, syntax.GetReference(), ImmutableArray.Create(syntax.Keyword.GetLocation()), explicitlyImplementedEventOpt, aliasQualifierOpt, syntax.Kind() == SyntaxKind.AddAccessorDeclaration, SyntaxFacts.HasYieldOperations(syntax.Body), isNullableAnalysisEnabled)
        {
            CheckFeatureAvailabilityAndRuntimeSupport(syntax, base.Location, hasBody: true, diagnostics);
            if ((syntax.Body != null || syntax.ExpressionBody != null) && IsExtern && !IsAbstract)
            {
                diagnostics.Add(ErrorCode.ERR_ExternHasBody, base.Location, this);
            }
            if (syntax.Modifiers.Count > 0)
            {
                diagnostics.Add(ErrorCode.ERR_NoModifiersOnAccessor, syntax.Modifiers[0].GetLocation());
            }
            Symbol.CheckForBlockAndExpressionBody(syntax.Body, syntax.ExpressionBody, syntax, diagnostics);
        }

        internal AccessorDeclarationSyntax GetSyntax()
        {
            return (AccessorDeclarationSyntax)syntaxReferenceOpt.GetSyntax();
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(GetSyntax().AttributeLists);
        }
    }
}
