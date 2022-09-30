using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class UsingStatementBinder : LockOrUsingBinder
    {
        private readonly UsingStatementSyntax _syntax;

        protected override ExpressionSyntax TargetExpressionSyntax => _syntax.Expression;

        internal override SyntaxNode ScopeDesignator => _syntax;

        public UsingStatementBinder(Binder enclosing, UsingStatementSyntax syntax)
            : base(enclosing)
        {
            _syntax = syntax;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            ExpressionSyntax targetExpressionSyntax = TargetExpressionSyntax;
            VariableDeclarationSyntax declaration = _syntax.Declaration;
            if (targetExpressionSyntax != null)
            {
                ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
                ExpressionVariableFinder.FindExpressionVariables(this, instance, targetExpressionSyntax);
                return instance.ToImmutableAndFree();
            }
            ArrayBuilder<LocalSymbol> instance2 = ArrayBuilder<LocalSymbol>.GetInstance(declaration.Variables.Count);
            TypeSyntax type = declaration.Type;
            Action<ArrayRankSpecifierSyntax, (UsingStatementBinder, ArrayBuilder<LocalSymbol>)> action = delegate (ArrayRankSpecifierSyntax rankSpecifier, (UsingStatementBinder binder, ArrayBuilder<LocalSymbol> locals) args)
            {
                SeparatedSyntaxList<ExpressionSyntax>.Enumerator enumerator2 = rankSpecifier.Sizes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ExpressionSyntax current2 = enumerator2.Current;
                    if (current2.Kind() != SyntaxKind.OmittedArraySizeExpression)
                    {
                        ExpressionVariableFinder.FindExpressionVariables(args.binder, args.locals, current2);
                    }
                }
            };
            (UsingStatementBinder, ArrayBuilder<LocalSymbol>) argument = (this, instance2);
            type.VisitRankSpecifiers(action, in argument);
            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = declaration.Variables.GetEnumerator();
            while (enumerator.MoveNext())
            {
                VariableDeclaratorSyntax current = enumerator.Current;
                instance2.Add(MakeLocal(declaration, current, LocalDeclarationKind.UsingVariable));
                ExpressionVariableFinder.FindExpressionVariables(this, instance2, current);
            }
            return instance2.ToImmutableAndFree();
        }

        internal override BoundStatement BindUsingStatementParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            object obj = TargetExpressionSyntax;
            VariableDeclarationSyntax declaration = _syntax.Declaration;
            _syntax.AwaitKeyword.Kind();
            if (obj == null)
            {
                obj = declaration;
            }
            return BindUsingStatementOrDeclarationFromParts((SyntaxNode)obj, _syntax.UsingKeyword, _syntax.AwaitKeyword, originalBinder, this, diagnostics);
        }

        internal static BoundStatement BindUsingStatementOrDeclarationFromParts(SyntaxNode syntax, SyntaxToken usingKeyword, SyntaxToken awaitKeyword, Binder originalBinder, UsingStatementBinder? usingBinderOpt, BindingDiagnosticBag diagnostics)
        {
            Binder originalBinder2 = originalBinder;
            BindingDiagnosticBag diagnostics2 = diagnostics;
            SyntaxNode syntax2 = syntax;
            UsingStatementBinder usingBinderOpt2 = usingBinderOpt;
            bool flag = syntax2.Kind() == SyntaxKind.LocalDeclarationStatement;
            bool num = !flag && syntax2.Kind() != SyntaxKind.VariableDeclaration;
            bool hasAwait = awaitKeyword != default(SyntaxToken);
            TypeSymbol disposableInterface = getDisposableInterface(hasAwait);
            bool hasErrors = Binder.ReportUseSite(disposableInterface, diagnostics2, hasAwait ? awaitKeyword : usingKeyword);
            ImmutableArray<BoundLocalDeclaration> declarationsOpt = default(ImmutableArray<BoundLocalDeclaration>);
            BoundMultipleLocalDeclarations declarationsOpt2 = null;
            BoundExpression expressionOpt = null;
            TypeSymbol declarationTypeOpt = null;
            Conversion iDisposableConversion2;
            MethodArgumentInfo patternDisposeInfo2;
            TypeSymbol awaitableType2;
            if (num)
            {
                expressionOpt = usingBinderOpt2.BindTargetExpression(diagnostics2, originalBinder2);
                hasErrors |= !populateDisposableConversionOrDisposeMethod(fromExpression: true, out iDisposableConversion2, out patternDisposeInfo2, out awaitableType2);
            }
            else
            {
                VariableDeclarationSyntax variableDeclarationSyntax = (flag ? ((LocalDeclarationStatementSyntax)syntax2).Declaration : ((VariableDeclarationSyntax)syntax2));
                originalBinder2.BindForOrUsingOrFixedDeclarations(variableDeclarationSyntax, LocalDeclarationKind.UsingVariable, diagnostics2, out declarationsOpt);
                declarationsOpt2 = new BoundMultipleLocalDeclarations(variableDeclarationSyntax, declarationsOpt);
                declarationTypeOpt = declarationsOpt[0].DeclaredTypeOpt!.Type;
                if (declarationTypeOpt.IsDynamic())
                {
                    iDisposableConversion2 = Conversion.ImplicitDynamic;
                    patternDisposeInfo2 = null;
                    awaitableType2 = null;
                }
                else
                {
                    hasErrors |= !populateDisposableConversionOrDisposeMethod(fromExpression: false, out iDisposableConversion2, out patternDisposeInfo2, out awaitableType2);
                }
            }
            BoundAwaitableInfo awaitOpt = null;
            if (hasAwait)
            {
                originalBinder2.ReportBadAwaitDiagnostics(syntax2, awaitKeyword.GetLocation(), diagnostics2, ref hasErrors);
                if ((object)awaitableType2 == null)
                {
                    awaitOpt = new BoundAwaitableInfo(syntax2, null, isDynamic: true, null, null, null)
                    {
                        WasCompilerGenerated = true
                    };
                }
                else
                {
                    hasErrors |= Binder.ReportUseSite(awaitableType2, diagnostics2, awaitKeyword);
                    BoundAwaitableValuePlaceholder placeholder = new BoundAwaitableValuePlaceholder(syntax2, originalBinder2.LocalScopeDepth, awaitableType2).MakeCompilerGenerated();
                    awaitOpt = originalBinder2.BindAwaitInfo(placeholder, syntax2, diagnostics2, ref hasErrors);
                }
            }
            if (flag)
            {
                return new BoundUsingLocalDeclarations(syntax2, patternDisposeInfo2, iDisposableConversion2, awaitOpt, declarationsOpt, hasErrors);
            }
            BoundStatement body = originalBinder2.BindPossibleEmbeddedStatement(usingBinderOpt2._syntax.Statement, diagnostics2);
            return new BoundUsingStatement(usingBinderOpt2._syntax, usingBinderOpt2.Locals, declarationsOpt2, expressionOpt, iDisposableConversion2, body, awaitOpt, patternDisposeInfo2, hasErrors);
            Conversion classifyConversion(bool fromExpression, TypeSymbol targetInterface, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
            {
                if (!fromExpression)
                {
                    return originalBinder2.Conversions.ClassifyImplicitConversionFromType(declarationTypeOpt, targetInterface, ref useSiteInfo);
                }
                return originalBinder2.Conversions.ClassifyImplicitConversionFromExpression(expressionOpt, targetInterface, ref useSiteInfo);
            }
            TypeSymbol getDisposableInterface(bool isAsync)
            {
                if (!isAsync)
                {
                    return originalBinder2.Compilation.GetSpecialType(SpecialType.System_IDisposable);
                }
                return originalBinder2.Compilation.GetWellKnownType(WellKnownType.System_IAsyncDisposable);
            }
            bool populateDisposableConversionOrDisposeMethod(bool fromExpression, out Conversion iDisposableConversion, out MethodArgumentInfo? patternDisposeInfo, out TypeSymbol? awaitableType)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = originalBinder2.GetNewCompoundUseSiteInfo(diagnostics2);
                iDisposableConversion = classifyConversion(fromExpression, disposableInterface, ref useSiteInfo2);
                patternDisposeInfo = null;
                awaitableType = null;
                diagnostics2.Add(syntax2, useSiteInfo2);
                if (iDisposableConversion.IsImplicit)
                {
                    if (hasAwait)
                    {
                        awaitableType = originalBinder2.Compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_ValueTask);
                    }
                    return true;
                }
                TypeSymbol typeSymbol = (fromExpression ? expressionOpt.Type : declarationTypeOpt);
                if ((object)typeSymbol != null && (typeSymbol.IsRefLikeType || hasAwait))
                {
                    BoundExpression expr = (fromExpression ? expressionOpt : new BoundLocal(syntax2, declarationsOpt[0].LocalSymbol, null, typeSymbol)
                    {
                        WasCompilerGenerated = true
                    });
                    BindingDiagnosticBag diagnostics3 = (originalBinder2.Compilation.IsFeatureEnabled(MessageID.IDS_FeatureUsingDeclarations) ? diagnostics2 : BindingDiagnosticBag.Discarded);
                    MethodSymbol methodSymbol = originalBinder2.TryFindDisposePatternMethod(expr, syntax2, hasAwait, diagnostics3);
                    if ((object)methodSymbol != null)
                    {
                        MessageID.IDS_FeatureUsingDeclarations.CheckFeatureAvailability(diagnostics2, originalBinder2.Compilation, syntax2.Location);
                        ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(methodSymbol.ParameterCount);
                        ImmutableArray<int> argsToParamsOpt = default(ImmutableArray<int>);
                        bool expanded = methodSymbol.HasParamsParameter();
                        originalBinder2.BindDefaultArguments(usingBinderOpt2?._syntax ?? syntax2, methodSymbol.Parameters, instance, null, ref argsToParamsOpt, out var defaultArguments, expanded, enableCallerInfo: true, diagnostics3);
                        patternDisposeInfo = new MethodArgumentInfo(methodSymbol, instance.ToImmutableAndFree(), argsToParamsOpt, defaultArguments, expanded);
                        if (hasAwait)
                        {
                            awaitableType = methodSymbol.ReturnType;
                        }
                        return true;
                    }
                }
                if ((object)typeSymbol == null || !typeSymbol.IsErrorType())
                {
                    TypeSymbol disposableInterface2 = getDisposableInterface(!hasAwait);
                    CompoundUseSiteInfo<AssemblySymbol> useSiteInfo3 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                    ErrorCode code = ((!classifyConversion(fromExpression, disposableInterface2, ref useSiteInfo3).IsImplicit) ? (hasAwait ? ErrorCode.ERR_NoConvToIAsyncDisp : ErrorCode.ERR_NoConvToIDisp) : (hasAwait ? ErrorCode.ERR_NoConvToIAsyncDispWrongAsync : ErrorCode.ERR_NoConvToIDispWrongAsync));
                    Binder.Error(diagnostics2, code, syntax2, declarationTypeOpt ?? expressionOpt.Display);
                }
                return false;
            }
        }

        internal override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(SyntaxNode scopeDesignator)
        {
            if (_syntax == scopeDesignator)
            {
                return Locals;
            }
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<LocalFunctionSymbol> GetDeclaredLocalFunctionsForScope(CSharpSyntaxNode scopeDesignator)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
