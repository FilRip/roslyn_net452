using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public class OverloadResolutionResult<TMember> where TMember : Symbol
    {
        private MemberResolutionResult<TMember> _bestResult;

        private ThreeState _bestResultState;

        internal readonly ArrayBuilder<MemberResolutionResult<TMember>> ResultsBuilder;

        private static readonly ObjectPool<OverloadResolutionResult<TMember>> s_pool = CreatePool();

        public bool Succeeded
        {
            get
            {
                EnsureBestResultLoaded();
                if (_bestResultState == ThreeState.True)
                {
                    return _bestResult.Result.IsValid;
                }
                return false;
            }
        }

        public MemberResolutionResult<TMember> ValidResult
        {
            get
            {
                EnsureBestResultLoaded();
                return _bestResult;
            }
        }

        public MemberResolutionResult<TMember> BestResult
        {
            get
            {
                EnsureBestResultLoaded();
                return _bestResult;
            }
        }

        public ImmutableArray<MemberResolutionResult<TMember>> Results => ResultsBuilder.ToImmutable();

        internal bool HasAnyApplicableMember
        {
            get
            {
                ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Result.IsApplicable)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        internal OverloadResolutionResult()
        {
            ResultsBuilder = new ArrayBuilder<MemberResolutionResult<TMember>>();
        }

        internal void Clear()
        {
            _bestResult = default(MemberResolutionResult<TMember>);
            _bestResultState = ThreeState.Unknown;
            ResultsBuilder.Clear();
        }

        private void EnsureBestResultLoaded()
        {
            if (!_bestResultState.HasValue())
            {
                _bestResultState = TryGetBestResult(ResultsBuilder, out _bestResult);
            }
        }

        internal ImmutableArray<TMember> GetAllApplicableMembers()
        {
            ArrayBuilder<TMember> instance = ArrayBuilder<TMember>.GetInstance();
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                if (current.Result.IsApplicable)
                {
                    instance.Add(current.Member);
                }
            }
            return instance.ToImmutableAndFree();
        }

        private static ThreeState TryGetBestResult(ArrayBuilder<MemberResolutionResult<TMember>> allResults, out MemberResolutionResult<TMember> best)
        {
            best = default(MemberResolutionResult<TMember>);
            ThreeState threeState = ThreeState.False;
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = allResults.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                if (current.Result.IsValid)
                {
                    if (threeState == ThreeState.True)
                    {
                        best = default(MemberResolutionResult<TMember>);
                        return ThreeState.False;
                    }
                    threeState = ThreeState.True;
                    best = current;
                }
            }
            return threeState;
        }

        internal void ReportDiagnostics<T>(Binder binder, Location location, SyntaxNode nodeOpt, BindingDiagnosticBag diagnostics, string name, BoundExpression receiver, SyntaxNode invokedExpression, AnalyzedArguments arguments, ImmutableArray<T> memberGroup, NamedTypeSymbol typeContainingConstructor, NamedTypeSymbol delegateTypeBeingInvoked, CSharpSyntaxNode queryClause = null, bool isMethodGroupConversion = false, RefKind? returnRefKind = null, TypeSymbol delegateOrFunctionPointerType = null) where T : Symbol
        {
            ImmutableArray<Symbol> symbols = StaticCast<Symbol>.From(memberGroup);
            if (HadAmbiguousBestMethods(diagnostics, symbols, location) || HadAmbiguousWorseMethods(diagnostics, symbols, location, queryClause != null, receiver, name) || HadLambdaConversionError(diagnostics, arguments) || HadStaticInstanceMismatch(diagnostics, symbols, invokedExpression?.GetLocation() ?? location, binder, receiver, nodeOpt, delegateOrFunctionPointerType) || (isMethodGroupConversion && returnRefKind.HasValue && HadReturnMismatch(location, diagnostics, returnRefKind.GetValueOrDefault(), delegateOrFunctionPointerType)) || HadConstraintFailure(location, diagnostics) || HadBadArguments(diagnostics, binder, name, arguments, symbols, location, binder.Flags, isMethodGroupConversion) || HadConstructedParameterFailedConstraintCheck(binder.Conversions, binder.Compilation, diagnostics, location) || InaccessibleTypeArgument(diagnostics, symbols, location) || TypeInferenceFailed(binder, diagnostics, symbols, receiver, arguments, location, queryClause) || UseSiteError())
            {
                return;
            }
            bool flag = false;
            MemberResolutionResult<TMember> memberResolutionResult = default(MemberResolutionResult<TMember>);
            MemberResolutionResult<TMember> firstUnsupported = default(MemberResolutionResult<TMember>);
            MemberResolutionResult<TMember>[] array = new MemberResolutionResult<TMember>[7];
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                switch (current.Result.Kind)
                {
                    case MemberResolutionKind.UnsupportedMetadata:
                        if (memberResolutionResult.IsNull)
                        {
                            firstUnsupported = current;
                        }
                        break;
                    case MemberResolutionKind.NoCorrespondingNamedParameter:
                        if (array[3].IsNull || current.Result.BadArgumentsOpt[0] > array[3].Result.BadArgumentsOpt[0])
                        {
                            array[3] = current;
                        }
                        break;
                    case MemberResolutionKind.NoCorrespondingParameter:
                        if (array[4].IsNull)
                        {
                            array[4] = current;
                        }
                        break;
                    case MemberResolutionKind.RequiredParameterMissing:
                        if (array[1].IsNull)
                        {
                            array[1] = current;
                        }
                        else
                        {
                            flag = true;
                        }
                        break;
                    case MemberResolutionKind.NameUsedForPositional:
                        if (array[2].IsNull || current.Result.BadArgumentsOpt[0] > array[2].Result.BadArgumentsOpt[0])
                        {
                            array[2] = current;
                        }
                        break;
                    case MemberResolutionKind.BadNonTrailingNamedArgument:
                        if (array[5].IsNull || current.Result.BadArgumentsOpt[0] > array[5].Result.BadArgumentsOpt[0])
                        {
                            array[5] = current;
                        }
                        break;
                    case MemberResolutionKind.DuplicateNamedArgument:
                        if (array[0].IsNull || current.Result.BadArgumentsOpt[0] > array[0].Result.BadArgumentsOpt[0])
                        {
                            array[0] = current;
                        }
                        break;
                    case MemberResolutionKind.WrongCallingConvention:
                        if (array[6].IsNull)
                        {
                            array[6] = current;
                        }
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(current.Result.Kind);
                }
            }
            MemberResolutionResult<TMember>[] array2 = array;
            for (int i = 0; i < array2.Length; i++)
            {
                MemberResolutionResult<TMember> memberResolutionResult2 = array2[i];
                if (memberResolutionResult2.IsNotNull)
                {
                    memberResolutionResult = memberResolutionResult2;
                    break;
                }
            }
            if (memberResolutionResult.IsNotNull)
            {
                if (memberResolutionResult.Member is FunctionPointerMethodSymbol && memberResolutionResult.Result.Kind == MemberResolutionKind.NoCorrespondingNamedParameter)
                {
                    int index = memberResolutionResult.Result.BadArgumentsOpt[0];
                    IdentifierNameSyntax identifierNameSyntax = arguments.Names[index];
                    diagnostics.Add(ErrorCode.ERR_FunctionPointersCannotBeCalledWithNamedArguments, identifierNameSyntax.Location);
                    return;
                }
                if (!(memberResolutionResult.Result.Kind == MemberResolutionKind.RequiredParameterMissing && flag) && !isMethodGroupConversion && !(memberResolutionResult.Member is FunctionPointerMethodSymbol))
                {
                    switch (memberResolutionResult.Result.Kind)
                    {
                        case MemberResolutionKind.NameUsedForPositional:
                            ReportNameUsedForPositional(memberResolutionResult, diagnostics, arguments, symbols);
                            return;
                        case MemberResolutionKind.NoCorrespondingNamedParameter:
                            ReportNoCorrespondingNamedParameter(memberResolutionResult, name, diagnostics, arguments, delegateTypeBeingInvoked, symbols);
                            return;
                        case MemberResolutionKind.RequiredParameterMissing:
                            ReportMissingRequiredParameter(memberResolutionResult, diagnostics, delegateTypeBeingInvoked, symbols, location);
                            return;
                        case MemberResolutionKind.BadNonTrailingNamedArgument:
                            ReportBadNonTrailingNamedArgument(memberResolutionResult, diagnostics, arguments, symbols);
                            return;
                        case MemberResolutionKind.DuplicateNamedArgument:
                            ReportDuplicateNamedArgument(memberResolutionResult, diagnostics, arguments);
                            return;
                    }
                }
                else if (memberResolutionResult.Result.Kind == MemberResolutionKind.WrongCallingConvention)
                {
                    ReportWrongCallingConvention(location, diagnostics, symbols, memberResolutionResult, ((FunctionPointerTypeSymbol)delegateOrFunctionPointerType).Signature);
                    return;
                }
            }
            else if (firstUnsupported.IsNotNull)
            {
                ReportUnsupportedMetadata(location, diagnostics, symbols, firstUnsupported);
                return;
            }
            if (!isMethodGroupConversion)
            {
                ReportBadParameterCount(diagnostics, name, arguments, symbols, location, typeContainingConstructor, delegateTypeBeingInvoked);
            }
        }

        private static void ReportUnsupportedMetadata(Location location, BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, MemberResolutionResult<TMember> firstUnsupported)
        {
            DiagnosticInfo diagnosticInfo = firstUnsupported.Member.GetUseSiteInfo().DiagnosticInfo;
            diagnosticInfo = new DiagnosticInfoWithSymbols((ErrorCode)diagnosticInfo.Code, diagnosticInfo.Arguments, symbols);
            Symbol.ReportUseSiteDiagnostic(diagnosticInfo, diagnostics, location);
        }

        private static void ReportWrongCallingConvention(Location location, BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, MemberResolutionResult<TMember> firstSupported, MethodSymbol target)
        {
            diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_WrongFuncPtrCallingConvention, new object[2] { firstSupported.Member, target.CallingConvention }, symbols), location);
        }

        private bool UseSiteError()
        {
            if (GetFirstMemberKind(MemberResolutionKind.UseSiteError).IsNull)
            {
                return false;
            }
            return true;
        }

        private bool InaccessibleTypeArgument(BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, Location location)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.InaccessibleTypeArgument);
            if (firstMemberKind.IsNull)
            {
                return false;
            }
            diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_BadAccess, new object[1] { firstMemberKind.Member }, symbols), location);
            return true;
        }

        private bool HadStaticInstanceMismatch(BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, Location location, Binder binder, BoundExpression receiverOpt, SyntaxNode nodeOpt, TypeSymbol delegateOrFunctionPointerType)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.StaticInstanceMismatch);
            if (firstMemberKind.IsNull)
            {
                return false;
            }
            Symbol member = firstMemberKind.Member;
            if (receiverOpt != null && receiverOpt.Kind == BoundKind.QueryClause)
            {
                diagnostics.Add(ErrorCode.ERR_QueryNoProvider, location, receiverOpt.Type, member.Name);
            }
            else if (binder.Flags.Includes(BinderFlags.CollectionInitializerAddMethod))
            {
                diagnostics.Add(ErrorCode.ERR_InitializerAddHasWrongSignature, location, member);
            }
            else if (nodeOpt != null && nodeOpt.Kind() == SyntaxKind.AwaitExpression && member.Name == "GetAwaiter")
            {
                diagnostics.Add(ErrorCode.ERR_BadAwaitArg, location, receiverOpt.Type);
            }
            else if (delegateOrFunctionPointerType is FunctionPointerTypeSymbol)
            {
                diagnostics.Add(ErrorCode.ERR_FuncPtrMethMustBeStatic, location, member);
            }
            else
            {
                ErrorCode errorCode = ((!member.RequiresInstanceReceiver()) ? ErrorCode.ERR_ObjectProhibited : ((Binder.WasImplicitReceiver(receiverOpt) && binder.InFieldInitializer && !binder.BindingTopLevelScriptCode) ? ErrorCode.ERR_FieldInitRefNonstatic : ErrorCode.ERR_ObjectRequired));
                diagnostics.Add(new DiagnosticInfoWithSymbols(errorCode, new object[1] { member }, symbols), location);
            }
            return true;
        }

        private bool HadReturnMismatch(Location location, BindingDiagnosticBag diagnostics, RefKind refKind, TypeSymbol delegateOrFunctionPointerType)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.WrongRefKind);
            if (!firstMemberKind.IsNull)
            {
                diagnostics.Add(delegateOrFunctionPointerType.IsFunctionPointer() ? ErrorCode.ERR_FuncPtrRefMismatch : ErrorCode.ERR_DelegateRefMismatch, location, firstMemberKind.Member, delegateOrFunctionPointerType);
                return true;
            }
            firstMemberKind = GetFirstMemberKind(MemberResolutionKind.WrongReturnType);
            if (!firstMemberKind.IsNull)
            {
                MethodSymbol methodSymbol = (MethodSymbol)(object)firstMemberKind.Member;
                diagnostics.Add(ErrorCode.ERR_BadRetType, location, methodSymbol, methodSymbol.ReturnType);
                return true;
            }
            return false;
        }

        private bool HadConstraintFailure(Location location, BindingDiagnosticBag diagnostics)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.ConstraintFailure);
            if (firstMemberKind.IsNull)
            {
                return false;
            }
            ImmutableArray<TypeParameterDiagnosticInfo>.Enumerator enumerator = firstMemberKind.Result.ConstraintFailureDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterDiagnosticInfo current = enumerator.Current;
                if (current.UseSiteInfo.DiagnosticInfo != null)
                {
                    diagnostics.Add(new CSDiagnostic(current.UseSiteInfo.DiagnosticInfo, location));
                }
            }
            return true;
        }

        private bool TypeInferenceFailed(Binder binder, BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, BoundExpression receiver, AnalyzedArguments arguments, Location location, CSharpSyntaxNode queryClause = null)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.TypeInferenceFailed);
            if (firstMemberKind.IsNotNull)
            {
                if (queryClause != null)
                {
                    Binder.ReportQueryInferenceFailed(queryClause, firstMemberKind.Member.Name, receiver, arguments, symbols, diagnostics);
                }
                else
                {
                    diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_CantInferMethTypeArgs, new object[1] { firstMemberKind.Member }, symbols), location);
                }
                return true;
            }
            firstMemberKind = GetFirstMemberKind(MemberResolutionKind.TypeInferenceExtensionInstanceArgument);
            if (firstMemberKind.IsNotNull)
            {
                BoundExpression boundExpression = arguments.Arguments[0];
                if (queryClause != null)
                {
                    binder.ReportQueryLookupFailed(queryClause, boundExpression, firstMemberKind.Member.Name, symbols, diagnostics);
                }
                else
                {
                    diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_NoSuchMemberOrExtension, new object[2]
                    {
                        boundExpression.Type,
                        firstMemberKind.Member.Name
                    }, symbols), location);
                }
                return true;
            }
            return false;
        }

        private static void ReportNameUsedForPositional(MemberResolutionResult<TMember> bad, BindingDiagnosticBag diagnostics, AnalyzedArguments arguments, ImmutableArray<Symbol> symbols)
        {
            int index = bad.Result.BadArgumentsOpt[0];
            IdentifierNameSyntax identifierNameSyntax = arguments.Names[index];
            Location location = new SourceLocation(identifierNameSyntax);
            diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_NamedArgumentUsedInPositional, new object[1] { identifierNameSyntax.Identifier.ValueText }, symbols), location);
        }

        private static void ReportBadNonTrailingNamedArgument(MemberResolutionResult<TMember> bad, BindingDiagnosticBag diagnostics, AnalyzedArguments arguments, ImmutableArray<Symbol> symbols)
        {
            int index = bad.Result.BadArgumentsOpt[0];
            IdentifierNameSyntax identifierNameSyntax = arguments.Names[index];
            Location location = new SourceLocation(identifierNameSyntax);
            diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_BadNonTrailingNamedArgument, new object[1] { identifierNameSyntax.Identifier.ValueText }, symbols), location);
        }

        private static void ReportDuplicateNamedArgument(MemberResolutionResult<TMember> result, BindingDiagnosticBag diagnostics, AnalyzedArguments arguments)
        {
            IdentifierNameSyntax identifierNameSyntax = arguments.Names[result.Result.BadArgumentsOpt[0]];
            diagnostics.Add(new CSDiagnosticInfo(ErrorCode.ERR_DuplicateNamedArgument, identifierNameSyntax.Identifier.Text), identifierNameSyntax.Location);
        }

        private static void ReportNoCorrespondingNamedParameter(MemberResolutionResult<TMember> bad, string methodName, BindingDiagnosticBag diagnostics, AnalyzedArguments arguments, NamedTypeSymbol delegateTypeBeingInvoked, ImmutableArray<Symbol> symbols)
        {
            int index = bad.Result.BadArgumentsOpt[0];
            IdentifierNameSyntax identifierNameSyntax = arguments.Names[index];
            Location location = new SourceLocation(identifierNameSyntax);
            ErrorCode errorCode = (((object)delegateTypeBeingInvoked != null) ? ErrorCode.ERR_BadNamedArgumentForDelegateInvoke : ErrorCode.ERR_BadNamedArgument);
            object obj = delegateTypeBeingInvoked ?? ((object)methodName);
            diagnostics.Add(new DiagnosticInfoWithSymbols(errorCode, new object[2]
            {
                obj,
                identifierNameSyntax.Identifier.ValueText
            }, symbols), location);
        }

        private static void ReportMissingRequiredParameter(MemberResolutionResult<TMember> bad, BindingDiagnosticBag diagnostics, NamedTypeSymbol delegateTypeBeingInvoked, ImmutableArray<Symbol> symbols, Location location)
        {
            TMember member = bad.Member;
            ImmutableArray<ParameterSymbol> parameters = member.GetParameters();
            int badParameter = bad.Result.BadParameter;
            string text = ((badParameter != parameters.Length) ? parameters[badParameter].Name : SyntaxFacts.GetText(SyntaxKind.ArgListKeyword));
            object obj = delegateTypeBeingInvoked ?? ((object)member);
            diagnostics.Add(new DiagnosticInfoWithSymbols(ErrorCode.ERR_NoCorrespondingArgument, new object[2] { text, obj }, symbols), location);
        }

        private static void ReportBadParameterCount(BindingDiagnosticBag diagnostics, string name, AnalyzedArguments arguments, ImmutableArray<Symbol> symbols, Location location, NamedTypeSymbol typeContainingConstructor, NamedTypeSymbol delegateTypeBeingInvoked)
        {
            FunctionPointerMethodSymbol functionPointerMethodSymbol = ((symbols.IsDefault || symbols.Length != 1) ? null : (symbols[0] as FunctionPointerMethodSymbol));
            (ErrorCode, object) tuple;
            if ((object)typeContainingConstructor == null)
            {
                if ((object)delegateTypeBeingInvoked == null)
                {
                    object obj = functionPointerMethodSymbol;
                    tuple = ((obj == null) ? (ErrorCode.ERR_BadArgCount, name) : (ErrorCode.ERR_BadFuncPointerArgCount, obj));
                }
                else
                {
                    tuple = (ErrorCode.ERR_BadDelArgCount, delegateTypeBeingInvoked);
                }
            }
            else
            {
                tuple = (ErrorCode.ERR_BadCtorArgCount, typeContainingConstructor);
            }
            (ErrorCode, object) tuple2 = tuple;
            ErrorCode item = tuple2.Item1;
            object item2 = tuple2.Item2;
            int num = arguments.Arguments.Count;
            if (arguments.IsExtensionMethodInvocation)
            {
                num--;
            }
            diagnostics.Add(new DiagnosticInfoWithSymbols(item, new object[2] { item2, num }, symbols), location);
        }

        private bool HadConstructedParameterFailedConstraintCheck(ConversionsBase conversions, CSharpCompilation compilation, BindingDiagnosticBag diagnostics, Location location)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.ConstructedParameterFailedConstraintCheck);
            if (firstMemberKind.IsNull)
            {
                return false;
            }
            MethodSymbol methodSymbol = (MethodSymbol)(object)firstMemberKind.Member;
            ConstraintsHelper.CheckConstraintsArgs args = new ConstraintsHelper.CheckConstraintsArgs(compilation, conversions, includeNullability: false, location, diagnostics);
            if (!methodSymbol.CheckConstraints(in args))
            {
                return true;
            }
            methodSymbol.GetParameterType(firstMemberKind.Result.BadParameter).CheckAllConstraints(new ConstraintsHelper.CheckConstraintsArgsBoxed(compilation, conversions, includeNullability: false, location, diagnostics));
            return true;
        }

        private static bool HadLambdaConversionError(BindingDiagnosticBag diagnostics, AnalyzedArguments arguments)
        {
            bool flag = false;
            ArrayBuilder<BoundExpression>.Enumerator enumerator = arguments.Arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                BoundExpression current = enumerator.Current;
                if (current.Kind == BoundKind.UnboundLambda)
                {
                    flag |= ((UnboundLambda)current).GenerateSummaryErrors(diagnostics);
                }
            }
            return flag;
        }

        private bool HadBadArguments(BindingDiagnosticBag diagnostics, Binder binder, string name, AnalyzedArguments arguments, ImmutableArray<Symbol> symbols, Location location, BinderFlags flags, bool isMethodGroupConversion)
        {
            MemberResolutionResult<TMember> firstMemberKind = GetFirstMemberKind(MemberResolutionKind.BadArgumentConversion);
            if (firstMemberKind.IsNull)
            {
                return false;
            }
            if (isMethodGroupConversion)
            {
                return true;
            }
            TMember member = firstMemberKind.Member;
            if (flags.Includes(BinderFlags.CollectionInitializerAddMethod))
            {
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = member.GetParameters().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.RefKind != 0)
                    {
                        diagnostics.Add(ErrorCode.ERR_InitializerAddHasParamModifiers, location, symbols, member);
                        return true;
                    }
                }
                diagnostics.Add(ErrorCode.ERR_BadArgTypesForCollectionAdd, location, symbols, member);
            }
            ImmutableArray<int>.Enumerator enumerator2 = firstMemberKind.Result.BadArgumentsOpt.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                int current = enumerator2.Current;
                ReportBadArgumentError(diagnostics, binder, name, arguments, symbols, location, firstMemberKind, member, current);
            }
            return true;
        }

        private static void ReportBadArgumentError(BindingDiagnosticBag diagnostics, Binder binder, string name, AnalyzedArguments arguments, ImmutableArray<Symbol> symbols, Location location, MemberResolutionResult<TMember> badArg, TMember method, int arg)
        {
            BoundExpression boundExpression = arguments.Argument(arg);
            if (boundExpression.HasAnyErrors)
            {
                return;
            }
            int num = badArg.Result.ParameterFromArgument(arg);
            SourceLocation location2 = new SourceLocation(boundExpression.Syntax);
            if (method.GetIsVararg() && num == method.GetParameterCount())
            {
                diagnostics.Add(ErrorCode.ERR_BadArgType, location2, symbols, arg + 1, boundExpression.Display, "__arglist");
                return;
            }
            ParameterSymbol parameterSymbol = method.GetParameters()[num];
            bool isLastParameter = method.GetParameterCount() == num + 1;
            RefKind refKind = arguments.RefKind(arg);
            RefKind refKind2 = parameterSymbol.RefKind;
            if (arguments.IsExtensionMethodThisArgument(arg) && (refKind2 == RefKind.Ref || refKind2 == RefKind.In))
            {
                refKind = refKind2;
            }
            if (!boundExpression.HasExpressionType() && boundExpression.Kind != BoundKind.OutDeconstructVarPendingInference && boundExpression.Kind != BoundKind.OutVariablePendingInference && boundExpression.Kind != BoundKind.DiscardExpression)
            {
                TypeSymbol typeSymbol2 = ((UnwrapIfParamsArray(parameterSymbol, isLastParameter) is TypeSymbol typeSymbol) ? typeSymbol : parameterSymbol.Type);
                if (boundExpression.Kind == BoundKind.UnboundLambda && refKind == refKind2)
                {
                    ((UnboundLambda)boundExpression).GenerateAnonymousFunctionConversionError(diagnostics, typeSymbol2);
                }
                else if (boundExpression.Kind != BoundKind.MethodGroup || typeSymbol2.TypeKind != TypeKind.Delegate || !binder.Conversions.ReportDelegateOrFunctionPointerMethodGroupDiagnostics(binder, (BoundMethodGroup)boundExpression, typeSymbol2, diagnostics))
                {
                    if (boundExpression.Kind == BoundKind.MethodGroup && typeSymbol2.TypeKind == TypeKind.FunctionPointer)
                    {
                        diagnostics.Add(ErrorCode.ERR_MissingAddressOf, location2);
                    }
                    else if (boundExpression.Kind != BoundKind.UnconvertedAddressOfOperator || !binder.Conversions.ReportDelegateOrFunctionPointerMethodGroupDiagnostics(binder, ((BoundUnconvertedAddressOfOperator)boundExpression).Operand, typeSymbol2, diagnostics))
                    {
                        diagnostics.Add(ErrorCode.ERR_BadArgType, location2, symbols, arg + 1, boundExpression.Display, UnwrapIfParamsArray(parameterSymbol, isLastParameter));
                    }
                }
            }
            else if (refKind != refKind2 && (refKind != 0 || refKind2 != RefKind.In))
            {
                if (refKind2 == RefKind.None || refKind2 == RefKind.In)
                {
                    diagnostics.Add(ErrorCode.ERR_BadArgExtraRef, location2, symbols, arg + 1, refKind.ToArgumentDisplayString());
                }
                else
                {
                    diagnostics.Add(ErrorCode.ERR_BadArgRef, location2, symbols, arg + 1, refKind2.ToParameterDisplayString());
                }
            }
            else if (arguments.IsExtensionMethodThisArgument(arg))
            {
                diagnostics.Add(ErrorCode.ERR_BadInstanceArgType, location2, symbols, boundExpression.Display, name, method, parameterSymbol);
            }
            else if (boundExpression.Display is TypeSymbol typeSymbol3)
            {
                SignatureOnlyParameterSymbol symbol = new SignatureOnlyParameterSymbol(TypeWithAnnotations.Create(typeSymbol3), ImmutableArray<CustomModifier>.Empty, isParams: false, refKind);
                SymbolDistinguisher symbolDistinguisher = new SymbolDistinguisher(binder.Compilation, symbol, UnwrapIfParamsArray(parameterSymbol, isLastParameter));
                diagnostics.Add(ErrorCode.ERR_BadArgType, location2, symbols, arg + 1, symbolDistinguisher.First, symbolDistinguisher.Second);
            }
            else
            {
                diagnostics.Add(ErrorCode.ERR_BadArgType, location2, symbols, arg + 1, boundExpression.Display, UnwrapIfParamsArray(parameterSymbol, isLastParameter));
            }
        }

        private static Symbol UnwrapIfParamsArray(ParameterSymbol parameter, bool isLastParameter)
        {
            if (parameter.IsParams && isLastParameter && parameter.Type is ArrayTypeSymbol arrayTypeSymbol && arrayTypeSymbol.IsSZArray)
            {
                return arrayTypeSymbol.ElementType;
            }
            return parameter;
        }

        private bool HadAmbiguousWorseMethods(BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, Location location, bool isQuery, BoundExpression receiver, string name)
        {
            if (TryGetFirstTwoWorseResults(out var first, out var second) <= 1)
            {
                return false;
            }
            if (isQuery)
            {
                diagnostics.Add(ErrorCode.ERR_QueryMultipleProviders, location, receiver.Type, name);
            }
            else
            {
                diagnostics.Add(CreateAmbiguousCallDiagnosticInfo(first.LeastOverriddenMember.OriginalDefinition, second.LeastOverriddenMember.OriginalDefinition, symbols), location);
            }
            return true;
        }

        private int TryGetFirstTwoWorseResults(out MemberResolutionResult<TMember> first, out MemberResolutionResult<TMember> second)
        {
            int num = 0;
            bool flag = false;
            bool flag2 = false;
            first = default(MemberResolutionResult<TMember>);
            second = default(MemberResolutionResult<TMember>);
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                if (current.Result.Kind == MemberResolutionKind.Worse)
                {
                    num++;
                    if (!flag)
                    {
                        first = current;
                        flag = true;
                    }
                    else if (!flag2)
                    {
                        second = current;
                        flag2 = true;
                    }
                }
            }
            return num;
        }

        private bool HadAmbiguousBestMethods(BindingDiagnosticBag diagnostics, ImmutableArray<Symbol> symbols, Location location)
        {
            if (TryGetFirstTwoValidResults(out var first, out var second) <= 1)
            {
                return false;
            }
            diagnostics.Add(CreateAmbiguousCallDiagnosticInfo(first.LeastOverriddenMember.OriginalDefinition, second.LeastOverriddenMember.OriginalDefinition, symbols), location);
            return true;
        }

        private int TryGetFirstTwoValidResults(out MemberResolutionResult<TMember> first, out MemberResolutionResult<TMember> second)
        {
            int num = 0;
            bool flag = false;
            bool flag2 = false;
            first = default(MemberResolutionResult<TMember>);
            second = default(MemberResolutionResult<TMember>);
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                if (current.Result.IsValid)
                {
                    num++;
                    if (!flag)
                    {
                        first = current;
                        flag = true;
                    }
                    else if (!flag2)
                    {
                        second = current;
                        flag2 = true;
                    }
                }
            }
            return num;
        }

        private static DiagnosticInfoWithSymbols CreateAmbiguousCallDiagnosticInfo(Symbol first, Symbol second, ImmutableArray<Symbol> symbols)
        {
            object[] arguments = ((!(first.ContainingNamespace != second.ContainingNamespace)) ? new object[2] { first, second } : new object[2]
            {
                new FormattedSymbol(first, SymbolDisplayFormat.CSharpErrorMessageFormat),
                new FormattedSymbol(second, SymbolDisplayFormat.CSharpErrorMessageFormat)
            });
            return new DiagnosticInfoWithSymbols(ErrorCode.ERR_AmbigCall, arguments, symbols);
        }

        [Conditional("DEBUG")]
        private void AssertNone(MemberResolutionKind kind)
        {
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Result.Kind == kind)
                {
                    throw ExceptionUtilities.UnexpectedValue(kind);
                }
            }
        }

        private MemberResolutionResult<TMember> GetFirstMemberKind(MemberResolutionKind kind)
        {
            ArrayBuilder<MemberResolutionResult<TMember>>.Enumerator enumerator = ResultsBuilder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberResolutionResult<TMember> current = enumerator.Current;
                if (current.Result.Kind == kind)
                {
                    return current;
                }
            }
            return default(MemberResolutionResult<TMember>);
        }

        internal static OverloadResolutionResult<TMember> GetInstance()
        {
            return s_pool.Allocate();
        }

        internal void Free()
        {
            Clear();
            s_pool.Free(this);
        }

        private static ObjectPool<OverloadResolutionResult<TMember>> CreatePool()
        {
            return new ObjectPool<OverloadResolutionResult<TMember>>(() => new OverloadResolutionResult<TMember>(), 10);
        }
    }
}
