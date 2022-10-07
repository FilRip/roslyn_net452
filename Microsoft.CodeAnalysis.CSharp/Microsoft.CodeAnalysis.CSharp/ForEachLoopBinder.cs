using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class ForEachLoopBinder : LoopBinder
    {
        private enum EnumeratorResult
        {
            Succeeded,
            FailedNotReported,
            FailedAndReported
        }

        private const string GetEnumeratorMethodName = "GetEnumerator";

        private const string CurrentPropertyName = "Current";

        private const string MoveNextMethodName = "MoveNext";

        private const string GetAsyncEnumeratorMethodName = "GetAsyncEnumerator";

        private const string MoveNextAsyncMethodName = "MoveNextAsync";

        private readonly CommonForEachStatementSyntax _syntax;

        private SourceLocalSymbol IterationVariable
        {
            get
            {
                if (_syntax.Kind() != SyntaxKind.ForEachStatement)
                {
                    return null;
                }
                return (SourceLocalSymbol)Locals[0];
            }
        }

        private bool IsAsync => _syntax.AwaitKeyword != default(SyntaxToken);

        internal override SyntaxNode ScopeDesignator => _syntax;

        public ForEachLoopBinder(Binder enclosing, CommonForEachStatementSyntax syntax)
            : base(enclosing)
        {
            _syntax = syntax;
        }

        protected override ImmutableArray<LocalSymbol> BuildLocals()
        {
            switch (_syntax.Kind())
            {
                case SyntaxKind.ForEachVariableStatement:
                    {
                        ForEachVariableStatementSyntax forEachVariableStatementSyntax = (ForEachVariableStatementSyntax)_syntax;
                        ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
                        CollectLocalsFromDeconstruction(forEachVariableStatementSyntax.Variable, LocalDeclarationKind.ForEachIterationVariable, instance, forEachVariableStatementSyntax);
                        return instance.ToImmutableAndFree();
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        ForEachStatementSyntax forEachStatementSyntax = (ForEachStatementSyntax)_syntax;
                        return ImmutableArray.Create((LocalSymbol)SourceLocalSymbol.MakeForeachLocal((MethodSymbol)ContainingMemberOrLambda, this, forEachStatementSyntax.Type, forEachStatementSyntax.Identifier, forEachStatementSyntax.Expression));
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(_syntax.Kind());
            }
        }

        internal void CollectLocalsFromDeconstruction(ExpressionSyntax declaration, LocalDeclarationKind kind, ArrayBuilder<LocalSymbol> locals, SyntaxNode deconstructionStatement, Binder enclosingBinderOpt = null)
        {
            switch (declaration.Kind())
            {
                case SyntaxKind.TupleExpression:
                    {
                        SeparatedSyntaxList<ArgumentSyntax>.Enumerator enumerator = ((TupleExpressionSyntax)declaration).Arguments.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ArgumentSyntax current = enumerator.Current;
                            CollectLocalsFromDeconstruction(current.Expression, kind, locals, deconstructionStatement, enclosingBinderOpt);
                        }
                        break;
                    }
                case SyntaxKind.DeclarationExpression:
                    {
                        DeclarationExpressionSyntax declarationExpressionSyntax = (DeclarationExpressionSyntax)declaration;
                        CollectLocalsFromDeconstruction(declarationExpressionSyntax.Designation, declarationExpressionSyntax.Type, kind, locals, deconstructionStatement, enclosingBinderOpt);
                        break;
                    }
                default:
                    ExpressionVariableFinder.FindExpressionVariables(this, locals, declaration);
                    break;
                case SyntaxKind.IdentifierName:
                    break;
            }
        }

        internal void CollectLocalsFromDeconstruction(VariableDesignationSyntax designation, TypeSyntax closestTypeSyntax, LocalDeclarationKind kind, ArrayBuilder<LocalSymbol> locals, SyntaxNode deconstructionStatement, Binder enclosingBinderOpt)
        {
            switch (designation.Kind())
            {
                case SyntaxKind.SingleVariableDesignation:
                    {
                        SingleVariableDesignationSyntax singleVariableDesignationSyntax = (SingleVariableDesignationSyntax)designation;
                        SourceLocalSymbol item = SourceLocalSymbol.MakeDeconstructionLocal(ContainingMemberOrLambda, this, enclosingBinderOpt ?? this, closestTypeSyntax, singleVariableDesignationSyntax.Identifier, kind, deconstructionStatement);
                        locals.Add(item);
                        break;
                    }
                case SyntaxKind.ParenthesizedVariableDesignation:
                    {
                        SeparatedSyntaxList<VariableDesignationSyntax>.Enumerator enumerator = ((ParenthesizedVariableDesignationSyntax)designation).Variables.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            VariableDesignationSyntax current = enumerator.Current;
                            CollectLocalsFromDeconstruction(current, closestTypeSyntax, kind, locals, deconstructionStatement, enclosingBinderOpt);
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(designation.Kind());
                case SyntaxKind.DiscardDesignation:
                    break;
            }
        }

        internal override BoundStatement BindForEachParts(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            return BindForEachPartsWorker(diagnostics, originalBinder);
        }

        internal override BoundStatement BindForEachDeconstruction(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            BoundExpression collectionExpr = originalBinder.GetBinder(_syntax.Expression)!.BindRValueWithoutTargetType(_syntax.Expression, diagnostics);
            ForEachEnumeratorInfo.Builder builder = default(ForEachEnumeratorInfo.Builder);
            GetEnumeratorInfoAndInferCollectionElementType(ref builder, ref collectionExpr, diagnostics, out var inferredType);
            ExpressionSyntax variable = ((ForEachVariableStatementSyntax)_syntax).Variable;
            BoundDeconstructValuePlaceholder rightPlaceholder = new BoundDeconstructValuePlaceholder(_syntax.Expression, LocalScopeDepth, inferredType.Type ?? CreateErrorType("var"));
            DeclarationExpressionSyntax declaration = null;
            ExpressionSyntax expression = null;
            BoundDeconstructionAssignmentOperator expression2 = BindDeconstruction(variable, variable, _syntax.Expression, diagnostics, ref declaration, ref expression, resultIsUsedOverride: false, rightPlaceholder);
            return new BoundExpressionStatement(_syntax, expression2);
        }

        private BoundForEachStatement BindForEachPartsWorker(BindingDiagnosticBag diagnostics, Binder originalBinder)
        {
            BoundExpression collectionExpr = originalBinder.GetBinder(_syntax.Expression)!.BindRValueWithoutTargetType(_syntax.Expression, diagnostics);
            ForEachEnumeratorInfo.Builder builder = default(ForEachEnumeratorInfo.Builder);
            bool flag = !GetEnumeratorInfoAndInferCollectionElementType(ref builder, ref collectionExpr, diagnostics, out var inferredType);
            flag |= builder.IsIncomplete;
            BoundAwaitableInfo boundAwaitableInfo = null;
            MethodSymbol methodSymbol = builder.GetEnumeratorInfo?.Method;
            if (methodSymbol != null)
            {
                originalBinder.CheckImplicitThisCopyInReadOnlyMember(collectionExpr, methodSymbol, diagnostics);
                if (methodSymbol.IsExtensionMethod && !flag)
                {
                    MessageID feature = (IsAsync ? MessageID.IDS_FeatureExtensionGetAsyncEnumerator : MessageID.IDS_FeatureExtensionGetEnumerator);
                    flag |= !feature.CheckFeatureAvailability(diagnostics, base.Compilation, collectionExpr.Syntax.Location);
                    ImmutableArray<RefKind> parameterRefKinds = methodSymbol.ParameterRefKinds;
                    if (!parameterRefKinds.IsDefault && parameterRefKinds[0] == RefKind.Ref)
                    {
                        Binder.Error(diagnostics, ErrorCode.ERR_RefLvalueExpected, collectionExpr.Syntax);
                        flag = true;
                    }
                }
            }
            if (IsAsync)
            {
                ExpressionSyntax expression = _syntax.Expression;
                ReportBadAwaitDiagnostics(expression, _syntax.AwaitKeyword.GetLocation(), diagnostics, ref flag);
                BoundAwaitableValuePlaceholder placeholder = new BoundAwaitableValuePlaceholder(expression, LocalScopeDepth, builder.MoveNextInfo?.Method.ReturnType ?? CreateErrorType());
                boundAwaitableInfo = BindAwaitInfo(placeholder, expression, diagnostics, ref flag);
                if (!flag)
                {
                    MethodSymbol? getResult = boundAwaitableInfo.GetResult;
                    if ((object)getResult == null || getResult!.ReturnType.SpecialType != SpecialType.System_Boolean)
                    {
                        diagnostics.Add(ErrorCode.ERR_BadGetAsyncEnumerator, expression.Location, methodSymbol.ReturnTypeWithAnnotations, methodSymbol);
                        flag = true;
                    }
                }
            }
            bool flag2 = false;
            BoundForEachDeconstructStep deconstructionOpt = null;
            BoundExpression boundExpression = null;
            uint valEscape = Binder.GetValEscape(collectionExpr, LocalScopeDepth);
            TypeWithAnnotations typeWithAnnotations;
            BoundTypeExpression boundTypeExpression;
            switch (_syntax.Kind())
            {
                case SyntaxKind.ForEachStatement:
                    {
                        ForEachStatementSyntax obj = (ForEachStatementSyntax)_syntax;
                        flag2 = originalBinder.ValidateDeclarationNameConflictsInScope(IterationVariable, diagnostics);
                        TypeSyntax syntax = obj.Type.SkipRef(out RefKind refKind);
                        TypeWithAnnotations typeWithAnnotations2 = BindTypeOrVarKeyword(syntax, diagnostics, out bool isVar, out AliasSymbol alias);
                        if (isVar)
                        {
                            typeWithAnnotations2 = (inferredType.HasType ? inferredType : TypeWithAnnotations.Create(CreateErrorType("var")));
                        }
                        typeWithAnnotations = typeWithAnnotations2;
                        boundTypeExpression = new BoundTypeExpression(syntax, alias, typeWithAnnotations);
                        SourceLocalSymbol iterationVariable = IterationVariable;
                        iterationVariable.SetTypeWithAnnotations(typeWithAnnotations2);
                        iterationVariable.SetValEscape(valEscape);
                        if (iterationVariable.RefKind != 0)
                        {
                            iterationVariable.SetRefEscape(valEscape);
                            if (CheckRefLocalInAsyncOrIteratorMethod(iterationVariable.IdentifierToken, diagnostics))
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            BindValueKind valueKind = iterationVariable.RefKind switch
                            {
                                RefKind.None => BindValueKind.RValue,
                                RefKind.Ref => BindValueKind.Assignable | BindValueKind.RefersToLocation,
                                RefKind.In => BindValueKind.RefersToLocation,
                                _ => throw ExceptionUtilities.UnexpectedValue(iterationVariable.RefKind),
                            };
                            flag |= !CheckMethodReturnValueKind(builder.CurrentPropertyGetter, null, collectionExpr.Syntax, valueKind, checkingReceiver: false, diagnostics);
                        }
                        break;
                    }
                case SyntaxKind.ForEachVariableStatement:
                    {
                        ForEachVariableStatementSyntax forEachVariableStatementSyntax = (ForEachVariableStatementSyntax)_syntax;
                        typeWithAnnotations = (inferredType.HasType ? inferredType : TypeWithAnnotations.Create(CreateErrorType("var")));
                        ExpressionSyntax variable = forEachVariableStatementSyntax.Variable;
                        if (variable.IsDeconstructionLeft())
                        {
                            BoundDeconstructValuePlaceholder boundDeconstructValuePlaceholder = new BoundDeconstructValuePlaceholder(_syntax.Expression, valEscape, typeWithAnnotations.Type).MakeCompilerGenerated();
                            DeclarationExpressionSyntax declaration = null;
                            ExpressionSyntax expression2 = null;
                            BoundDeconstructionAssignmentOperator deconstructionAssignment = BindDeconstruction(variable, variable, _syntax.Expression, diagnostics, ref declaration, ref expression2, resultIsUsedOverride: false, boundDeconstructValuePlaceholder);
                            if (expression2 != null)
                            {
                                Binder.Error(diagnostics, ErrorCode.ERR_MustDeclareForeachIteration, variable);
                                flag = true;
                            }
                            deconstructionOpt = new BoundForEachDeconstructStep(variable, deconstructionAssignment, boundDeconstructValuePlaceholder).MakeCompilerGenerated();
                        }
                        else
                        {
                            boundExpression = BindExpression(forEachVariableStatementSyntax.Variable, BindingDiagnosticBag.Discarded);
                            if (boundExpression.Kind == BoundKind.DiscardExpression)
                            {
                                boundExpression = ((BoundDiscardExpression)boundExpression).FailInference(this, null);
                            }
                            flag = true;
                            if (!forEachVariableStatementSyntax.HasErrors)
                            {
                                Binder.Error(diagnostics, ErrorCode.ERR_MustDeclareForeachIteration, variable);
                            }
                        }
                        boundTypeExpression = new BoundTypeExpression(variable, null, typeWithAnnotations).MakeCompilerGenerated();
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(_syntax.Kind());
            }
            BoundStatement body = originalBinder.BindPossibleEmbeddedStatement(_syntax.Statement, diagnostics);
            ImmutableArray<LocalSymbol> locals = Locals;
            flag = flag || boundTypeExpression.HasErrors || typeWithAnnotations.Type.IsErrorType();
            if (flag)
            {
                return new BoundForEachStatement(_syntax, null, default(Conversion), boundTypeExpression, locals, boundExpression, collectionExpr, deconstructionOpt, boundAwaitableInfo, body, base.CheckOverflowAtRuntime, BreakLabel, ContinueLabel, flag);
            }
            flag = flag || flag2;
            SyntaxToken forEachKeyword = _syntax.ForEachKeyword;
            ReportDiagnosticsIfObsolete(diagnostics, methodSymbol, forEachKeyword, hasBaseReceiver: false);
            Binder.ReportDiagnosticsIfUnmanagedCallersOnly(diagnostics, methodSymbol, forEachKeyword.GetLocation(), isDelegateConversion: false);
            ReportDiagnosticsIfObsolete(diagnostics, builder.MoveNextInfo!.Method, forEachKeyword, hasBaseReceiver: false);
            ReportDiagnosticsIfObsolete(diagnostics, builder.CurrentPropertyGetter, forEachKeyword, hasBaseReceiver: false);
            ReportDiagnosticsIfObsolete(diagnostics, builder.CurrentPropertyGetter.AssociatedSymbol, forEachKeyword, hasBaseReceiver: false);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            Conversion conversion = base.Conversions.ClassifyConversionFromType(inferredType.Type, typeWithAnnotations.Type, ref useSiteInfo, forCast: true);
            if (!conversion.IsValid)
            {
                ImmutableArray<MethodSymbol> originalUserDefinedConversions = conversion.OriginalUserDefinedConversions;
                if (originalUserDefinedConversions.Length > 1)
                {
                    diagnostics.Add(ErrorCode.ERR_AmbigUDConv, forEachKeyword.GetLocation(), originalUserDefinedConversions[0], originalUserDefinedConversions[1], inferredType.Type, typeWithAnnotations);
                }
                else
                {
                    SymbolDistinguisher symbolDistinguisher = new SymbolDistinguisher(base.Compilation, inferredType.Type, typeWithAnnotations.Type);
                    diagnostics.Add(ErrorCode.ERR_NoExplicitConv, forEachKeyword.GetLocation(), symbolDistinguisher.First, symbolDistinguisher.Second);
                }
                flag = true;
            }
            else
            {
                ReportDiagnosticsIfObsolete(diagnostics, conversion, _syntax.ForEachKeyword, hasBaseReceiver: false);
            }
            builder.CollectionConversion = base.Conversions.ClassifyConversionFromExpression(collectionExpr, builder.CollectionType, ref useSiteInfo);
            builder.CurrentConversion = base.Conversions.ClassifyConversionFromType(builder.CurrentPropertyGetter.ReturnType, builder.ElementType, ref useSiteInfo);
            TypeSymbol returnType = methodSymbol.ReturnType;
            builder.EnumeratorConversion = (returnType.IsValueType ? Conversion.Identity : base.Conversions.ClassifyConversionFromType(returnType, GetSpecialType(SpecialType.System_Object, diagnostics, _syntax), ref useSiteInfo));
            if (returnType.IsRestrictedType() && (IsDirectlyInIterator || IsInAsyncMethod()))
            {
                diagnostics.Add(ErrorCode.ERR_BadSpecialByRefIterator, forEachKeyword.GetLocation(), returnType);
            }
            diagnostics.Add(_syntax.ForEachKeyword.GetLocation(), useSiteInfo);
            BoundConversion expression3 = new BoundConversion(collectionExpr.Syntax, collectionExpr, builder.CollectionConversion, base.CheckOverflowAtRuntime, explicitCastInCode: false, null, null, builder.CollectionType);
            if (builder.NeedsDisposal && IsAsync)
            {
                flag |= GetAwaitDisposeAsyncInfo(ref builder, diagnostics);
            }
            return new BoundForEachStatement(_syntax, builder.Build(Flags), conversion, boundTypeExpression, locals, boundExpression, expression3, deconstructionOpt, boundAwaitableInfo, body, base.CheckOverflowAtRuntime, BreakLabel, ContinueLabel, flag);
        }

        private bool GetAwaitDisposeAsyncInfo(ref ForEachEnumeratorInfo.Builder builder, BindingDiagnosticBag diagnostics)
        {
            TypeSymbol type = (((object)builder.PatternDisposeInfo == null) ? GetWellKnownType(WellKnownType.System_Threading_Tasks_ValueTask, diagnostics, _syntax) : builder.PatternDisposeInfo!.Method.ReturnType);
            bool hasErrors = false;
            ExpressionSyntax expression = _syntax.Expression;
            ReportBadAwaitDiagnostics(expression, _syntax.AwaitKeyword.GetLocation(), diagnostics, ref hasErrors);
            BoundAwaitableValuePlaceholder placeholder = new BoundAwaitableValuePlaceholder(expression, LocalScopeDepth, type);
            builder.DisposeAwaitableInfo = BindAwaitInfo(placeholder, expression, diagnostics, ref hasErrors);
            return hasErrors;
        }

        internal TypeWithAnnotations InferCollectionElementType(BindingDiagnosticBag diagnostics, ExpressionSyntax collectionSyntax)
        {
            BoundExpression collectionExpr = GetBinder(collectionSyntax)!.BindValue(collectionSyntax, diagnostics, BindValueKind.RValue);
            ForEachEnumeratorInfo.Builder builder = default(ForEachEnumeratorInfo.Builder);
            GetEnumeratorInfoAndInferCollectionElementType(ref builder, ref collectionExpr, diagnostics, out var inferredType);
            return inferredType;
        }

        private bool GetEnumeratorInfoAndInferCollectionElementType(ref ForEachEnumeratorInfo.Builder builder, ref BoundExpression collectionExpr, BindingDiagnosticBag diagnostics, out TypeWithAnnotations inferredType)
        {
            bool enumeratorInfo = GetEnumeratorInfo(ref builder, ref collectionExpr, diagnostics);
            if (!enumeratorInfo)
            {
                inferredType = default(TypeWithAnnotations);
                return enumeratorInfo;
            }
            if (collectionExpr.HasDynamicType())
            {
                inferredType = TypeWithAnnotations.Create(DynamicTypeSymbol.Instance);
                return enumeratorInfo;
            }
            if (collectionExpr.Type!.SpecialType == SpecialType.System_String && builder.CollectionType.SpecialType == SpecialType.System_Collections_IEnumerable)
            {
                inferredType = TypeWithAnnotations.Create(GetSpecialType(SpecialType.System_Char, diagnostics, collectionExpr.Syntax));
                return enumeratorInfo;
            }
            inferredType = builder.ElementTypeWithAnnotations;
            return enumeratorInfo;
        }

        private BoundExpression UnwrapCollectionExpressionIfNullable(BoundExpression collectionExpr, BindingDiagnosticBag diagnostics)
        {
            TypeSymbol type = collectionExpr.Type;
            if ((object)type != null && type.IsNullableType())
            {
                SyntaxNode syntax = collectionExpr.Syntax;
                MethodSymbol methodSymbol = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Nullable_T_get_Value, diagnostics, syntax);
                if ((object)methodSymbol != null)
                {
                    methodSymbol = methodSymbol.AsMember((NamedTypeSymbol)type);
                    return BoundCall.Synthesized(syntax, collectionExpr, methodSymbol);
                }
                return new BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(collectionExpr), type.GetNullableUnderlyingType())
                {
                    WasCompilerGenerated = true
                };
            }
            return collectionExpr;
        }

        private bool GetEnumeratorInfo(ref ForEachEnumeratorInfo.Builder builder, ref BoundExpression collectionExpr, BindingDiagnosticBag diagnostics)
        {
            bool flag = (builder.IsAsync = IsAsync);
            switch (GetEnumeratorInfo(ref builder, ref collectionExpr, flag, diagnostics))
            {
                case EnumeratorResult.Succeeded:
                    return true;
                case EnumeratorResult.FailedAndReported:
                    return false;
                default:
                    {
                        TypeSymbol type = collectionExpr.Type;
                        if (string.IsNullOrEmpty(type.Name) && collectionExpr.HasErrors)
                        {
                            return false;
                        }
                        if (type.IsErrorType())
                        {
                            return false;
                        }
                        ForEachEnumeratorInfo.Builder builder2 = default(ForEachEnumeratorInfo.Builder);
                        ErrorCode code = ((GetEnumeratorInfo(ref builder2, ref collectionExpr, !flag, BindingDiagnosticBag.Discarded) != 0) ? (flag ? ErrorCode.ERR_AwaitForEachMissingMember : ErrorCode.ERR_ForEachMissingMember) : (flag ? ErrorCode.ERR_AwaitForEachMissingMemberWrongAsync : ErrorCode.ERR_ForEachMissingMemberWrongAsync));
                        diagnostics.Add(code, _syntax.Expression.Location, type, flag ? "GetAsyncEnumerator" : "GetEnumerator");
                        return false;
                    }
            }
        }

        private EnumeratorResult GetEnumeratorInfo(ref ForEachEnumeratorInfo.Builder builder, ref BoundExpression collectionExpr, bool isAsync, BindingDiagnosticBag diagnostics)
        {
            TypeSymbol type = collectionExpr.Type;
            if ((object)type == null)
            {
                if (!ReportConstantNullCollectionExpr(collectionExpr, diagnostics))
                {
                    diagnostics.Add(ErrorCode.ERR_AnonMethGrpInForEach, _syntax.Expression.Location, collectionExpr.Display);
                }
                return EnumeratorResult.FailedAndReported;
            }
            if (collectionExpr.ResultKind == LookupResultKind.NotAValue)
            {
                return EnumeratorResult.FailedAndReported;
            }
            if (type.Kind == SymbolKind.DynamicType && IsAsync)
            {
                diagnostics.Add(ErrorCode.ERR_BadDynamicAwaitForEach, _syntax.Expression.Location);
                return EnumeratorResult.FailedAndReported;
            }
            if (type.Kind == SymbolKind.ArrayType || type.Kind == SymbolKind.DynamicType)
            {
                if (ReportConstantNullCollectionExpr(collectionExpr, diagnostics))
                {
                    return EnumeratorResult.FailedAndReported;
                }
                builder = GetDefaultEnumeratorInfo(builder, diagnostics, type);
                return EnumeratorResult.Succeeded;
            }
            BoundExpression boundExpression = UnwrapCollectionExpressionIfNullable(collectionExpr, diagnostics);
            TypeSymbol type2 = boundExpression.Type;
            if (SatisfiesGetEnumeratorPattern(ref builder, boundExpression, isAsync, viaExtensionMethod: false, diagnostics))
            {
                collectionExpr = boundExpression;
                if (ReportConstantNullCollectionExpr(collectionExpr, diagnostics))
                {
                    return EnumeratorResult.FailedAndReported;
                }
                return createPatternBasedEnumeratorResult(ref builder, boundExpression, isAsync, viaExtensionMethod: false, diagnostics);
            }
            if (!isAsync && IsIEnumerable(type2))
            {
                collectionExpr = boundExpression;
                diagnostics.Add(ErrorCode.ERR_ForEachMissingMember, _syntax.Expression.Location, type2, "GetEnumerator");
                return EnumeratorResult.FailedAndReported;
            }
            if (isAsync && IsIAsyncEnumerable(type2))
            {
                collectionExpr = boundExpression;
                diagnostics.Add(ErrorCode.ERR_AwaitForEachMissingMember, _syntax.Expression.Location, type2, "GetAsyncEnumerator");
                return EnumeratorResult.FailedAndReported;
            }
            EnumeratorResult enumeratorResult = SatisfiesIEnumerableInterfaces(ref builder, boundExpression, isAsync, diagnostics, type2);
            if (enumeratorResult != EnumeratorResult.FailedNotReported)
            {
                collectionExpr = boundExpression;
                return enumeratorResult;
            }
            if (!isAsync && type.SpecialType == SpecialType.System_String)
            {
                if (ReportConstantNullCollectionExpr(collectionExpr, diagnostics))
                {
                    return EnumeratorResult.FailedAndReported;
                }
                builder = GetDefaultEnumeratorInfo(builder, diagnostics, type);
                return EnumeratorResult.Succeeded;
            }
            if (SatisfiesGetEnumeratorPattern(ref builder, collectionExpr, isAsync, viaExtensionMethod: true, diagnostics))
            {
                return createPatternBasedEnumeratorResult(ref builder, collectionExpr, isAsync, viaExtensionMethod: true, diagnostics);
            }
            return EnumeratorResult.FailedNotReported;
            EnumeratorResult createPatternBasedEnumeratorResult(ref ForEachEnumeratorInfo.Builder builder, BoundExpression collectionExpr, bool isAsync, bool viaExtensionMethod, BindingDiagnosticBag diagnostics)
            {
                builder.CollectionType = (viaExtensionMethod ? builder.GetEnumeratorInfo!.Method.Parameters[0].Type : collectionExpr.Type);
                if (SatisfiesForEachPattern(ref builder, isAsync, diagnostics))
                {
                    builder.ElementTypeWithAnnotations = ((PropertySymbol)builder.CurrentPropertyGetter.AssociatedSymbol).TypeWithAnnotations;
                    GetDisposalInfoForEnumerator(ref builder, collectionExpr, isAsync, diagnostics);
                    return EnumeratorResult.Succeeded;
                }
                MethodSymbol method = builder.GetEnumeratorInfo!.Method;
                diagnostics.Add(isAsync ? ErrorCode.ERR_BadGetAsyncEnumerator : ErrorCode.ERR_BadGetEnumerator, _syntax.Expression.Location, method.ReturnType, method);
                return EnumeratorResult.FailedAndReported;
            }
        }

        private EnumeratorResult SatisfiesIEnumerableInterfaces(ref ForEachEnumeratorInfo.Builder builder, BoundExpression collectionExpr, bool isAsync, BindingDiagnosticBag diagnostics, TypeSymbol unwrappedCollectionExprType)
        {
            if (!AllInterfacesContainsIEnumerable(ref builder, unwrappedCollectionExprType, isAsync, diagnostics, out var foundMultiple))
            {
                return EnumeratorResult.FailedNotReported;
            }
            if (ReportConstantNullCollectionExpr(collectionExpr, diagnostics))
            {
                return EnumeratorResult.FailedAndReported;
            }
            CSharpSyntaxNode expression = _syntax.Expression;
            if (foundMultiple)
            {
                diagnostics.Add(isAsync ? ErrorCode.ERR_MultipleIAsyncEnumOfT : ErrorCode.ERR_MultipleIEnumOfT, expression.Location, unwrappedCollectionExprType, isAsync ? base.Compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T) : base.Compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T));
                return EnumeratorResult.FailedAndReported;
            }
            NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)builder.CollectionType;
            if (namedTypeSymbol.IsGenericType)
            {
                builder.ElementTypeWithAnnotations = namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.Single();
                MethodSymbol methodSymbol;
                if (isAsync)
                {
                    methodSymbol = (MethodSymbol)Binder.GetWellKnownTypeMember(base.Compilation, WellKnownMember.System_Collections_Generic_IAsyncEnumerable_T__GetAsyncEnumerator, diagnostics, expression.Location);
                    if ((object)methodSymbol != null && !methodSymbol.Parameters[0].IsOptional)
                    {
                        diagnostics.Add(ErrorCode.ERR_AwaitForEachMissingMember, _syntax.Expression.Location, unwrappedCollectionExprType, "GetAsyncEnumerator");
                        return EnumeratorResult.FailedAndReported;
                    }
                }
                else
                {
                    methodSymbol = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Collections_Generic_IEnumerable_T__GetEnumerator, diagnostics, expression);
                }
                MethodSymbol methodSymbol2 = null;
                if ((object)methodSymbol != null)
                {
                    MethodSymbol methodSymbol3 = methodSymbol.AsMember(namedTypeSymbol);
                    TypeSymbol returnType = methodSymbol3.ReturnType;
                    builder.GetEnumeratorInfo = BindDefaultArguments(methodSymbol3, null, expanded: false, collectionExpr.Syntax, diagnostics, assertMissingParametersAreOptional: false);
                    MethodSymbol methodSymbol5;
                    if (isAsync)
                    {
                        MethodSymbol methodSymbol4 = (MethodSymbol)GetWellKnownTypeMember(WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__MoveNextAsync, diagnostics, expression.Location);
                        if ((object)methodSymbol4 != null)
                        {
                            methodSymbol2 = methodSymbol4.AsMember((NamedTypeSymbol)returnType);
                        }
                        methodSymbol5 = (MethodSymbol)Binder.GetWellKnownTypeMember(base.Compilation, WellKnownMember.System_Collections_Generic_IAsyncEnumerator_T__get_Current, diagnostics, expression.Location);
                    }
                    else
                    {
                        methodSymbol5 = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Collections_Generic_IEnumerator_T__get_Current, diagnostics, expression);
                    }
                    if ((object)methodSymbol5 != null)
                    {
                        builder.CurrentPropertyGetter = methodSymbol5.AsMember((NamedTypeSymbol)returnType);
                    }
                }
                if (!isAsync)
                {
                    methodSymbol2 = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Collections_IEnumerator__MoveNext, diagnostics, expression);
                }
                if ((object)methodSymbol2 != null)
                {
                    builder.MoveNextInfo = MethodArgumentInfo.CreateParameterlessMethod(methodSymbol2);
                }
            }
            else
            {
                builder.GetEnumeratorInfo = GetParameterlessSpecialTypeMemberInfo(SpecialMember.System_Collections_IEnumerable__GetEnumerator, expression, diagnostics);
                builder.CurrentPropertyGetter = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Collections_IEnumerator__get_Current, diagnostics, expression);
                builder.MoveNextInfo = GetParameterlessSpecialTypeMemberInfo(SpecialMember.System_Collections_IEnumerator__MoveNext, expression, diagnostics);
                builder.ElementTypeWithAnnotations = builder.CurrentPropertyGetter?.ReturnTypeWithAnnotations ?? TypeWithAnnotations.Create(GetSpecialType(SpecialType.System_Object, diagnostics, expression));
            }
            builder.NeedsDisposal = true;
            return EnumeratorResult.Succeeded;
        }

        private bool ReportConstantNullCollectionExpr(BoundExpression collectionExpr, BindingDiagnosticBag diagnostics)
        {
            ConstantValue constantValue = collectionExpr.ConstantValue;
            if ((object)constantValue != null && constantValue.IsNull)
            {
                diagnostics.Add(ErrorCode.ERR_NullNotValid, _syntax.Expression.Location);
                return true;
            }
            return false;
        }

        private void GetDisposalInfoForEnumerator(ref ForEachEnumeratorInfo.Builder builder, BoundExpression expr, bool isAsync, BindingDiagnosticBag diagnostics)
        {
            TypeSymbol returnType = builder.GetEnumeratorInfo!.Method.ReturnType;
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            if ((!returnType.IsSealed && !isAsync) || base.Conversions.ClassifyImplicitConversionFromType(returnType, isAsync ? base.Compilation.GetWellKnownType(WellKnownType.System_IAsyncDisposable) : base.Compilation.GetSpecialType(SpecialType.System_IDisposable), ref useSiteInfo).IsImplicit)
            {
                builder.NeedsDisposal = true;
            }
            else if (base.Compilation.IsFeatureEnabled(MessageID.IDS_FeatureUsingDeclarations) && (returnType.IsRefLikeType || isAsync))
            {
                BoundDisposableValuePlaceholder expr2 = new BoundDisposableValuePlaceholder(_syntax, returnType);
                MethodSymbol methodSymbol = TryFindDisposePatternMethod(expr2, _syntax, isAsync, BindingDiagnosticBag.Discarded);
                if ((object)methodSymbol != null)
                {
                    ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(methodSymbol.ParameterCount);
                    ImmutableArray<int> argsToParamsOpt = default(ImmutableArray<int>);
                    bool expanded = methodSymbol.HasParamsParameter();
                    BindDefaultArguments(_syntax, methodSymbol.Parameters, instance, null, ref argsToParamsOpt, out var defaultArguments, expanded, enableCallerInfo: true, diagnostics);
                    builder.NeedsDisposal = true;
                    builder.PatternDisposeInfo = new MethodArgumentInfo(methodSymbol, instance.ToImmutableAndFree(), argsToParamsOpt, defaultArguments, expanded);
                }
            }
            diagnostics.Add(_syntax, useSiteInfo);
        }

        private ForEachEnumeratorInfo.Builder GetDefaultEnumeratorInfo(ForEachEnumeratorInfo.Builder builder, BindingDiagnosticBag diagnostics, TypeSymbol collectionExprType)
        {
            builder.CollectionType = GetSpecialType(SpecialType.System_Collections_IEnumerable, diagnostics, _syntax);
            if (collectionExprType.IsDynamic())
            {
                ForEachStatementSyntax obj = _syntax as ForEachStatementSyntax;
                builder.ElementTypeWithAnnotations = TypeWithAnnotations.Create((obj != null && obj.Type.IsVar) ? DynamicTypeSymbol.Instance : GetSpecialType(SpecialType.System_Object, diagnostics, _syntax));
            }
            else
            {
                builder.ElementTypeWithAnnotations = ((collectionExprType.SpecialType == SpecialType.System_String) ? TypeWithAnnotations.Create(GetSpecialType(SpecialType.System_Char, diagnostics, _syntax)) : ((ArrayTypeSymbol)collectionExprType).ElementTypeWithAnnotations);
            }
            builder.GetEnumeratorInfo = GetParameterlessSpecialTypeMemberInfo(SpecialMember.System_Collections_IEnumerable__GetEnumerator, _syntax, diagnostics);
            builder.CurrentPropertyGetter = (MethodSymbol)GetSpecialTypeMember(SpecialMember.System_Collections_IEnumerator__get_Current, diagnostics, _syntax);
            builder.MoveNextInfo = GetParameterlessSpecialTypeMemberInfo(SpecialMember.System_Collections_IEnumerator__MoveNext, _syntax, diagnostics);
            builder.NeedsDisposal = true;
            return builder;
        }

        private bool SatisfiesGetEnumeratorPattern(ref ForEachEnumeratorInfo.Builder builder, BoundExpression collectionExpr, bool isAsync, bool viaExtensionMethod, BindingDiagnosticBag diagnostics)
        {
            string methodName = (isAsync ? "GetAsyncEnumerator" : "GetEnumerator");
            MethodArgumentInfo methodArgumentInfo;
            if (viaExtensionMethod)
            {
                methodArgumentInfo = FindForEachPatternMethodViaExtension(collectionExpr, methodName, diagnostics);
            }
            else
            {
                LookupResult instance = LookupResult.GetInstance();
                methodArgumentInfo = FindForEachPatternMethod(collectionExpr.Type, methodName, instance, warningsOnly: true, diagnostics, isAsync);
                instance.Free();
            }
            builder.GetEnumeratorInfo = methodArgumentInfo;
            return (object)methodArgumentInfo != null;
        }

        private MethodArgumentInfo FindForEachPatternMethod(TypeSymbol patternType, string methodName, LookupResult lookupResult, bool warningsOnly, BindingDiagnosticBag diagnostics, bool isAsync)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            LookupMembersInType(lookupResult, patternType, methodName, 0, null, LookupOptions.Default, this, diagnose: false, ref useSiteInfo);
            diagnostics.Add(_syntax.Expression, useSiteInfo);
            if (!lookupResult.IsMultiViable)
            {
                ReportPatternMemberLookupDiagnostics(lookupResult, patternType, methodName, warningsOnly, diagnostics);
                return null;
            }
            ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
            ArrayBuilder<Symbol>.Enumerator enumerator = lookupResult.Symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Method)
                {
                    instance.Free();
                    if (warningsOnly)
                    {
                        ReportEnumerableWarning(diagnostics, patternType, current);
                    }
                    return null;
                }
                if (((MethodSymbol)current).ParameterCount == 0 || isAsync)
                {
                    instance.Add((MethodSymbol)current);
                }
            }
            MethodArgumentInfo result = PerformForEachPatternOverloadResolution(patternType, instance, warningsOnly, diagnostics, isAsync);
            instance.Free();
            return result;
        }

        private MethodArgumentInfo PerformForEachPatternOverloadResolution(TypeSymbol patternType, ArrayBuilder<MethodSymbol> candidateMethods, bool warningsOnly, BindingDiagnosticBag diagnostics, bool isAsync)
        {
            AnalyzedArguments instance = AnalyzedArguments.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            OverloadResolutionResult<MethodSymbol> instance3 = OverloadResolutionResult<MethodSymbol>.GetInstance();
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            BoundImplicitReceiver receiver = new BoundImplicitReceiver(_syntax.Expression, patternType);
            OverloadResolution overloadResolution = base.OverloadResolution;
            CallingConventionInfo callingConventionInfo = default(CallingConventionInfo);
            overloadResolution.MethodInvocationOverloadResolution(candidateMethods, instance2, receiver, instance, instance3, ref useSiteInfo, isMethodGroupConversion: false, allowRefOmittedArguments: false, inferWithDynamic: false, allowUnexpandedForm: true, RefKind.None, null, isFunctionPointerResolution: false, in callingConventionInfo);
            diagnostics.Add(_syntax.Expression, useSiteInfo);
            MethodSymbol methodSymbol = null;
            MethodArgumentInfo result = null;
            if (instance3.Succeeded)
            {
                methodSymbol = instance3.ValidResult.Member;
                if (methodSymbol.IsStatic || methodSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    if (warningsOnly)
                    {
                        MessageID id = (isAsync ? MessageID.IDS_FeatureAsyncStreams : MessageID.IDS_Collection);
                        diagnostics.Add(ErrorCode.WRN_PatternNotPublicOrNotInstance, _syntax.Expression.Location, patternType, id.Localize(), methodSymbol);
                    }
                    methodSymbol = null;
                }
                else if (methodSymbol.CallsAreOmitted(_syntax.SyntaxTree))
                {
                    methodSymbol = null;
                }
                else
                {
                    ImmutableArray<int> argsToParamsOpt = instance3.ValidResult.Result.ArgsToParamsOpt;
                    bool expanded = instance3.ValidResult.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm;
                    BindDefaultArguments(_syntax, methodSymbol.Parameters, instance.Arguments, instance.RefKinds, ref argsToParamsOpt, out var defaultArguments, expanded, enableCallerInfo: true, diagnostics);
                    result = new MethodArgumentInfo(methodSymbol, instance.Arguments.ToImmutable(), argsToParamsOpt, defaultArguments, expanded);
                }
            }
            else
            {
                ImmutableArray<MethodSymbol> allApplicableMembers = instance3.GetAllApplicableMembers();
                if (allApplicableMembers.Length > 1 && warningsOnly)
                {
                    diagnostics.Add(ErrorCode.WRN_PatternIsAmbiguous, _syntax.Expression.Location, patternType, MessageID.IDS_Collection.Localize(), allApplicableMembers[0], allApplicableMembers[1]);
                }
            }
            instance3.Free();
            instance.Free();
            instance2.Free();
            return result;
        }

        private MethodArgumentInfo FindForEachPatternMethodViaExtension(BoundExpression collectionExpr, string methodName, BindingDiagnosticBag diagnostics)
        {
            var analyzedArguments = AnalyzedArguments.GetInstance();

            var methodGroupResolutionResult = this.BindExtensionMethod(
                _syntax.Expression,
                methodName,
                analyzedArguments,
                collectionExpr,
                typeArgumentsWithAnnotations: default,
                isMethodGroupConversion: false,
                returnRefKind: default,
                returnType: null,
                withDependencies: diagnostics.AccumulatesDependencies);

            diagnostics.AddRange(methodGroupResolutionResult.Diagnostics);

            var overloadResolutionResult = methodGroupResolutionResult.OverloadResolutionResult;
            if (overloadResolutionResult?.Succeeded ?? false)
            {
                var result = overloadResolutionResult.ValidResult.Member;

                if (result.CallsAreOmitted(_syntax.SyntaxTree))
                {
                    // Calls to this method are omitted in the current syntax tree, i.e it is either a partial method with no implementation part OR a conditional method whose condition is not true in this source file.
                    // We don't want to allow this case.
                    methodGroupResolutionResult.Free();
                    analyzedArguments.Free();
                    return null;
                }

                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                var collectionConversion = this.Conversions.ClassifyConversionFromExpression(collectionExpr, result.Parameters[0].Type, ref useSiteInfo);
                diagnostics.Add(_syntax, useSiteInfo);

                // Unconditionally convert here, to match what we set the ConvertedExpression to in the main BoundForEachStatement node.
                collectionExpr = new BoundConversion(
                    collectionExpr.Syntax,
                    collectionExpr,
                    collectionConversion,
                    @checked: CheckOverflowAtRuntime,
                    explicitCastInCode: false,
                    conversionGroupOpt: null,
                    ConstantValue.NotAvailable,
                    result.Parameters[0].Type);

                var info = BindDefaultArguments(
                    result,
                    collectionExpr,
                    expanded: overloadResolutionResult.ValidResult.Result.Kind == MemberResolutionKind.ApplicableInExpandedForm,
                    collectionExpr.Syntax,
                    diagnostics);
                methodGroupResolutionResult.Free();
                analyzedArguments.Free();
                return info;
            }
            else if (overloadResolutionResult?.GetAllApplicableMembers() is { } applicableMembers && applicableMembers.Length > 1)
            {
                diagnostics.Add(ErrorCode.WRN_PatternIsAmbiguous, _syntax.Expression.Location, collectionExpr.Type, MessageID.IDS_Collection.Localize(),
                    applicableMembers[0], applicableMembers[1]);
            }
            else if (overloadResolutionResult != null)
            {
                overloadResolutionResult.ReportDiagnostics(
                    binder: this,
                    location: _syntax.Expression.Location,
                    nodeOpt: _syntax.Expression,
                    diagnostics: diagnostics,
                    name: methodName,
                    receiver: null,
                    invokedExpression: _syntax.Expression,
                    arguments: methodGroupResolutionResult.AnalyzedArguments,
                    memberGroup: methodGroupResolutionResult.MethodGroup.Methods.ToImmutable(),
                    typeContainingConstructor: null,
                    delegateTypeBeingInvoked: null);
            }

            methodGroupResolutionResult.Free();
            analyzedArguments.Free();
            return null;
        }

        private bool SatisfiesForEachPattern(ref ForEachEnumeratorInfo.Builder builder, bool isAsync, BindingDiagnosticBag diagnostics)
        {
            TypeSymbol returnType = builder.GetEnumeratorInfo!.Method.ReturnType;
            switch (returnType.TypeKind)
            {
                case TypeKind.Submission:
                    throw ExceptionUtilities.UnexpectedValue(returnType.TypeKind);
                default:
                    return false;
                case TypeKind.Class:
                case TypeKind.Dynamic:
                case TypeKind.Interface:
                case TypeKind.Struct:
                case TypeKind.TypeParameter:
                    {
                        LookupResult instance = LookupResult.GetInstance();
                        try
                        {
                            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                            LookupMembersInType(instance, returnType, "Current", 0, null, LookupOptions.Default, this, diagnose: false, ref useSiteInfo);
                            diagnostics.Add(_syntax.Expression, useSiteInfo);
                            useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo);
                            if (!instance.IsSingleViable)
                            {
                                ReportPatternMemberLookupDiagnostics(instance, returnType, "Current", warningsOnly: false, diagnostics);
                                return false;
                            }
                            Symbol singleSymbolOrDefault = instance.SingleSymbolOrDefault;
                            if (singleSymbolOrDefault.IsStatic || singleSymbolOrDefault.DeclaredAccessibility != Accessibility.Public || singleSymbolOrDefault.Kind != SymbolKind.Property)
                            {
                                return false;
                            }
                            MethodSymbol ownOrInheritedGetMethod = ((PropertySymbol)singleSymbolOrDefault).GetOwnOrInheritedGetMethod();
                            if ((object)ownOrInheritedGetMethod == null)
                            {
                                return false;
                            }
                            bool num = IsAccessible(ownOrInheritedGetMethod, ref useSiteInfo);
                            diagnostics.Add(_syntax.Expression, useSiteInfo);
                            if (!num)
                            {
                                return false;
                            }
                            builder.CurrentPropertyGetter = ownOrInheritedGetMethod;
                            instance.Clear();
                            MethodArgumentInfo methodArgumentInfo = FindForEachPatternMethod(returnType, isAsync ? "MoveNextAsync" : "MoveNext", instance, warningsOnly: false, diagnostics, isAsync);
                            if ((object)methodArgumentInfo == null || methodArgumentInfo.Method.IsStatic || methodArgumentInfo.Method.DeclaredAccessibility != Accessibility.Public || IsInvalidMoveNextMethod(methodArgumentInfo.Method, isAsync))
                            {
                                return false;
                            }
                            builder.MoveNextInfo = methodArgumentInfo;
                            return true;
                        }
                        finally
                        {
                            instance.Free();
                        }
                    }
            }
        }

        private bool IsInvalidMoveNextMethod(MethodSymbol moveNextMethodCandidate, bool isAsync)
        {
            if (isAsync)
            {
                return false;
            }
            return moveNextMethodCandidate.OriginalDefinition.ReturnType.SpecialType != SpecialType.System_Boolean;
        }

        private void ReportEnumerableWarning(BindingDiagnosticBag diagnostics, TypeSymbol enumeratorType, Symbol patternMemberCandidate)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            if (IsAccessible(patternMemberCandidate, ref useSiteInfo))
            {
                diagnostics.Add(ErrorCode.WRN_PatternBadSignature, _syntax.Expression.Location, enumeratorType, MessageID.IDS_Collection.Localize(), patternMemberCandidate);
            }
            diagnostics.Add(_syntax.Expression, useSiteInfo);
        }

        private static bool IsIEnumerable(TypeSymbol type)
        {
            SpecialType specialType = type.OriginalDefinition.SpecialType;
            if ((uint)(specialType - 24) <= 1u)
            {
                return true;
            }
            return false;
        }

        private bool IsIAsyncEnumerable(TypeSymbol type)
        {
            return type.OriginalDefinition.Equals(base.Compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T));
        }

        private bool AllInterfacesContainsIEnumerable(ref ForEachEnumeratorInfo.Builder builder, TypeSymbol type, bool isAsync, BindingDiagnosticBag diagnostics, out bool foundMultiple)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
            NamedTypeSymbol namedTypeSymbol = GetIEnumerableOfT(type, isAsync, base.Compilation, ref useSiteInfo, out foundMultiple);
            if ((object)namedTypeSymbol == null || !IsAccessible(namedTypeSymbol, ref useSiteInfo))
            {
                namedTypeSymbol = null;
                if (!isAsync)
                {
                    NamedTypeSymbol specialType = base.Compilation.GetSpecialType(SpecialType.System_Collections_IEnumerable);
                    if ((object)specialType != null && base.Conversions.ClassifyImplicitConversionFromType(type, specialType, ref useSiteInfo).IsImplicit)
                    {
                        namedTypeSymbol = specialType;
                    }
                }
            }
            diagnostics.Add(_syntax.Expression, useSiteInfo);
            builder.CollectionType = namedTypeSymbol;
            return (object)namedTypeSymbol != null;
        }

        internal static NamedTypeSymbol GetIEnumerableOfT(TypeSymbol type, bool isAsync, CSharpCompilation compilation, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, out bool foundMultiple)
        {
            NamedTypeSymbol result = null;
            foundMultiple = false;
            if (type.TypeKind == TypeKind.TypeParameter)
            {
                TypeParameterSymbol typeParameterSymbol = (TypeParameterSymbol)type;
                GetIEnumerableOfT(typeParameterSymbol.EffectiveBaseClass(ref useSiteInfo).AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo).Concat(typeParameterSymbol.AllEffectiveInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo)), isAsync, compilation, ref result, ref foundMultiple);
            }
            else
            {
                GetIEnumerableOfT(type.AllInterfacesWithDefinitionUseSiteDiagnostics(ref useSiteInfo), isAsync, compilation, ref result, ref foundMultiple);
            }
            return result;
        }

        private static void GetIEnumerableOfT(ImmutableArray<NamedTypeSymbol> interfaces, bool isAsync, CSharpCompilation compilation, ref NamedTypeSymbol result, ref bool foundMultiple)
        {
            if (foundMultiple)
            {
                return;
            }
            interfaces = MethodTypeInferrer.ModuloReferenceTypeNullabilityDifferences(interfaces, VarianceKind.In);
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = interfaces.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (IsIEnumerableT(current.OriginalDefinition, isAsync, compilation))
                {
                    if ((object)result != null && !TypeSymbol.Equals(current, result, TypeCompareKind.IgnoreTupleNames))
                    {
                        foundMultiple = true;
                        break;
                    }
                    result = current;
                }
            }
        }

        internal static bool IsIEnumerableT(TypeSymbol type, bool isAsync, CSharpCompilation compilation)
        {
            if (isAsync)
            {
                return type.Equals(compilation.GetWellKnownType(WellKnownType.System_Collections_Generic_IAsyncEnumerable_T));
            }
            return type.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;
        }

        private void ReportPatternMemberLookupDiagnostics(LookupResult lookupResult, TypeSymbol patternType, string memberName, bool warningsOnly, BindingDiagnosticBag diagnostics)
        {
            if (lookupResult.Symbols.Any())
            {
                if (warningsOnly)
                {
                    ReportEnumerableWarning(diagnostics, patternType, lookupResult.Symbols.First());
                    return;
                }
                lookupResult.Clear();
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = GetNewCompoundUseSiteInfo(diagnostics);
                LookupMembersInType(lookupResult, patternType, memberName, 0, null, LookupOptions.Default, this, diagnose: true, ref useSiteInfo);
                diagnostics.Add(_syntax.Expression, useSiteInfo);
                if (lookupResult.Error != null)
                {
                    diagnostics.Add(lookupResult.Error, _syntax.Expression.Location);
                }
            }
            else if (!warningsOnly)
            {
                diagnostics.Add(ErrorCode.ERR_NoSuchMember, _syntax.Expression.Location, patternType, memberName);
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

        private MethodArgumentInfo GetParameterlessSpecialTypeMemberInfo(SpecialMember member, SyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            MethodSymbol methodSymbol = (MethodSymbol)GetSpecialTypeMember(member, diagnostics, syntax);
            if ((object)methodSymbol == null)
            {
                return null;
            }
            return MethodArgumentInfo.CreateParameterlessMethod(methodSymbol);
        }

        private MethodArgumentInfo BindDefaultArguments(MethodSymbol method, BoundExpression extensionReceiverOpt, bool expanded, SyntaxNode syntax, BindingDiagnosticBag diagnostics, bool assertMissingParametersAreOptional = true)
        {
            if (method.ParameterCount == 0)
            {
                return MethodArgumentInfo.CreateParameterlessMethod(method);
            }
            ArrayBuilder<BoundExpression> instance = ArrayBuilder<BoundExpression>.GetInstance(method.ParameterCount);
            if (method.IsExtensionMethod)
            {
                instance.Add(extensionReceiverOpt);
            }
            ImmutableArray<int> argsToParamsOpt = default(ImmutableArray<int>);
            BindDefaultArguments(syntax, method.Parameters, instance, null, ref argsToParamsOpt, out var defaultArguments, expanded, enableCallerInfo: true, diagnostics, assertMissingParametersAreOptional);
            return new MethodArgumentInfo(method, instance.ToImmutableAndFree(), argsToParamsOpt, defaultArguments, expanded);
        }
    }
}
