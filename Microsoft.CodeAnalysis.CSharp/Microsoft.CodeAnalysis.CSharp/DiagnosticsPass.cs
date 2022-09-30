using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class DiagnosticsPass : BoundTreeWalkerWithStackGuard
    {
        private readonly BindingDiagnosticBag _diagnostics;

        private readonly CSharpCompilation _compilation;

        private bool _inExpressionLambda;

        private bool _reportedUnsafe;

        private readonly MethodSymbol _containingSymbol;

        private SourceMethodSymbol _staticLocalOrAnonymousFunction;

        public static void IssueDiagnostics(CSharpCompilation compilation, BoundNode node, BindingDiagnosticBag diagnostics, MethodSymbol containingSymbol)
        {
            ExecutableCodeBinder.ValidateIteratorMethod(compilation, containingSymbol, diagnostics);
            try
            {
                new DiagnosticsPass(compilation, diagnostics, containingSymbol).Visit(node);
            }
            catch (CancelledByStackGuardException ex)
            {
                ex.AddAnError(diagnostics);
            }
        }

        private DiagnosticsPass(CSharpCompilation compilation, BindingDiagnosticBag diagnostics, MethodSymbol containingSymbol)
        {
            _compilation = compilation;
            _diagnostics = diagnostics;
            _containingSymbol = containingSymbol;
        }

        private void Error(ErrorCode code, BoundNode node, params object[] args)
        {
            _diagnostics.Add(code, node.Syntax.Location, args);
        }

        private void CheckUnsafeType(BoundExpression e)
        {
            if (e != null && (object)e.Type != null && e.Type.IsPointerOrFunctionPointer())
            {
                NoteUnsafe(e);
            }
        }

        private void NoteUnsafe(BoundNode node)
        {
            if (_inExpressionLambda && !_reportedUnsafe)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsPointerOp, node);
                _reportedUnsafe = true;
            }
        }

        public override BoundNode VisitArrayCreation(BoundArrayCreation node)
        {
            ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)node.Type;
            if (_inExpressionLambda && node.InitializerOpt != null && !arrayTypeSymbol.IsSZArray)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsMultiDimensionalArrayInitializer, node);
            }
            return base.VisitArrayCreation(node);
        }

        public override BoundNode VisitArrayAccess(BoundArrayAccess node)
        {
            if (_inExpressionLambda && node.Indices.Length == 1 && node.Indices[0].Type!.SpecialType == SpecialType.None)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsPatternIndexOrRangeIndexer, node);
            }
            return base.VisitArrayAccess(node);
        }

        public override BoundNode VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsPatternIndexOrRangeIndexer, node);
            }
            return base.VisitIndexOrRangePatternIndexerAccess(node);
        }

        public override BoundNode VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsFromEndIndexExpression, node);
            }
            return base.VisitFromEndIndexExpression(node);
        }

        public override BoundNode VisitRangeExpression(BoundRangeExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsRangeExpression, node);
            }
            return base.VisitRangeExpression(node);
        }

        public override BoundNode VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            if (_inExpressionLambda && node.ConstantValue == null)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsPointerOp, node);
            }
            return base.VisitSizeOfOperator(node);
        }

        public override BoundNode VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            ExecutableCodeBinder.ValidateIteratorMethod(_compilation, node.Symbol, _diagnostics);
            SourceMethodSymbol staticLocalOrAnonymousFunction = _staticLocalOrAnonymousFunction;
            if (node.Symbol.IsStatic)
            {
                _staticLocalOrAnonymousFunction = node.Symbol;
            }
            BoundNode? result = base.VisitLocalFunctionStatement(node);
            _staticLocalOrAnonymousFunction = staticLocalOrAnonymousFunction;
            return result;
        }

        public override BoundNode VisitThisReference(BoundThisReference node)
        {
            CheckReferenceToThisOrBase(node);
            return base.VisitThisReference(node);
        }

        public override BoundNode VisitBaseReference(BoundBaseReference node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsBaseAccess, node);
            }
            CheckReferenceToThisOrBase(node);
            return base.VisitBaseReference(node);
        }

        public override BoundNode VisitLocal(BoundLocal node)
        {
            CheckReferenceToVariable(node, node.LocalSymbol);
            return base.VisitLocal(node);
        }

        public override BoundNode VisitParameter(BoundParameter node)
        {
            CheckReferenceToVariable(node, node.ParameterSymbol);
            return base.VisitParameter(node);
        }

        private void CheckReferenceToThisOrBase(BoundExpression node)
        {
            if ((object)_staticLocalOrAnonymousFunction != null)
            {
                ErrorCode code = ((_staticLocalOrAnonymousFunction.MethodKind == MethodKind.LocalFunction) ? ErrorCode.ERR_StaticLocalFunctionCannotCaptureThis : ErrorCode.ERR_StaticAnonymousFunctionCannotCaptureThis);
                Error(code, node);
            }
        }

        private void CheckReferenceToVariable(BoundExpression node, Symbol symbol)
        {
            if ((object)_staticLocalOrAnonymousFunction != null && Symbol.IsCaptured(symbol, _staticLocalOrAnonymousFunction))
            {
                ErrorCode code = ((_staticLocalOrAnonymousFunction.MethodKind == MethodKind.LocalFunction) ? ErrorCode.ERR_StaticLocalFunctionCannotCaptureVariable : ErrorCode.ERR_StaticAnonymousFunctionCannotCaptureVariable);
                Error(code, node, new FormattedSymbol(symbol, SymbolDisplayFormat.ShortFormat));
            }
        }

        private void CheckReferenceToMethodIfLocalFunction(BoundExpression node, MethodSymbol method)
        {
            if (method?.OriginalDefinition is LocalFunctionSymbol symbol)
            {
                CheckReferenceToVariable(node, symbol);
            }
        }

        public override BoundNode VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsSwitchExpression, node);
            }
            return base.VisitConvertedSwitchExpression(node);
        }

        public override BoundNode VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            if (!node.HasAnyErrors)
            {
                CheckForDeconstructionAssignmentToSelf(node.Left, node.Right);
            }
            return base.VisitDeconstructionAssignmentOperator(node);
        }

        public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            CheckForAssignmentToSelf(node);
            if (_inExpressionLambda && node.Left.Kind != BoundKind.ObjectInitializerMember && node.Left.Kind != BoundKind.DynamicObjectInitializerMember)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsAssignment, node);
            }
            return base.VisitAssignmentOperator(node);
        }

        public override BoundNode VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
            return base.VisitDynamicObjectInitializerMember(node);
        }

        public override BoundNode VisitEventAccess(BoundEventAccess node)
        {
            if (node.IsUsableAsField)
            {
                bool hasBaseReceiver = node.ReceiverOpt != null && node.ReceiverOpt!.Kind == BoundKind.BaseReference;
                Binder.ReportDiagnosticsIfObsolete(_diagnostics, node.EventSymbol.AssociatedField, node.Syntax, hasBaseReceiver, _containingSymbol, _containingSymbol.ContainingType, BinderFlags.None);
            }
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitEventAccess(node);
        }

        public override BoundNode VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsAssignment, node);
            }
            bool hasBaseReceiver = node.ReceiverOpt != null && node.ReceiverOpt!.Kind == BoundKind.BaseReference;
            Binder.ReportDiagnosticsIfObsolete(_diagnostics, node.Event, ((AssignmentExpressionSyntax)node.Syntax).Left, hasBaseReceiver, _containingSymbol, _containingSymbol.ContainingType, BinderFlags.None);
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitEventAssignmentOperator(node);
        }

        public override BoundNode VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            CheckCompoundAssignmentOperator(node);
            return base.VisitCompoundAssignmentOperator(node);
        }

        private void VisitCall(MethodSymbol method, PropertySymbol propertyAccess, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<string> argumentNamesOpt, BitVector defaultArguments, BoundNode node)
        {
            CheckArguments(argumentRefKindsOpt, arguments, method);
            if (_inExpressionLambda)
            {
                if (method.CallsAreOmitted(node.SyntaxTree))
                {
                    Error(ErrorCode.ERR_PartialMethodInExpressionTree, node);
                }
                else if ((object)propertyAccess != null && propertyAccess.IsIndexedProperty() && !propertyAccess.IsIndexer)
                {
                    Error(ErrorCode.ERR_ExpressionTreeContainsIndexedProperty, node);
                }
                else if (hasDefaultArgument(arguments, defaultArguments))
                {
                    Error(ErrorCode.ERR_ExpressionTreeContainsOptionalArgument, node);
                }
                else if (!argumentNamesOpt.IsDefaultOrEmpty)
                {
                    Error(ErrorCode.ERR_ExpressionTreeContainsNamedArgument, node);
                }
                else if (IsComCallWithRefOmitted(method, arguments, argumentRefKindsOpt))
                {
                    Error(ErrorCode.ERR_ComRefCallInExpressionTree, node);
                }
                else if (method.MethodKind == MethodKind.LocalFunction)
                {
                    Error(ErrorCode.ERR_ExpressionTreeContainsLocalFunction, node);
                }
                else if (method.RefKind != 0)
                {
                    Error(ErrorCode.ERR_RefReturningCallInExpressionTree, node);
                }
            }
            static bool hasDefaultArgument(ImmutableArray<BoundExpression> arguments, BitVector defaultArguments)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (defaultArguments[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override BoundNode Visit(BoundNode node)
        {
            if (_inExpressionLambda && !(node is BoundConversion) && node is BoundExpression boundExpression)
            {
                TypeSymbol type = boundExpression.Type;
                if ((object)type != null && type.IsRestrictedType())
                {
                    Error(ErrorCode.ERR_ExpressionTreeCantContainRefStruct, node, type.Name);
                }
            }
            return base.Visit(node);
        }

        public override BoundNode VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_FeatureNotValidInExpressionTree, node, "__reftype");
            }
            return base.VisitRefTypeOperator(node);
        }

        public override BoundNode VisitRefValueOperator(BoundRefValueOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_FeatureNotValidInExpressionTree, node, "__refvalue");
            }
            return base.VisitRefValueOperator(node);
        }

        public override BoundNode VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_FeatureNotValidInExpressionTree, node, "__makeref");
            }
            return base.VisitMakeRefOperator(node);
        }

        public override BoundNode VisitArgListOperator(BoundArgListOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_VarArgsInExpressionTree, node);
            }
            return base.VisitArgListOperator(node);
        }

        public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_NullPropagatingOpInExpressionTree, node);
            }
            return base.VisitConditionalAccess(node);
        }

        public override BoundNode VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            if (_inExpressionLambda && !node.Arguments.IsDefaultOrEmpty)
            {
                Error(ErrorCode.ERR_DictionaryInitializerInExpressionTree, node);
            }
            if (node.MemberSymbol is PropertySymbol property)
            {
                CheckRefReturningPropertyAccess(node, property);
            }
            return base.VisitObjectInitializerMember(node);
        }

        public override BoundNode VisitCall(BoundCall node)
        {
            VisitCall(node.Method, null, node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt, node.DefaultArguments, node);
            CheckReceiverIfField(node.ReceiverOpt);
            CheckReferenceToMethodIfLocalFunction(node, node.Method);
            return base.VisitCall(node);
        }

        private void CheckOutDeclaration(BoundLocal local)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsOutVariable, local);
            }
        }

        private void CheckDiscard(BoundDiscardExpression argument)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDiscard, argument);
            }
        }

        public override BoundNode VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            if (_inExpressionLambda && node.AddMethod.IsStatic)
            {
                Error(ErrorCode.ERR_ExtensionCollectionElementInitializerInExpressionTree, node);
            }
            VisitCall(node.AddMethod, null, node.Arguments, default(ImmutableArray<RefKind>), default(ImmutableArray<string>), node.DefaultArguments, node);
            return base.VisitCollectionElementInitializer(node);
        }

        public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            VisitCall(node.Constructor, null, node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt, node.DefaultArguments, node);
            return base.VisitObjectCreationExpression(node);
        }

        public override BoundNode VisitIndexerAccess(BoundIndexerAccess node)
        {
            PropertySymbol indexer = node.Indexer;
            MethodSymbol methodSymbol = indexer.GetOwnOrInheritedGetMethod() ?? indexer.GetOwnOrInheritedSetMethod();
            if ((object)methodSymbol != null)
            {
                VisitCall(methodSymbol, indexer, node.Arguments, node.ArgumentRefKindsOpt, node.ArgumentNamesOpt, node.DefaultArguments, node);
            }
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitIndexerAccess(node);
        }

        private void CheckRefReturningPropertyAccess(BoundNode node, PropertySymbol property)
        {
            if (_inExpressionLambda && property.RefKind != 0)
            {
                Error(ErrorCode.ERR_RefReturningCallInExpressionTree, node);
            }
        }

        public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
        {
            PropertySymbol propertySymbol = node.PropertySymbol;
            CheckRefReturningPropertyAccess(node, propertySymbol);
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitPropertyAccess(node);
        }

        public override BoundNode VisitLambda(BoundLambda node)
        {
            if (_inExpressionLambda)
            {
                ImmutableArray<ParameterSymbol>.Enumerator enumerator = node.Symbol.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ParameterSymbol current = enumerator.Current;
                    if (current.RefKind != 0 && current.Locations.Length != 0)
                    {
                        _diagnostics.Add(ErrorCode.ERR_ByRefParameterInExpressionTree, current.Locations[0]);
                    }
                    if (current.TypeWithAnnotations.IsRestrictedType())
                    {
                        _diagnostics.Add(ErrorCode.ERR_ExpressionTreeCantContainRefStruct, current.Locations[0], current.Type.Name);
                    }
                }
                switch (node.Syntax.Kind())
                {
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        {
                            ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax = (ParenthesizedLambdaExpressionSyntax)node.Syntax;
                            if (parenthesizedLambdaExpressionSyntax.AsyncKeyword.Kind() == SyntaxKind.AsyncKeyword)
                            {
                                Error(ErrorCode.ERR_BadAsyncExpressionTree, node);
                            }
                            else if (parenthesizedLambdaExpressionSyntax.Body.Kind() == SyntaxKind.Block)
                            {
                                Error(ErrorCode.ERR_StatementLambdaToExpressionTree, node);
                            }
                            else if (parenthesizedLambdaExpressionSyntax.Body.Kind() == SyntaxKind.RefExpression)
                            {
                                Error(ErrorCode.ERR_BadRefReturnExpressionTree, node);
                            }
                            break;
                        }
                    case SyntaxKind.SimpleLambdaExpression:
                        {
                            SimpleLambdaExpressionSyntax simpleLambdaExpressionSyntax = (SimpleLambdaExpressionSyntax)node.Syntax;
                            if (simpleLambdaExpressionSyntax.AsyncKeyword.Kind() == SyntaxKind.AsyncKeyword)
                            {
                                Error(ErrorCode.ERR_BadAsyncExpressionTree, node);
                            }
                            else if (simpleLambdaExpressionSyntax.Body.Kind() == SyntaxKind.Block)
                            {
                                Error(ErrorCode.ERR_StatementLambdaToExpressionTree, node);
                            }
                            else if (simpleLambdaExpressionSyntax.Body.Kind() == SyntaxKind.RefExpression)
                            {
                                Error(ErrorCode.ERR_BadRefReturnExpressionTree, node);
                            }
                            break;
                        }
                    case SyntaxKind.AnonymousMethodExpression:
                        Error(ErrorCode.ERR_ExpressionTreeContainsAnonymousMethod, node);
                        break;
                }
            }
            SourceMethodSymbol staticLocalOrAnonymousFunction = _staticLocalOrAnonymousFunction;
            if (node.Symbol.IsStatic)
            {
                _staticLocalOrAnonymousFunction = node.Symbol;
            }
            BoundNode? result = base.VisitLambda(node);
            _staticLocalOrAnonymousFunction = staticLocalOrAnonymousFunction;
            return result;
        }

        public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
        {
            BoundBinaryOperator boundBinaryOperator = node;
            while (true)
            {
                CheckBinaryOperator(boundBinaryOperator);
                Visit(boundBinaryOperator.Right);
                if (boundBinaryOperator.Left.Kind != BoundKind.BinaryOperator)
                {
                    break;
                }
                boundBinaryOperator = (BoundBinaryOperator)boundBinaryOperator.Left;
            }
            Visit(boundBinaryOperator.Left);
            return null;
        }

        public override BoundNode VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            CheckLiftedUserDefinedConditionalLogicalOperator(node);
            return base.VisitUserDefinedConditionalLogicalOperator(node);
        }

        private void CheckDynamic(BoundUnaryOperator node)
        {
            if (_inExpressionLambda && node.OperatorKind.IsDynamic())
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
        }

        private void CheckDynamic(BoundBinaryOperator node)
        {
            if (_inExpressionLambda && node.OperatorKind.IsDynamic())
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
        }

        public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
        {
            CheckUnsafeType(node);
            CheckLiftedUnaryOp(node);
            CheckDynamic(node);
            return base.VisitUnaryOperator(node);
        }

        public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            CheckUnsafeType(node);
            BoundExpression operand = node.Operand;
            if (operand.Kind == BoundKind.FieldAccess)
            {
                CheckFieldAddress((BoundFieldAccess)operand, null);
            }
            return base.VisitAddressOfOperator(node);
        }

        public override BoundNode VisitIncrementOperator(BoundIncrementOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsAssignment, node);
            }
            return base.VisitIncrementOperator(node);
        }

        public override BoundNode VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            NoteUnsafe(node);
            return base.VisitPointerElementAccess(node);
        }

        public override BoundNode VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            NoteUnsafe(node);
            return base.VisitPointerIndirectionOperator(node);
        }

        public override BoundNode VisitConversion(BoundConversion node)
        {
            CheckUnsafeType(node.Operand);
            CheckUnsafeType(node);
            bool inExpressionLambda = _inExpressionLambda;
            bool reportedUnsafe = _reportedUnsafe;
            switch (node.ConversionKind)
            {
                case ConversionKind.MethodGroup:
                    CheckMethodGroup((BoundMethodGroup)node.Operand, node.Conversion.Method, parentIsConversion: true, node.Type);
                    return node;
                case ConversionKind.AnonymousFunction:
                    if (!inExpressionLambda && node.Type.IsExpressionTree())
                    {
                        _inExpressionLambda = true;
                        _reportedUnsafe = false;
                    }
                    break;
                case ConversionKind.ImplicitDynamic:
                case ConversionKind.ExplicitDynamic:
                    if (_inExpressionLambda)
                    {
                        Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
                    }
                    break;
                case ConversionKind.ImplicitTupleLiteral:
                case ConversionKind.ImplicitTuple:
                case ConversionKind.ExplicitTupleLiteral:
                case ConversionKind.ExplicitTuple:
                    if (_inExpressionLambda)
                    {
                        Error(ErrorCode.ERR_ExpressionTreeContainsTupleConversion, node);
                    }
                    break;
            }
            BoundNode? result = base.VisitConversion(node);
            _inExpressionLambda = inExpressionLambda;
            _reportedUnsafe = reportedUnsafe;
            return result;
        }

        public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            if (node.Argument.Kind != BoundKind.MethodGroup)
            {
                Visit(node.Argument);
            }
            else
            {
                CheckMethodGroup((BoundMethodGroup)node.Argument, node.MethodOpt, parentIsConversion: true, node.Type);
            }
            return null;
        }

        public override BoundNode VisitMethodGroup(BoundMethodGroup node)
        {
            CheckMethodGroup(node, null, parentIsConversion: false, null);
            return null;
        }

        private void CheckMethodGroup(BoundMethodGroup node, MethodSymbol method, bool parentIsConversion, TypeSymbol convertedToType)
        {
            if (_inExpressionLambda)
            {
                MethodSymbol obj = node.LookupSymbolOpt as MethodSymbol;
                if ((object)obj != null && obj.MethodKind == MethodKind.LocalFunction)
                {
                    Error(ErrorCode.ERR_ExpressionTreeContainsLocalFunction, node);
                }
                else if (parentIsConversion && convertedToType.IsFunctionPointer())
                {
                    Error(ErrorCode.ERR_AddressOfMethodGroupInExpressionTree, node);
                }
            }
            CheckReceiverIfField(node.ReceiverOpt);
            CheckReferenceToMethodIfLocalFunction(node, method);
            if ((object)method == null || method.RequiresInstanceReceiver)
            {
                Visit(node.ReceiverOpt);
            }
        }

        public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
        {
            return node;
        }

        public override BoundNode VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            if (_inExpressionLambda && (node.LeftOperand.IsLiteralNull() || node.LeftOperand.IsLiteralDefault()))
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsBadCoalesce, node.LeftOperand);
            }
            return base.VisitNullCoalescingOperator(node);
        }

        public override BoundNode VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeCantContainNullCoalescingAssignment, node);
            }
            return base.VisitNullCoalescingAssignmentOperator(node);
        }

        public override BoundNode VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
                if (node.Expression.Kind == BoundKind.MethodGroup)
                {
                    return base.VisitMethodGroup((BoundMethodGroup)node.Expression);
                }
            }
            return base.VisitDynamicInvocation(node);
        }

        public override BoundNode VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
            CheckReceiverIfField(node.Receiver);
            return base.VisitDynamicIndexerAccess(node);
        }

        public override BoundNode VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
            return base.VisitDynamicMemberAccess(node);
        }

        public override BoundNode VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
            return base.VisitDynamicCollectionElementInitializer(node);
        }

        public override BoundNode VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsDynamicOperation, node);
            }
            return base.VisitDynamicObjectCreationExpression(node);
        }

        public override BoundNode VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsIsMatch, node);
            }
            return base.VisitIsPatternExpression(node);
        }

        public override BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsTupleLiteral, node);
            }
            return base.VisitConvertedTupleLiteral(node);
        }

        public override BoundNode VisitTupleLiteral(BoundTupleLiteral node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsTupleLiteral, node);
            }
            return base.VisitTupleLiteral(node);
        }

        public override BoundNode VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsTupleBinOp, node);
            }
            return base.VisitTupleBinaryOperator(node);
        }

        public override BoundNode VisitThrowExpression(BoundThrowExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsThrowExpression, node);
            }
            return base.VisitThrowExpression(node);
        }

        public override BoundNode VisitWithExpression(BoundWithExpression node)
        {
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsWithExpression, node);
            }
            return base.VisitWithExpression(node);
        }

        private void CheckArguments(ImmutableArray<RefKind> argumentRefKindsOpt, ImmutableArray<BoundExpression> arguments, Symbol method)
        {
            if (argumentRefKindsOpt.IsDefault)
            {
                return;
            }
            for (int i = 0; i < arguments.Length; i++)
            {
                if (argumentRefKindsOpt[i] == RefKind.None)
                {
                    continue;
                }
                BoundExpression boundExpression = arguments[i];
                switch (boundExpression.Kind)
                {
                    case BoundKind.FieldAccess:
                        CheckFieldAddress((BoundFieldAccess)boundExpression, method);
                        break;
                    case BoundKind.Local:
                        {
                            BoundLocal boundLocal = (BoundLocal)boundExpression;
                            if (boundLocal.Syntax.Kind() == SyntaxKind.DeclarationExpression)
                            {
                                CheckOutDeclaration(boundLocal);
                            }
                            break;
                        }
                    case BoundKind.DiscardExpression:
                        CheckDiscard((BoundDiscardExpression)boundExpression);
                        break;
                }
            }
        }

        private void CheckFieldAddress(BoundFieldAccess fieldAccess, Symbol consumerOpt)
        {
            FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
            if (fieldSymbol.IsVolatile && ((object)consumerOpt == null || !IsInterlockedAPI(consumerOpt)))
            {
                Error(ErrorCode.WRN_VolatileByRef, fieldAccess, fieldSymbol);
            }
            if (IsNonAgileFieldAccess(fieldAccess, _compilation))
            {
                Error(ErrorCode.WRN_ByRefNonAgileField, fieldAccess, fieldSymbol);
            }
        }

        private void CheckFieldAsReceiver(BoundFieldAccess fieldAccess)
        {
            FieldSymbol fieldSymbol = fieldAccess.FieldSymbol;
            if (IsNonAgileFieldAccess(fieldAccess, _compilation) && !fieldSymbol.Type.IsReferenceType)
            {
                Error(ErrorCode.WRN_CallOnNonAgileField, fieldAccess, fieldSymbol);
            }
        }

        private void CheckReceiverIfField(BoundExpression receiverOpt)
        {
            if (receiverOpt != null && receiverOpt.Kind == BoundKind.FieldAccess)
            {
                CheckFieldAsReceiver((BoundFieldAccess)receiverOpt);
            }
        }

        internal static bool IsNonAgileFieldAccess(BoundFieldAccess fieldAccess, CSharpCompilation compilation)
        {
            if (IsInstanceFieldAccessWithNonThisReceiver(fieldAccess))
            {
                NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(WellKnownType.System_MarshalByRefObject);
                TypeSymbol typeSymbol = fieldAccess.FieldSymbol.ContainingType;
                while ((object)typeSymbol != null)
                {
                    if (TypeSymbol.Equals(typeSymbol, wellKnownType, TypeCompareKind.ConsiderEverything))
                    {
                        return true;
                    }
                    typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
                }
            }
            return false;
        }

        private static bool IsInstanceFieldAccessWithNonThisReceiver(BoundFieldAccess fieldAccess)
        {
            BoundExpression boundExpression = fieldAccess.ReceiverOpt;
            if (boundExpression == null || fieldAccess.FieldSymbol.IsStatic)
            {
                return false;
            }
            while (boundExpression.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)boundExpression;
                if (boundConversion.ExplicitCastInCode)
                {
                    break;
                }
                boundExpression = boundConversion.Operand;
            }
            if (boundExpression.Kind != BoundKind.ThisReference)
            {
                return boundExpression.Kind != BoundKind.BaseReference;
            }
            return false;
        }

        private bool IsInterlockedAPI(Symbol method)
        {
            NamedTypeSymbol wellKnownType = _compilation.GetWellKnownType(WellKnownType.System_Threading_Interlocked);
            if ((object)wellKnownType != null && TypeSymbol.Equals(wellKnownType, method.ContainingType, TypeCompareKind.ConsiderEverything))
            {
                return true;
            }
            return false;
        }

        private static BoundExpression StripImplicitCasts(BoundExpression expr)
        {
            BoundExpression boundExpression = expr;
            while (boundExpression is BoundConversion boundConversion && boundConversion.ConversionKind.IsImplicitConversion())
            {
                boundExpression = boundConversion.Operand;
            }
            return boundExpression;
        }

        private static bool IsSameLocalOrField(BoundExpression expr1, BoundExpression expr2)
        {
            if (expr1 == null && expr2 == null)
            {
                return true;
            }
            if (expr1 == null || expr2 == null)
            {
                return false;
            }
            if (expr1.HasAnyErrors || expr2.HasAnyErrors)
            {
                return false;
            }
            expr1 = StripImplicitCasts(expr1);
            expr2 = StripImplicitCasts(expr2);
            if (expr1.Kind != expr2.Kind)
            {
                return false;
            }
            switch (expr1.Kind)
            {
                case BoundKind.Local:
                    {
                        BoundLocal obj3 = (BoundLocal)expr1;
                        BoundLocal boundLocal = (BoundLocal)expr2;
                        return obj3.LocalSymbol == boundLocal.LocalSymbol;
                    }
                case BoundKind.FieldAccess:
                    {
                        BoundFieldAccess boundFieldAccess = (BoundFieldAccess)expr1;
                        BoundFieldAccess boundFieldAccess2 = (BoundFieldAccess)expr2;
                        if (boundFieldAccess.FieldSymbol == boundFieldAccess2.FieldSymbol)
                        {
                            if (!boundFieldAccess.FieldSymbol.IsStatic)
                            {
                                return IsSameLocalOrField(boundFieldAccess.ReceiverOpt, boundFieldAccess2.ReceiverOpt);
                            }
                            return true;
                        }
                        return false;
                    }
                case BoundKind.EventAccess:
                    {
                        BoundEventAccess boundEventAccess = (BoundEventAccess)expr1;
                        BoundEventAccess boundEventAccess2 = (BoundEventAccess)expr2;
                        if (boundEventAccess.EventSymbol == boundEventAccess2.EventSymbol)
                        {
                            if (!boundEventAccess.EventSymbol.IsStatic)
                            {
                                return IsSameLocalOrField(boundEventAccess.ReceiverOpt, boundEventAccess2.ReceiverOpt);
                            }
                            return true;
                        }
                        return false;
                    }
                case BoundKind.Parameter:
                    {
                        BoundParameter obj2 = (BoundParameter)expr1;
                        BoundParameter boundParameter = (BoundParameter)expr2;
                        return obj2.ParameterSymbol == boundParameter.ParameterSymbol;
                    }
                case BoundKind.RangeVariable:
                    {
                        BoundRangeVariable obj = (BoundRangeVariable)expr1;
                        BoundRangeVariable boundRangeVariable = (BoundRangeVariable)expr2;
                        return obj.RangeVariableSymbol == boundRangeVariable.RangeVariableSymbol;
                    }
                case BoundKind.ThisReference:
                case BoundKind.PreviousSubmissionReference:
                case BoundKind.HostObjectMemberReference:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsComCallWithRefOmitted(MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<RefKind> argumentRefKindsOpt)
        {
            if (method.ParameterCount != arguments.Length || (object)method.ContainingType == null || !method.ContainingType.IsComImport)
            {
                return false;
            }
            for (int i = 0; i < arguments.Length; i++)
            {
                if (method.Parameters[i].RefKind != 0 && (argumentRefKindsOpt.IsDefault || argumentRefKindsOpt[i] == RefKind.None))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckBinaryOperator(BoundBinaryOperator node)
        {
            if ((object)node.MethodOpt == null)
            {
                CheckUnsafeType(node.Left);
                CheckUnsafeType(node.Right);
            }
            CheckForBitwiseOrSignExtend(node, node.OperatorKind, node.Left, node.Right);
            CheckNullableNullBinOp(node);
            CheckLiftedBinOp(node);
            CheckRelationals(node);
            CheckDynamic(node);
        }

        private void CheckCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            BoundExpression boundExpression = node.Left;
            if (!node.Operator.Kind.IsDynamic() && !node.LeftConversion.IsIdentity && node.LeftConversion.Exists)
            {
                boundExpression = new BoundConversion(boundExpression.Syntax, boundExpression, node.LeftConversion, node.Operator.Kind.IsChecked(), explicitCastInCode: false, null, null, node.Operator.LeftType);
            }
            CheckForBitwiseOrSignExtend(node, node.Operator.Kind, boundExpression, node.Right);
            CheckLiftedCompoundAssignment(node);
            if (_inExpressionLambda)
            {
                Error(ErrorCode.ERR_ExpressionTreeContainsAssignment, node);
            }
        }

        private void CheckRelationals(BoundBinaryOperator node)
        {
            if (!node.OperatorKind.IsComparison())
            {
                return;
            }
            if (node.Left.ConstantValue != null && node.Right.ConstantValue == null && node.Right.Kind == BoundKind.Conversion)
            {
                CheckVacuousComparisons(node, node.Left.ConstantValue, node.Right);
            }
            if (node.Right.ConstantValue != null && node.Left.ConstantValue == null && node.Left.Kind == BoundKind.Conversion)
            {
                CheckVacuousComparisons(node, node.Right.ConstantValue, node.Left);
            }
            if (node.OperatorKind == BinaryOperatorKind.ObjectEqual || node.OperatorKind == BinaryOperatorKind.ObjectNotEqual)
            {
                if (node.Left.Type!.SpecialType == SpecialType.System_Object && !IsExplicitCast(node.Left) && (!(node.Left.ConstantValue != null) || !node.Left.ConstantValue!.IsNull) && ConvertedHasEqual(node.OperatorKind, node.Right, out var type))
                {
                    _diagnostics.Add(ErrorCode.WRN_BadRefCompareLeft, node.Syntax.Location, type);
                }
                else if (node.Right.Type!.SpecialType == SpecialType.System_Object && !IsExplicitCast(node.Right) && (!(node.Right.ConstantValue != null) || !node.Right.ConstantValue!.IsNull) && ConvertedHasEqual(node.OperatorKind, node.Left, out type))
                {
                    _diagnostics.Add(ErrorCode.WRN_BadRefCompareRight, node.Syntax.Location, type);
                }
            }
            CheckSelfComparisons(node);
        }

        private static bool IsExplicitCast(BoundExpression node)
        {
            if (node.Kind == BoundKind.Conversion)
            {
                return ((BoundConversion)node).ExplicitCastInCode;
            }
            return false;
        }

        private static bool ConvertedHasEqual(BinaryOperatorKind oldOperatorKind, BoundNode node, out TypeSymbol type)
        {
            type = null;
            if (node.Kind != BoundKind.Conversion)
            {
                return false;
            }
            BoundConversion boundConversion = (BoundConversion)node;
            if (boundConversion.ExplicitCastInCode)
            {
                return false;
            }
            if (!(boundConversion.Operand.Type is NamedTypeSymbol namedTypeSymbol) || !namedTypeSymbol.IsReferenceType || namedTypeSymbol.IsInterface)
            {
                return false;
            }
            string name = ((oldOperatorKind == BinaryOperatorKind.ObjectEqual) ? "op_Equality" : "op_Inequality");
            NamedTypeSymbol namedTypeSymbol2 = namedTypeSymbol;
            while ((object)namedTypeSymbol2 != null)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol2.GetMembers(name).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.UserDefinedOperator)
                    {
                        ImmutableArray<ParameterSymbol> parameters = methodSymbol.GetParameters();
                        if (parameters.Length == 2 && TypeSymbol.Equals(parameters[0].Type, namedTypeSymbol2, TypeCompareKind.ConsiderEverything) && TypeSymbol.Equals(parameters[1].Type, namedTypeSymbol2, TypeCompareKind.ConsiderEverything))
                        {
                            type = namedTypeSymbol2;
                            return true;
                        }
                    }
                }
                namedTypeSymbol2 = namedTypeSymbol2.BaseTypeNoUseSiteDiagnostics;
            }
            return false;
        }

        private void CheckSelfComparisons(BoundBinaryOperator node)
        {
            if (!node.HasAnyErrors && IsSameLocalOrField(node.Left, node.Right))
            {
                Error(ErrorCode.WRN_ComparisonToSelf, node);
            }
        }

        private void CheckVacuousComparisons(BoundBinaryOperator tree, ConstantValue constantValue, BoundNode operand)
        {
            BoundConversion boundConversion = operand as BoundConversion;
            while (boundConversion != null && (boundConversion.ConversionKind == ConversionKind.ImplicitNumeric || boundConversion.ConversionKind == ConversionKind.ImplicitConstant) && !boundConversion.ExplicitCastInCode && boundConversion.Operand.Type!.SpecialType.IsIntegralType() && boundConversion.Type.SpecialType.IsIntegralType())
            {
                if (!Binder.CheckConstantBounds(boundConversion.Operand.Type!.SpecialType, constantValue, out var _))
                {
                    Error(ErrorCode.WRN_VacuousIntegralComp, tree, boundConversion.Operand.Type);
                    break;
                }
                boundConversion = boundConversion.Operand as BoundConversion;
            }
        }

        private void CheckForBitwiseOrSignExtend(BoundExpression node, BinaryOperatorKind operatorKind, BoundExpression leftOperand, BoundExpression rightOperand)
        {
            if (((uint)(operatorKind - 7941) > 3u && (uint)(operatorKind - 73477) > 3u) || node.ConstantValue != null)
            {
                return;
            }
            ulong num = FindSurprisingSignExtensionBits(leftOperand);
            ulong num2 = FindSurprisingSignExtensionBits(rightOperand);
            if (num == num2)
            {
                return;
            }
            ConstantValue constantValueForBitwiseOrCheck = GetConstantValueForBitwiseOrCheck(leftOperand);
            if (constantValueForBitwiseOrCheck != null)
            {
                ulong uInt64Value = constantValueForBitwiseOrCheck.UInt64Value;
                if ((uInt64Value & num2) == num2 || (~uInt64Value & num2) == num2)
                {
                    return;
                }
            }
            constantValueForBitwiseOrCheck = GetConstantValueForBitwiseOrCheck(rightOperand);
            if (constantValueForBitwiseOrCheck != null)
            {
                ulong uInt64Value2 = constantValueForBitwiseOrCheck.UInt64Value;
                if ((uInt64Value2 & num) == num || (~uInt64Value2 & num) == num)
                {
                    return;
                }
            }
            Error(ErrorCode.WRN_BitwiseOrSignExtend, node);
        }

        private static ConstantValue GetConstantValueForBitwiseOrCheck(BoundExpression operand)
        {
            if (operand.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)operand;
                if (boundConversion.ConversionKind == ConversionKind.ImplicitNullable)
                {
                    operand = boundConversion.Operand;
                }
            }
            ConstantValue constantValue = operand.ConstantValue;
            if (constantValue == null || !constantValue.IsIntegral)
            {
                return null;
            }
            return constantValue;
        }

        private static ulong FindSurprisingSignExtensionBits(BoundExpression expr)
        {
            if (expr.Kind != BoundKind.Conversion)
            {
                return 0uL;
            }
            BoundConversion boundConversion = (BoundConversion)expr;
            TypeSymbol typeSymbol = boundConversion.Operand.Type;
            TypeSymbol typeSymbol2 = boundConversion.Type;
            if ((object)typeSymbol == null || (object)typeSymbol2 == null)
            {
                return 0uL;
            }
            if (typeSymbol.IsNullableType())
            {
                typeSymbol = typeSymbol.GetNullableUnderlyingType();
            }
            if (typeSymbol2.IsNullableType())
            {
                typeSymbol2 = typeSymbol2.GetNullableUnderlyingType();
            }
            SpecialType specialType = typeSymbol.SpecialType;
            SpecialType specialType2 = typeSymbol2.SpecialType;
            if (!specialType.IsIntegralType() || !specialType2.IsIntegralType())
            {
                return 0uL;
            }
            int num = specialType.SizeInBytes();
            int num2 = specialType2.SizeInBytes();
            if (num == 0 || num2 == 0)
            {
                return 0uL;
            }
            ulong num3 = FindSurprisingSignExtensionBits(boundConversion.Operand);
            if (num == num2)
            {
                return num3;
            }
            if (num2 < num)
            {
                return num2 switch
                {
                    1 => (byte)num3,
                    2 => (ushort)num3,
                    4 => (uint)num3,
                    _ => num3,
                };
            }
            if (!specialType.IsSignedIntegralType())
            {
                return num3;
            }
            if (boundConversion.ExplicitCastInCode && specialType2.IsSignedIntegralType())
            {
                return num3;
            }
            ulong num4 = num3;
            for (int i = num; i < num2; i++)
            {
                num4 |= (ulong)(255L << i * 8);
            }
            return num4;
        }

        private void CheckLiftedCompoundAssignment(BoundCompoundAssignmentOperator node)
        {
            if (node.Operator.Kind.IsLifted() && node.Right.NullableNeverHasValue())
            {
                Error(ErrorCode.WRN_AlwaysNull, node, node.Type);
            }
        }

        private void CheckLiftedUnaryOp(BoundUnaryOperator node)
        {
            if (node.OperatorKind.IsLifted() && node.Operand.NullableNeverHasValue())
            {
                Error(ErrorCode.WRN_AlwaysNull, node, node.Type);
            }
        }

        private void CheckNullableNullBinOp(BoundBinaryOperator node)
        {
            if (node.OperatorKind.OperandTypes() != BinaryOperatorKind.NullableNull)
            {
                return;
            }
            BinaryOperatorKind binaryOperatorKind = node.OperatorKind.Operator();
            if (binaryOperatorKind == BinaryOperatorKind.Equal || binaryOperatorKind == BinaryOperatorKind.NotEqual)
            {
                string text = ((node.OperatorKind.Operator() == BinaryOperatorKind.NotEqual) ? "true" : "false");
                if (node.Right.IsLiteralNull() && node.Left.NullableAlwaysHasValue())
                {
                    Error(ErrorCode.WRN_NubExprIsConstBool, node, text, node.Left.Type.GetNullableUnderlyingType(), node.Left.Type);
                }
                else if (node.Left.IsLiteralNull() && node.Right.NullableAlwaysHasValue())
                {
                    Error(ErrorCode.WRN_NubExprIsConstBool, node, text, node.Right.Type.GetNullableUnderlyingType(), node.Right.Type);
                }
            }
        }

        private void CheckLiftedBinOp(BoundBinaryOperator node)
        {
            if (!node.OperatorKind.IsLifted())
            {
                return;
            }
            switch (node.OperatorKind.Operator())
            {
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThanOrEqual:
                    if (node.Right.NullableNeverHasValue())
                    {
                        Error(ErrorCode.WRN_CmpAlwaysFalse, node, GetTypeForLiftedComparisonWarning(node.Right));
                    }
                    else if (node.Left.NullableNeverHasValue())
                    {
                        Error(ErrorCode.WRN_CmpAlwaysFalse, node, GetTypeForLiftedComparisonWarning(node.Left));
                    }
                    break;
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                    {
                        string text = ((node.OperatorKind.Operator() == BinaryOperatorKind.NotEqual) ? "true" : "false");
                        if (node.Right.NullableNeverHasValue() && node.Left.NullableAlwaysHasValue())
                        {
                            Error(node.OperatorKind.IsUserDefined() ? ErrorCode.WRN_NubExprIsConstBool2 : ErrorCode.WRN_NubExprIsConstBool, node, text, node.Left.Type.GetNullableUnderlyingType(), GetTypeForLiftedComparisonWarning(node.Right));
                        }
                        else if (node.Left.NullableNeverHasValue() && node.Right.NullableAlwaysHasValue())
                        {
                            Error(node.OperatorKind.IsUserDefined() ? ErrorCode.WRN_NubExprIsConstBool2 : ErrorCode.WRN_NubExprIsConstBool, node, text, node.Right.Type.GetNullableUnderlyingType(), GetTypeForLiftedComparisonWarning(node.Left));
                        }
                        break;
                    }
                case BinaryOperatorKind.And:
                case BinaryOperatorKind.Or:
                    if ((node.Left.NullableNeverHasValue() && node.Right.IsNullableNonBoolean()) || (node.Left.IsNullableNonBoolean() && node.Right.NullableNeverHasValue()))
                    {
                        Error(ErrorCode.WRN_AlwaysNull, node, node.Type);
                    }
                    break;
                default:
                    if (node.Right.NullableNeverHasValue() || node.Left.NullableNeverHasValue())
                    {
                        Error(ErrorCode.WRN_AlwaysNull, node, node.Type);
                    }
                    break;
            }
        }

        private void CheckLiftedUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            if (node.Right.NullableNeverHasValue() || node.Left.NullableNeverHasValue())
            {
                Error(ErrorCode.WRN_AlwaysNull, node, node.Type);
            }
        }

        private static TypeSymbol GetTypeForLiftedComparisonWarning(BoundExpression node)
        {
            if ((object)node.Type == null || !node.Type.IsNullableType())
            {
                return null;
            }
            TypeSymbol typeSymbol = null;
            if (node.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)node;
                if (boundConversion.ConversionKind == ConversionKind.ExplicitNullable || boundConversion.ConversionKind == ConversionKind.ImplicitNullable)
                {
                    typeSymbol = GetTypeForLiftedComparisonWarning(boundConversion.Operand);
                }
            }
            return typeSymbol ?? node.Type;
        }

        private bool CheckForAssignmentToSelf(BoundAssignmentOperator node)
        {
            if (!node.HasAnyErrors && IsSameLocalOrField(node.Left, node.Right))
            {
                Error(ErrorCode.WRN_AssignmentToSelf, node);
                return true;
            }
            return false;
        }

        private void CheckForDeconstructionAssignmentToSelf(BoundTupleExpression leftTuple, BoundExpression right)
        {
            while (right.Kind == BoundKind.Conversion)
            {
                BoundConversion boundConversion = (BoundConversion)right;
                ConversionKind conversionKind = boundConversion.ConversionKind;
                if (conversionKind == ConversionKind.Identity || conversionKind == ConversionKind.ImplicitTupleLiteral || conversionKind == ConversionKind.Deconstruction)
                {
                    right = boundConversion.Operand;
                    continue;
                }
                return;
            }
            if (right.Kind != BoundKind.ConvertedTupleLiteral && right.Kind != BoundKind.TupleLiteral)
            {
                return;
            }
            BoundTupleExpression boundTupleExpression = (BoundTupleExpression)right;
            ImmutableArray<BoundExpression> arguments = leftTuple.Arguments;
            int length = arguments.Length;
            for (int i = 0; i < length; i++)
            {
                BoundExpression boundExpression = arguments[i];
                BoundExpression boundExpression2 = boundTupleExpression.Arguments[i];
                if (boundExpression is BoundTupleExpression leftTuple2)
                {
                    CheckForDeconstructionAssignmentToSelf(leftTuple2, boundExpression2);
                }
                else if (IsSameLocalOrField(boundExpression, boundExpression2))
                {
                    Error(ErrorCode.WRN_AssignmentToSelf, boundExpression);
                }
            }
        }

        public override BoundNode VisitFieldAccess(BoundFieldAccess node)
        {
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitFieldAccess(node);
        }

        public override BoundNode VisitPropertyGroup(BoundPropertyGroup node)
        {
            CheckReceiverIfField(node.ReceiverOpt);
            return base.VisitPropertyGroup(node);
        }
    }
}
