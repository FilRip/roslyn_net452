using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class BuckStopsHereBinder : Binder
    {
        internal override ImportChain? ImportChain => null;

        internal override QuickAttributeChecker QuickAttributeChecker => Microsoft.CodeAnalysis.CSharp.Symbols.QuickAttributeChecker.Predefined;

        internal override uint LocalScopeDepth => 0u;

        protected override bool InExecutableBinder => false;

        internal override ConstantFieldsInProgress ConstantFieldsInProgress => Microsoft.CodeAnalysis.CSharp.ConstantFieldsInProgress.Empty;

        internal override ConsList<FieldSymbol> FieldsBeingBound => ConsList<FieldSymbol>.Empty;

        internal override LocalSymbol? LocalInProgress => null;

        internal override bool IsInMethodBody => false;

        internal override bool IsDirectlyInIterator => false;

        internal override bool IsIndirectlyInIterator => false;

        internal override GeneratedLabelSymbol? BreakLabel => null;

        internal override GeneratedLabelSymbol? ContinueLabel => null;

        internal override BoundExpression? ConditionalReceiverExpression => null;

        internal override Symbol? ContainingMemberOrLambda => null;

        internal override ImmutableHashSet<Symbol> LockedOrDisposedVariables => ImmutableHashSet.Create<Symbol>();

        internal BuckStopsHereBinder(CSharpCompilation compilation)
            : base(compilation)
        {
        }

        protected override SourceLocalSymbol? LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        protected override LocalFunctionSymbol? LookupLocalFunction(SyntaxToken nameToken)
        {
            return null;
        }

        internal override bool IsAccessibleHelper(Symbol symbol, TypeSymbol accessThroughType, out bool failedThroughTypeCheck, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ConsList<TypeSymbol> basesBeingResolved)
        {
            failedThroughTypeCheck = false;
            return Binder.IsSymbolAccessibleConditional(symbol, base.Compilation.Assembly, ref useSiteInfo);
        }

        protected override bool IsUnboundTypeAllowed(GenericNameSyntax syntax)
        {
            return false;
        }

        internal override TypeWithAnnotations GetIteratorElementType()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool AreNullableAnnotationsGloballyEnabled()
        {
            return GetGlobalAnnotationState();
        }

        internal override Binder? GetBinder(SyntaxNode node)
        {
            return null;
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundStatement BindSwitchStatementCore(SwitchStatementSyntax node, Binder originalBinder, BindingDiagnosticBag diagnostics)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundExpression BindSwitchExpressionCore(SwitchExpressionSyntax node, Binder originalBinder, BindingDiagnosticBag diagnostics)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override void BindPatternSwitchLabelForInference(CasePatternSwitchLabelSyntax node, BindingDiagnosticBag diagnostics)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundSwitchExpressionArm BindSwitchExpressionArm(SwitchExpressionArmSyntax node, TypeSymbol switchGoverningType, uint switchGoverningValEscape, BindingDiagnosticBag diagnostics)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundForStatement BindForParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundStatement BindForEachParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundStatement BindForEachDeconstruction(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundWhileStatement BindWhileParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundDoStatement BindDoParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundStatement BindUsingStatementParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override BoundStatement BindLockStatementParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
