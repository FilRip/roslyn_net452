using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class NullabilityRewriter : BoundTreeRewriter
    {
        private readonly ImmutableDictionary<BoundExpression, (NullabilityInfo Info, TypeSymbol? Type)> _updatedNullabilities;

        private readonly NullableWalker.SnapshotManager? _snapshotManager;

        private readonly ImmutableDictionary<Symbol, Symbol>.Builder _remappedSymbols;

        protected override BoundExpression? VisitExpressionWithoutStackGuard(BoundExpression node)
        {
            return (BoundExpression)Visit(node);
        }

        public override BoundNode? VisitBinaryOperator(BoundBinaryOperator node)
        {
            return VisitBinaryOperatorBase(node);
        }

        public override BoundNode? VisitUserDefinedConditionalLogicalOperator(BoundUserDefinedConditionalLogicalOperator node)
        {
            return VisitBinaryOperatorBase(node);
        }

        private BoundNode VisitBinaryOperatorBase(BoundBinaryOperatorBase binaryOperator)
        {
            ArrayBuilder<BoundBinaryOperatorBase> instance = ArrayBuilder<BoundBinaryOperatorBase>.GetInstance();
            BoundBinaryOperatorBase boundBinaryOperatorBase = binaryOperator;
            do
            {
                instance.Push(boundBinaryOperatorBase);
                boundBinaryOperatorBase = boundBinaryOperatorBase.Left as BoundBinaryOperatorBase;
            }
            while (boundBinaryOperatorBase != null);
            BoundExpression left = (BoundExpression)Visit(instance.Peek().Left);
            do
            {
                boundBinaryOperatorBase = instance.Pop();
                bool num = _updatedNullabilities.TryGetValue(boundBinaryOperatorBase, out (NullabilityInfo, TypeSymbol) value);
                BoundExpression right = (BoundExpression)Visit(boundBinaryOperatorBase.Right);
                TypeSymbol type = (num ? value.Item2 : boundBinaryOperatorBase.Type);
                BoundBinaryOperatorBase boundBinaryOperatorBase2;
                if (!(boundBinaryOperatorBase is BoundBinaryOperator boundBinaryOperator))
                {
                    if (!(boundBinaryOperatorBase is BoundUserDefinedConditionalLogicalOperator boundUserDefinedConditionalLogicalOperator))
                    {
                        throw ExceptionUtilities.UnexpectedValue(boundBinaryOperatorBase.Kind);
                    }
                    boundBinaryOperatorBase2 = boundUserDefinedConditionalLogicalOperator.Update(boundUserDefinedConditionalLogicalOperator.OperatorKind, boundUserDefinedConditionalLogicalOperator.LogicalOperator, boundUserDefinedConditionalLogicalOperator.TrueOperator, boundUserDefinedConditionalLogicalOperator.FalseOperator, boundUserDefinedConditionalLogicalOperator.ResultKind, boundUserDefinedConditionalLogicalOperator.OriginalUserDefinedOperatorsOpt, left, right, type);
                }
                else
                {
                    boundBinaryOperatorBase2 = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundBinaryOperator.ConstantValueOpt, GetUpdatedSymbol(boundBinaryOperator, boundBinaryOperator.MethodOpt), boundBinaryOperator.ResultKind, boundBinaryOperator.OriginalUserDefinedOperatorsOpt, left, right, type);
                }
                boundBinaryOperatorBase = boundBinaryOperatorBase2;
                if (num)
                {
                    (boundBinaryOperatorBase.TopLevelNullability, _) = value;
                }
                left = boundBinaryOperatorBase;
            }
            while (instance.Count > 0);
            return boundBinaryOperatorBase;
        }

        private T GetUpdatedSymbol<T>(BoundNode expr, T sym) where T : Symbol?
        {
            if (sym == null)
            {
                return sym;
            }
            NullableWalker.SnapshotManager? snapshotManager = _snapshotManager;
            if (snapshotManager == null || !snapshotManager!.TryGetUpdatedSymbol(expr, sym, out Symbol updatedSymbol))
            {
                updatedSymbol = sym;
            }
            if (!(updatedSymbol is LambdaSymbol lambda2))
            {
                if (!(updatedSymbol is SourceLocalSymbol local2))
                {
                    if (updatedSymbol is ParameterSymbol key && _remappedSymbols.TryGetValue(key, out var value))
                    {
                        return (T)value;
                    }
                    return (T)updatedSymbol;
                }
                return (T)remapLocal(local2);
            }
            return (T)remapLambda((BoundLambda)expr, lambda2);
            Symbol remapLambda(BoundLambda boundLambda, LambdaSymbol lambda)
            {
                NamedTypeSymbol namedTypeSymbol = _snapshotManager?.GetUpdatedDelegateTypeForLambda(lambda);
                if (!_remappedSymbols.TryGetValue(lambda.ContainingSymbol, out var value4) && (object)namedTypeSymbol == null)
                {
                    return lambda;
                }
                LambdaSymbol lambdaSymbol = (((object)namedTypeSymbol != null) ? boundLambda.CreateLambdaSymbol(namedTypeSymbol, value4 ?? lambda.ContainingSymbol) : boundLambda.CreateLambdaSymbol(value4, lambda.ReturnTypeWithAnnotations, lambda.ParameterTypesWithAnnotations, lambda.ParameterRefKinds, lambda.RefKind));
                _remappedSymbols.Add(lambda, lambdaSymbol);
                for (int i = 0; i < lambda.ParameterCount; i++)
                {
                    _remappedSymbols.Add(lambda.Parameters[i], lambdaSymbol.Parameters[i]);
                }
                return lambdaSymbol;
            }
            Symbol remapLocal(SourceLocalSymbol local)
            {
                if (_remappedSymbols.TryGetValue(local, out var value2))
                {
                    return value2;
                }
                TypeWithAnnotations? typeWithAnnotations = _snapshotManager?.GetUpdatedTypeForLocalSymbol(local);
                if (!_remappedSymbols.TryGetValue(local.ContainingSymbol, out var value3) && !typeWithAnnotations.HasValue)
                {
                    _remappedSymbols.Add(local, local);
                    return local;
                }
                value2 = new UpdatedContainingSymbolAndNullableAnnotationLocal(local, value3 ?? local.ContainingSymbol, typeWithAnnotations ?? local.TypeWithAnnotations);
                _remappedSymbols.Add(local, value2);
                return value2;
            }
        }

        private ImmutableArray<T> GetUpdatedArray<T>(BoundNode expr, ImmutableArray<T> symbols) where T : Symbol?
        {
            if (symbols.IsDefaultOrEmpty)
            {
                return symbols;
            }
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance(symbols.Length);
            bool flag = false;
            ImmutableArray<T>.Enumerator enumerator = symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                T val = null;
                if (current != null)
                {
                    val = GetUpdatedSymbol(expr, current);
                    if (current != val)
                    {
                        flag = true;
                    }
                }
                instance.Add(val);
            }
            if (flag)
            {
                return instance.ToImmutableAndFree();
            }
            instance.Free();
            return symbols;
        }

        public NullabilityRewriter(ImmutableDictionary<BoundExpression, (NullabilityInfo Info, TypeSymbol? Type)> updatedNullabilities, NullableWalker.SnapshotManager? snapshotManager, ImmutableDictionary<Symbol, Symbol>.Builder remappedSymbols)
        {
            _updatedNullabilities = updatedNullabilities;
            _snapshotManager = snapshotManager;
            _remappedSymbols = remappedSymbols;
        }

        public override BoundNode? VisitFieldEqualsValue(BoundFieldEqualsValue node)
        {
            FieldSymbol updatedSymbol = GetUpdatedSymbol(node, node.Field);
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            return node.Update(updatedSymbol, updatedArray, value);
        }

        public override BoundNode? VisitPropertyEqualsValue(BoundPropertyEqualsValue node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.Property);
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            return node.Update(updatedSymbol, updatedArray, value);
        }

        public override BoundNode? VisitParameterEqualsValue(BoundParameterEqualsValue node)
        {
            ParameterSymbol updatedSymbol = GetUpdatedSymbol(node, node.Parameter);
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            return node.Update(updatedSymbol, updatedArray, value);
        }

        public override BoundNode? VisitDeconstructValuePlaceholder(BoundDeconstructValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundDeconstructValuePlaceholder boundDeconstructValuePlaceholder = node.Update(node.ValEscape, value.Item2);
            (boundDeconstructValuePlaceholder.TopLevelNullability, _) = value;
            return boundDeconstructValuePlaceholder;
        }

        public override BoundNode? VisitTupleOperandPlaceholder(BoundTupleOperandPlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundTupleOperandPlaceholder boundTupleOperandPlaceholder = node.Update(value.Item2);
            (boundTupleOperandPlaceholder.TopLevelNullability, _) = value;
            return boundTupleOperandPlaceholder;
        }

        public override BoundNode? VisitAwaitableValuePlaceholder(BoundAwaitableValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundAwaitableValuePlaceholder boundAwaitableValuePlaceholder = node.Update(node.ValEscape, value.Item2);
            (boundAwaitableValuePlaceholder.TopLevelNullability, _) = value;
            return boundAwaitableValuePlaceholder;
        }

        public override BoundNode? VisitDisposableValuePlaceholder(BoundDisposableValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundDisposableValuePlaceholder boundDisposableValuePlaceholder = node.Update(value.Item2);
            (boundDisposableValuePlaceholder.TopLevelNullability, _) = value;
            return boundDisposableValuePlaceholder;
        }

        public override BoundNode? VisitObjectOrCollectionValuePlaceholder(BoundObjectOrCollectionValuePlaceholder node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundObjectOrCollectionValuePlaceholder boundObjectOrCollectionValuePlaceholder = node.Update(node.IsNewInstance, value.Item2);
            (boundObjectOrCollectionValuePlaceholder.TopLevelNullability, _) = value;
            return boundObjectOrCollectionValuePlaceholder;
        }

        public override BoundNode? VisitDup(BoundDup node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundDup boundDup = node.Update(node.RefKind, value.Item2);
            (boundDup.TopLevelNullability, _) = value;
            return boundDup;
        }

        public override BoundNode? VisitPassByCopy(BoundPassByCopy node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundPassByCopy boundPassByCopy;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundPassByCopy = node.Update(expression, value.Item2);
                (boundPassByCopy.TopLevelNullability, _) = value;
            }
            else
            {
                boundPassByCopy = node.Update(expression, node.Type);
            }
            return boundPassByCopy;
        }

        public override BoundNode? VisitBadExpression(BoundBadExpression node)
        {
            GetUpdatedArray(node, node.Symbols);
            ImmutableArray<BoundExpression> childBoundNodes = VisitList(node.ChildBoundNodes);
            BoundBadExpression boundBadExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundBadExpression = node.Update(node.ResultKind, node.Symbols, childBoundNodes, value.Item2);
                (boundBadExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundBadExpression = node.Update(node.ResultKind, node.Symbols, childBoundNodes, node.Type);
            }
            return boundBadExpression;
        }

        public override BoundNode? VisitTypeExpression(BoundTypeExpression node)
        {
            BoundTypeExpression boundContainingTypeOpt = (BoundTypeExpression)Visit(node.BoundContainingTypeOpt);
            ImmutableArray<BoundExpression> boundDimensionsOpt = VisitList(node.BoundDimensionsOpt);
            BoundTypeExpression boundTypeExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundTypeExpression = node.Update(node.AliasOpt, boundContainingTypeOpt, boundDimensionsOpt, node.TypeWithAnnotations, value.Item2);
                (boundTypeExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundTypeExpression = node.Update(node.AliasOpt, boundContainingTypeOpt, boundDimensionsOpt, node.TypeWithAnnotations, node.Type);
            }
            return boundTypeExpression;
        }

        public override BoundNode? VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundTypeOrValueExpression boundTypeOrValueExpression = node.Update(node.Data, value.Item2);
            (boundTypeOrValueExpression.TopLevelNullability, _) = value;
            return boundTypeOrValueExpression;
        }

        public override BoundNode? VisitNamespaceExpression(BoundNamespaceExpression node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundNamespaceExpression boundNamespaceExpression = node.Update(node.NamespaceSymbol, node.AliasOpt);
            (boundNamespaceExpression.TopLevelNullability, _) = value;
            return boundNamespaceExpression;
        }

        public override BoundNode? VisitUnaryOperator(BoundUnaryOperator node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.MethodOpt);
            GetUpdatedArray(node, node.OriginalUserDefinedOperatorsOpt);
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundUnaryOperator boundUnaryOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundUnaryOperator = node.Update(node.OperatorKind, operand, node.ConstantValueOpt, updatedSymbol, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, value.Item2);
                (boundUnaryOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundUnaryOperator = node.Update(node.OperatorKind, operand, node.ConstantValueOpt, updatedSymbol, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, node.Type);
            }
            return boundUnaryOperator;
        }

        public override BoundNode? VisitIncrementOperator(BoundIncrementOperator node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.MethodOpt);
            GetUpdatedArray(node, node.OriginalUserDefinedOperatorsOpt);
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundIncrementOperator boundIncrementOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundIncrementOperator = node.Update(node.OperatorKind, operand, updatedSymbol, node.OperandConversion, node.ResultConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, value.Item2);
                (boundIncrementOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundIncrementOperator = node.Update(node.OperatorKind, operand, updatedSymbol, node.OperandConversion, node.ResultConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, node.Type);
            }
            return boundIncrementOperator;
        }

        public override BoundNode? VisitAddressOfOperator(BoundAddressOfOperator node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundAddressOfOperator boundAddressOfOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAddressOfOperator = node.Update(operand, node.IsManaged, value.Item2);
                (boundAddressOfOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundAddressOfOperator = node.Update(operand, node.IsManaged, node.Type);
            }
            return boundAddressOfOperator;
        }

        public override BoundNode? VisitUnconvertedAddressOfOperator(BoundUnconvertedAddressOfOperator node)
        {
            BoundMethodGroup operand = (BoundMethodGroup)Visit(node.Operand);
            BoundUnconvertedAddressOfOperator boundUnconvertedAddressOfOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundUnconvertedAddressOfOperator = node.Update(operand);
                (boundUnconvertedAddressOfOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundUnconvertedAddressOfOperator = node.Update(operand);
            }
            return boundUnconvertedAddressOfOperator;
        }

        public override BoundNode? VisitFunctionPointerLoad(BoundFunctionPointerLoad node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.TargetMethod);
            BoundFunctionPointerLoad boundFunctionPointerLoad;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundFunctionPointerLoad = node.Update(updatedSymbol, value.Item2);
                (boundFunctionPointerLoad.TopLevelNullability, _) = value;
            }
            else
            {
                boundFunctionPointerLoad = node.Update(updatedSymbol, node.Type);
            }
            return boundFunctionPointerLoad;
        }

        public override BoundNode? VisitPointerIndirectionOperator(BoundPointerIndirectionOperator node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundPointerIndirectionOperator boundPointerIndirectionOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundPointerIndirectionOperator = node.Update(operand, value.Item2);
                (boundPointerIndirectionOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundPointerIndirectionOperator = node.Update(operand, node.Type);
            }
            return boundPointerIndirectionOperator;
        }

        public override BoundNode? VisitPointerElementAccess(BoundPointerElementAccess node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundExpression index = (BoundExpression)Visit(node.Index);
            BoundPointerElementAccess boundPointerElementAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundPointerElementAccess = node.Update(expression, index, node.Checked, value.Item2);
                (boundPointerElementAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundPointerElementAccess = node.Update(expression, index, node.Checked, node.Type);
            }
            return boundPointerElementAccess;
        }

        public override BoundNode? VisitFunctionPointerInvocation(BoundFunctionPointerInvocation node)
        {
            BoundExpression invokedExpression = (BoundExpression)Visit(node.InvokedExpression);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundFunctionPointerInvocation boundFunctionPointerInvocation;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundFunctionPointerInvocation = node.Update(invokedExpression, arguments, node.ArgumentRefKindsOpt, node.ResultKind, value.Item2);
                (boundFunctionPointerInvocation.TopLevelNullability, _) = value;
            }
            else
            {
                boundFunctionPointerInvocation = node.Update(invokedExpression, arguments, node.ArgumentRefKindsOpt, node.ResultKind, node.Type);
            }
            return boundFunctionPointerInvocation;
        }

        public override BoundNode? VisitRefTypeOperator(BoundRefTypeOperator node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.GetTypeFromHandle);
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundRefTypeOperator boundRefTypeOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundRefTypeOperator = node.Update(operand, updatedSymbol, value.Item2);
                (boundRefTypeOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundRefTypeOperator = node.Update(operand, updatedSymbol, node.Type);
            }
            return boundRefTypeOperator;
        }

        public override BoundNode? VisitMakeRefOperator(BoundMakeRefOperator node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundMakeRefOperator boundMakeRefOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundMakeRefOperator = node.Update(operand, value.Item2);
                (boundMakeRefOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundMakeRefOperator = node.Update(operand, node.Type);
            }
            return boundMakeRefOperator;
        }

        public override BoundNode? VisitRefValueOperator(BoundRefValueOperator node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundRefValueOperator boundRefValueOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundRefValueOperator = node.Update(node.NullableAnnotation, operand, value.Item2);
                (boundRefValueOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundRefValueOperator = node.Update(node.NullableAnnotation, operand, node.Type);
            }
            return boundRefValueOperator;
        }

        public override BoundNode? VisitFromEndIndexExpression(BoundFromEndIndexExpression node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.MethodOpt);
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundFromEndIndexExpression boundFromEndIndexExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundFromEndIndexExpression = node.Update(operand, updatedSymbol, value.Item2);
                (boundFromEndIndexExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundFromEndIndexExpression = node.Update(operand, updatedSymbol, node.Type);
            }
            return boundFromEndIndexExpression;
        }

        public override BoundNode? VisitRangeExpression(BoundRangeExpression node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.MethodOpt);
            BoundExpression leftOperandOpt = (BoundExpression)Visit(node.LeftOperandOpt);
            BoundExpression rightOperandOpt = (BoundExpression)Visit(node.RightOperandOpt);
            BoundRangeExpression boundRangeExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundRangeExpression = node.Update(leftOperandOpt, rightOperandOpt, updatedSymbol, value.Item2);
                (boundRangeExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundRangeExpression = node.Update(leftOperandOpt, rightOperandOpt, updatedSymbol, node.Type);
            }
            return boundRangeExpression;
        }

        public override BoundNode? VisitTupleBinaryOperator(BoundTupleBinaryOperator node)
        {
            BoundExpression left = (BoundExpression)Visit(node.Left);
            BoundExpression right = (BoundExpression)Visit(node.Right);
            BoundTupleBinaryOperator boundTupleBinaryOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundTupleBinaryOperator = node.Update(left, right, node.OperatorKind, node.Operators, value.Item2);
                (boundTupleBinaryOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundTupleBinaryOperator = node.Update(left, right, node.OperatorKind, node.Operators, node.Type);
            }
            return boundTupleBinaryOperator;
        }

        public override BoundNode? VisitCompoundAssignmentOperator(BoundCompoundAssignmentOperator node)
        {
            GetUpdatedArray(node, node.OriginalUserDefinedOperatorsOpt);
            BoundExpression left = (BoundExpression)Visit(node.Left);
            BoundExpression right = (BoundExpression)Visit(node.Right);
            BoundCompoundAssignmentOperator boundCompoundAssignmentOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundCompoundAssignmentOperator = node.Update(node.Operator, left, right, node.LeftConversion, node.FinalConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, value.Item2);
                (boundCompoundAssignmentOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundCompoundAssignmentOperator = node.Update(node.Operator, left, right, node.LeftConversion, node.FinalConversion, node.ResultKind, node.OriginalUserDefinedOperatorsOpt, node.Type);
            }
            return boundCompoundAssignmentOperator;
        }

        public override BoundNode? VisitAssignmentOperator(BoundAssignmentOperator node)
        {
            BoundExpression left = (BoundExpression)Visit(node.Left);
            BoundExpression right = (BoundExpression)Visit(node.Right);
            BoundAssignmentOperator boundAssignmentOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAssignmentOperator = node.Update(left, right, node.IsRef, value.Item2);
                (boundAssignmentOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundAssignmentOperator = node.Update(left, right, node.IsRef, node.Type);
            }
            return boundAssignmentOperator;
        }

        public override BoundNode? VisitDeconstructionAssignmentOperator(BoundDeconstructionAssignmentOperator node)
        {
            BoundTupleExpression left = (BoundTupleExpression)Visit(node.Left);
            BoundConversion right = (BoundConversion)Visit(node.Right);
            BoundDeconstructionAssignmentOperator boundDeconstructionAssignmentOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDeconstructionAssignmentOperator = node.Update(left, right, node.IsUsed, value.Item2);
                (boundDeconstructionAssignmentOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundDeconstructionAssignmentOperator = node.Update(left, right, node.IsUsed, node.Type);
            }
            return boundDeconstructionAssignmentOperator;
        }

        public override BoundNode? VisitNullCoalescingOperator(BoundNullCoalescingOperator node)
        {
            BoundExpression leftOperand = (BoundExpression)Visit(node.LeftOperand);
            BoundExpression rightOperand = (BoundExpression)Visit(node.RightOperand);
            BoundNullCoalescingOperator boundNullCoalescingOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundNullCoalescingOperator = node.Update(leftOperand, rightOperand, node.LeftConversion, node.OperatorResultKind, value.Item2);
                (boundNullCoalescingOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundNullCoalescingOperator = node.Update(leftOperand, rightOperand, node.LeftConversion, node.OperatorResultKind, node.Type);
            }
            return boundNullCoalescingOperator;
        }

        public override BoundNode? VisitNullCoalescingAssignmentOperator(BoundNullCoalescingAssignmentOperator node)
        {
            BoundExpression leftOperand = (BoundExpression)Visit(node.LeftOperand);
            BoundExpression rightOperand = (BoundExpression)Visit(node.RightOperand);
            BoundNullCoalescingAssignmentOperator boundNullCoalescingAssignmentOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundNullCoalescingAssignmentOperator = node.Update(leftOperand, rightOperand, value.Item2);
                (boundNullCoalescingAssignmentOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundNullCoalescingAssignmentOperator = node.Update(leftOperand, rightOperand, node.Type);
            }
            return boundNullCoalescingAssignmentOperator;
        }

        public override BoundNode? VisitUnconvertedConditionalOperator(BoundUnconvertedConditionalOperator node)
        {
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundExpression consequence = (BoundExpression)Visit(node.Consequence);
            BoundExpression alternative = (BoundExpression)Visit(node.Alternative);
            BoundUnconvertedConditionalOperator boundUnconvertedConditionalOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundUnconvertedConditionalOperator = node.Update(condition, consequence, alternative, node.ConstantValueOpt, node.NoCommonTypeError, value.Item2);
                (boundUnconvertedConditionalOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundUnconvertedConditionalOperator = node.Update(condition, consequence, alternative, node.ConstantValueOpt, node.NoCommonTypeError, node.Type);
            }
            return boundUnconvertedConditionalOperator;
        }

        public override BoundNode? VisitConditionalOperator(BoundConditionalOperator node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.NaturalTypeOpt);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundExpression consequence = (BoundExpression)Visit(node.Consequence);
            BoundExpression alternative = (BoundExpression)Visit(node.Alternative);
            BoundConditionalOperator boundConditionalOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundConditionalOperator = node.Update(node.IsRef, condition, consequence, alternative, node.ConstantValueOpt, updatedSymbol, node.WasTargetTyped, value.Item2);
                (boundConditionalOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundConditionalOperator = node.Update(node.IsRef, condition, consequence, alternative, node.ConstantValueOpt, updatedSymbol, node.WasTargetTyped, node.Type);
            }
            return boundConditionalOperator;
        }

        public override BoundNode? VisitArrayAccess(BoundArrayAccess node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            ImmutableArray<BoundExpression> indices = VisitList(node.Indices);
            BoundArrayAccess boundArrayAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundArrayAccess = node.Update(expression, indices, value.Item2);
                (boundArrayAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundArrayAccess = node.Update(expression, indices, node.Type);
            }
            return boundArrayAccess;
        }

        public override BoundNode? VisitArrayLength(BoundArrayLength node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundArrayLength boundArrayLength;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundArrayLength = node.Update(expression, value.Item2);
                (boundArrayLength.TopLevelNullability, _) = value;
            }
            else
            {
                boundArrayLength = node.Update(expression, node.Type);
            }
            return boundArrayLength;
        }

        public override BoundNode? VisitAwaitableInfo(BoundAwaitableInfo node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.IsCompleted);
            MethodSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.GetResult);
            BoundAwaitableValuePlaceholder awaitableInstancePlaceholder = (BoundAwaitableValuePlaceholder)Visit(node.AwaitableInstancePlaceholder);
            BoundExpression getAwaiter = (BoundExpression)Visit(node.GetAwaiter);
            return node.Update(awaitableInstancePlaceholder, node.IsDynamic, getAwaiter, updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitAwaitExpression(BoundAwaitExpression node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundAwaitableInfo awaitableInfo = (BoundAwaitableInfo)Visit(node.AwaitableInfo);
            BoundAwaitExpression boundAwaitExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAwaitExpression = node.Update(expression, awaitableInfo, value.Item2);
                (boundAwaitExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundAwaitExpression = node.Update(expression, awaitableInfo, node.Type);
            }
            return boundAwaitExpression;
        }

        public override BoundNode? VisitTypeOfOperator(BoundTypeOfOperator node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.GetTypeFromHandle);
            BoundTypeExpression sourceType = (BoundTypeExpression)Visit(node.SourceType);
            BoundTypeOfOperator boundTypeOfOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundTypeOfOperator = node.Update(sourceType, updatedSymbol, value.Item2);
                (boundTypeOfOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundTypeOfOperator = node.Update(sourceType, updatedSymbol, node.Type);
            }
            return boundTypeOfOperator;
        }

        public override BoundNode? VisitMethodDefIndex(BoundMethodDefIndex node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.Method);
            BoundMethodDefIndex boundMethodDefIndex;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundMethodDefIndex = node.Update(updatedSymbol, value.Item2);
                (boundMethodDefIndex.TopLevelNullability, _) = value;
            }
            else
            {
                boundMethodDefIndex = node.Update(updatedSymbol, node.Type);
            }
            return boundMethodDefIndex;
        }

        public override BoundNode? VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundMaximumMethodDefIndex boundMaximumMethodDefIndex = node.Update(value.Item2);
            (boundMaximumMethodDefIndex.TopLevelNullability, _) = value;
            return boundMaximumMethodDefIndex;
        }

        public override BoundNode? VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundInstrumentationPayloadRoot boundInstrumentationPayloadRoot = node.Update(node.AnalysisKind, value.Item2);
            (boundInstrumentationPayloadRoot.TopLevelNullability, _) = value;
            return boundInstrumentationPayloadRoot;
        }

        public override BoundNode? VisitModuleVersionId(BoundModuleVersionId node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundModuleVersionId boundModuleVersionId = node.Update(value.Item2);
            (boundModuleVersionId.TopLevelNullability, _) = value;
            return boundModuleVersionId;
        }

        public override BoundNode? VisitModuleVersionIdString(BoundModuleVersionIdString node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundModuleVersionIdString boundModuleVersionIdString = node.Update(value.Item2);
            (boundModuleVersionIdString.TopLevelNullability, _) = value;
            return boundModuleVersionIdString;
        }

        public override BoundNode? VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundSourceDocumentIndex boundSourceDocumentIndex = node.Update(node.Document, value.Item2);
            (boundSourceDocumentIndex.TopLevelNullability, _) = value;
            return boundSourceDocumentIndex;
        }

        public override BoundNode? VisitMethodInfo(BoundMethodInfo node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.Method);
            MethodSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.GetMethodFromHandle);
            BoundMethodInfo boundMethodInfo;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundMethodInfo = node.Update(updatedSymbol, updatedSymbol2, value.Item2);
                (boundMethodInfo.TopLevelNullability, _) = value;
            }
            else
            {
                boundMethodInfo = node.Update(updatedSymbol, updatedSymbol2, node.Type);
            }
            return boundMethodInfo;
        }

        public override BoundNode? VisitFieldInfo(BoundFieldInfo node)
        {
            FieldSymbol updatedSymbol = GetUpdatedSymbol(node, node.Field);
            MethodSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.GetFieldFromHandle);
            BoundFieldInfo boundFieldInfo;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundFieldInfo = node.Update(updatedSymbol, updatedSymbol2, value.Item2);
                (boundFieldInfo.TopLevelNullability, _) = value;
            }
            else
            {
                boundFieldInfo = node.Update(updatedSymbol, updatedSymbol2, node.Type);
            }
            return boundFieldInfo;
        }

        public override BoundNode? VisitDefaultLiteral(BoundDefaultLiteral node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundDefaultLiteral boundDefaultLiteral = node.Update();
            (boundDefaultLiteral.TopLevelNullability, _) = value;
            return boundDefaultLiteral;
        }

        public override BoundNode? VisitDefaultExpression(BoundDefaultExpression node)
        {
            BoundTypeExpression targetType = node.TargetType;
            BoundDefaultExpression boundDefaultExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDefaultExpression = node.Update(targetType, node.ConstantValueOpt, value.Item2);
                (boundDefaultExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundDefaultExpression = node.Update(targetType, node.ConstantValueOpt, node.Type);
            }
            return boundDefaultExpression;
        }

        public override BoundNode? VisitIsOperator(BoundIsOperator node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundTypeExpression targetType = (BoundTypeExpression)Visit(node.TargetType);
            BoundIsOperator boundIsOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundIsOperator = node.Update(operand, targetType, node.Conversion, value.Item2);
                (boundIsOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundIsOperator = node.Update(operand, targetType, node.Conversion, node.Type);
            }
            return boundIsOperator;
        }

        public override BoundNode? VisitAsOperator(BoundAsOperator node)
        {
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundTypeExpression targetType = (BoundTypeExpression)Visit(node.TargetType);
            BoundAsOperator boundAsOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAsOperator = node.Update(operand, targetType, node.Conversion, value.Item2);
                (boundAsOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundAsOperator = node.Update(operand, targetType, node.Conversion, node.Type);
            }
            return boundAsOperator;
        }

        public override BoundNode? VisitSizeOfOperator(BoundSizeOfOperator node)
        {
            BoundTypeExpression sourceType = (BoundTypeExpression)Visit(node.SourceType);
            BoundSizeOfOperator boundSizeOfOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundSizeOfOperator = node.Update(sourceType, node.ConstantValueOpt, value.Item2);
                (boundSizeOfOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundSizeOfOperator = node.Update(sourceType, node.ConstantValueOpt, node.Type);
            }
            return boundSizeOfOperator;
        }

        public override BoundNode? VisitConversion(BoundConversion node)
        {
            GetUpdatedArray(node, node.OriginalUserDefinedConversionsOpt);
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundConversion boundConversion;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundConversion = node.Update(operand, node.Conversion, node.IsBaseConversion, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ConversionGroupOpt, node.OriginalUserDefinedConversionsOpt, value.Item2);
                (boundConversion.TopLevelNullability, _) = value;
            }
            else
            {
                boundConversion = node.Update(operand, node.Conversion, node.IsBaseConversion, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ConversionGroupOpt, node.OriginalUserDefinedConversionsOpt, node.Type);
            }
            return boundConversion;
        }

        public override BoundNode? VisitReadOnlySpanFromArray(BoundReadOnlySpanFromArray node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.ConversionMethod);
            BoundExpression operand = (BoundExpression)Visit(node.Operand);
            BoundReadOnlySpanFromArray boundReadOnlySpanFromArray;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundReadOnlySpanFromArray = node.Update(operand, updatedSymbol, value.Item2);
                (boundReadOnlySpanFromArray.TopLevelNullability, _) = value;
            }
            else
            {
                boundReadOnlySpanFromArray = node.Update(operand, updatedSymbol, node.Type);
            }
            return boundReadOnlySpanFromArray;
        }

        public override BoundNode? VisitArgList(BoundArgList node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundArgList boundArgList = node.Update(value.Item2);
            (boundArgList.TopLevelNullability, _) = value;
            return boundArgList;
        }

        public override BoundNode? VisitArgListOperator(BoundArgListOperator node)
        {
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundArgListOperator boundArgListOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundArgListOperator = node.Update(arguments, node.ArgumentRefKindsOpt, value.Item2);
                (boundArgListOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundArgListOperator = node.Update(arguments, node.ArgumentRefKindsOpt, node.Type);
            }
            return boundArgListOperator;
        }

        public override BoundNode? VisitFixedLocalCollectionInitializer(BoundFixedLocalCollectionInitializer node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.ElementPointerType);
            MethodSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.GetPinnableOpt);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundFixedLocalCollectionInitializer boundFixedLocalCollectionInitializer;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundFixedLocalCollectionInitializer = node.Update(updatedSymbol, node.ElementPointerTypeConversion, expression, updatedSymbol2, value.Item2);
                (boundFixedLocalCollectionInitializer.TopLevelNullability, _) = value;
            }
            else
            {
                boundFixedLocalCollectionInitializer = node.Update(updatedSymbol, node.ElementPointerTypeConversion, expression, updatedSymbol2, node.Type);
            }
            return boundFixedLocalCollectionInitializer;
        }

        public override BoundNode? VisitBlock(BoundBlock node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            ImmutableArray<LocalFunctionSymbol> updatedArray2 = GetUpdatedArray(node, node.LocalFunctions);
            ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
            return node.Update(updatedArray, updatedArray2, statements);
        }

        public override BoundNode? VisitScope(BoundScope node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
            return node.Update(updatedArray, statements);
        }

        public override BoundNode? VisitStateMachineScope(BoundStateMachineScope node)
        {
            ImmutableArray<StateMachineFieldSymbol> updatedArray = GetUpdatedArray(node, node.Fields);
            BoundStatement statement = (BoundStatement)Visit(node.Statement);
            return node.Update(updatedArray, statement);
        }

        public override BoundNode? VisitLocalDeclaration(BoundLocalDeclaration node)
        {
            LocalSymbol updatedSymbol = GetUpdatedSymbol(node, node.LocalSymbol);
            BoundTypeExpression declaredTypeOpt = (BoundTypeExpression)Visit(node.DeclaredTypeOpt);
            BoundExpression initializerOpt = (BoundExpression)Visit(node.InitializerOpt);
            ImmutableArray<BoundExpression> argumentsOpt = VisitList(node.ArgumentsOpt);
            return node.Update(updatedSymbol, declaredTypeOpt, initializerOpt, argumentsOpt, node.InferredType);
        }

        public override BoundNode? VisitLocalFunctionStatement(BoundLocalFunctionStatement node)
        {
            LocalFunctionSymbol updatedSymbol = GetUpdatedSymbol(node, node.Symbol);
            BoundBlock blockBody = (BoundBlock)Visit(node.BlockBody);
            BoundBlock expressionBody = (BoundBlock)Visit(node.ExpressionBody);
            return node.Update(updatedSymbol, blockBody, expressionBody);
        }

        public override BoundNode? VisitSwitchStatement(BoundSwitchStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.InnerLocals);
            ImmutableArray<LocalFunctionSymbol> updatedArray2 = GetUpdatedArray(node, node.InnerLocalFunctions);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            ImmutableArray<BoundSwitchSection> switchSections = VisitList(node.SwitchSections);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundSwitchLabel defaultLabel = (BoundSwitchLabel)Visit(node.DefaultLabel);
            return node.Update(expression, updatedArray, updatedArray2, switchSections, decisionDag, defaultLabel, node.BreakLabel);
        }

        public override BoundNode? VisitSwitchDispatch(BoundSwitchDispatch node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.EqualityMethod);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            return node.Update(expression, node.Cases, node.DefaultLabel, updatedSymbol);
        }

        public override BoundNode? VisitDoStatement(BoundDoStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(updatedArray, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitWhileStatement(BoundWhileStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(updatedArray, condition, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitForStatement(BoundForStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.OuterLocals);
            ImmutableArray<LocalSymbol> updatedArray2 = GetUpdatedArray(node, node.InnerLocals);
            BoundStatement initializer = (BoundStatement)Visit(node.Initializer);
            BoundExpression condition = (BoundExpression)Visit(node.Condition);
            BoundStatement increment = (BoundStatement)Visit(node.Increment);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(updatedArray, initializer, updatedArray2, condition, increment, body, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitForEachStatement(BoundForEachStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.IterationVariables);
            BoundTypeExpression iterationVariableType = (BoundTypeExpression)Visit(node.IterationVariableType);
            BoundExpression iterationErrorExpressionOpt = (BoundExpression)Visit(node.IterationErrorExpressionOpt);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundForEachDeconstructStep deconstructionOpt = (BoundForEachDeconstructStep)Visit(node.DeconstructionOpt);
            BoundAwaitableInfo awaitOpt = (BoundAwaitableInfo)Visit(node.AwaitOpt);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(node.EnumeratorInfoOpt, node.ElementConversion, iterationVariableType, updatedArray, iterationErrorExpressionOpt, expression, deconstructionOpt, awaitOpt, body, node.Checked, node.BreakLabel, node.ContinueLabel);
        }

        public override BoundNode? VisitUsingStatement(BoundUsingStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundMultipleLocalDeclarations declarationsOpt = (BoundMultipleLocalDeclarations)Visit(node.DeclarationsOpt);
            BoundExpression expressionOpt = (BoundExpression)Visit(node.ExpressionOpt);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            BoundAwaitableInfo awaitOpt = (BoundAwaitableInfo)Visit(node.AwaitOpt);
            return node.Update(updatedArray, declarationsOpt, expressionOpt, node.IDisposableConversion, body, awaitOpt, node.PatternDisposeInfoOpt);
        }

        public override BoundNode? VisitFixedStatement(BoundFixedStatement node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundMultipleLocalDeclarations declarations = (BoundMultipleLocalDeclarations)Visit(node.Declarations);
            BoundStatement body = (BoundStatement)Visit(node.Body);
            return node.Update(updatedArray, declarations, body);
        }

        public override BoundNode? VisitCatchBlock(BoundCatchBlock node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.ExceptionTypeOpt);
            BoundExpression exceptionSourceOpt = (BoundExpression)Visit(node.ExceptionSourceOpt);
            BoundStatementList exceptionFilterPrologueOpt = (BoundStatementList)Visit(node.ExceptionFilterPrologueOpt);
            BoundExpression exceptionFilterOpt = (BoundExpression)Visit(node.ExceptionFilterOpt);
            BoundBlock body = (BoundBlock)Visit(node.Body);
            return node.Update(updatedArray, exceptionSourceOpt, updatedSymbol, exceptionFilterPrologueOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
        }

        public override BoundNode? VisitLiteral(BoundLiteral node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundLiteral boundLiteral = node.Update(node.ConstantValueOpt, value.Item2);
            (boundLiteral.TopLevelNullability, _) = value;
            return boundLiteral;
        }

        public override BoundNode? VisitThisReference(BoundThisReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundThisReference boundThisReference = node.Update(value.Item2);
            (boundThisReference.TopLevelNullability, _) = value;
            return boundThisReference;
        }

        public override BoundNode? VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundPreviousSubmissionReference boundPreviousSubmissionReference = node.Update(value.Item2);
            (boundPreviousSubmissionReference.TopLevelNullability, _) = value;
            return boundPreviousSubmissionReference;
        }

        public override BoundNode? VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundHostObjectMemberReference boundHostObjectMemberReference = node.Update(value.Item2);
            (boundHostObjectMemberReference.TopLevelNullability, _) = value;
            return boundHostObjectMemberReference;
        }

        public override BoundNode? VisitBaseReference(BoundBaseReference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundBaseReference boundBaseReference = node.Update(value.Item2);
            (boundBaseReference.TopLevelNullability, _) = value;
            return boundBaseReference;
        }

        public override BoundNode? VisitLocal(BoundLocal node)
        {
            LocalSymbol updatedSymbol = GetUpdatedSymbol(node, node.LocalSymbol);
            BoundLocal boundLocal;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundLocal = node.Update(updatedSymbol, node.DeclarationKind, node.ConstantValueOpt, node.IsNullableUnknown, value.Item2);
                (boundLocal.TopLevelNullability, _) = value;
            }
            else
            {
                boundLocal = node.Update(updatedSymbol, node.DeclarationKind, node.ConstantValueOpt, node.IsNullableUnknown, node.Type);
            }
            return boundLocal;
        }

        public override BoundNode? VisitPseudoVariable(BoundPseudoVariable node)
        {
            LocalSymbol updatedSymbol = GetUpdatedSymbol(node, node.LocalSymbol);
            BoundPseudoVariable boundPseudoVariable;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundPseudoVariable = node.Update(updatedSymbol, node.EmitExpressions, value.Item2);
                (boundPseudoVariable.TopLevelNullability, _) = value;
            }
            else
            {
                boundPseudoVariable = node.Update(updatedSymbol, node.EmitExpressions, node.Type);
            }
            return boundPseudoVariable;
        }

        public override BoundNode? VisitRangeVariable(BoundRangeVariable node)
        {
            RangeVariableSymbol updatedSymbol = GetUpdatedSymbol(node, node.RangeVariableSymbol);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            BoundRangeVariable boundRangeVariable;
            if (_updatedNullabilities.TryGetValue(node, out var value2))
            {
                boundRangeVariable = node.Update(updatedSymbol, value, value2.Item2);
                (boundRangeVariable.TopLevelNullability, _) = value2;
            }
            else
            {
                boundRangeVariable = node.Update(updatedSymbol, value, node.Type);
            }
            return boundRangeVariable;
        }

        public override BoundNode? VisitParameter(BoundParameter node)
        {
            ParameterSymbol updatedSymbol = GetUpdatedSymbol(node, node.ParameterSymbol);
            BoundParameter boundParameter;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundParameter = node.Update(updatedSymbol, value.Item2);
                (boundParameter.TopLevelNullability, _) = value;
            }
            else
            {
                boundParameter = node.Update(updatedSymbol, node.Type);
            }
            return boundParameter;
        }

        public override BoundNode? VisitLabel(BoundLabel node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundLabel boundLabel = node.Update(node.Label, value.Item2);
            (boundLabel.TopLevelNullability, _) = value;
            return boundLabel;
        }

        public override BoundNode? VisitSwitchExpressionArm(BoundSwitchExpressionArm node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundPattern pattern = (BoundPattern)Visit(node.Pattern);
            BoundExpression whenClause = (BoundExpression)Visit(node.WhenClause);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            return node.Update(updatedArray, pattern, whenClause, value, node.Label);
        }

        public override BoundNode? VisitUnconvertedSwitchExpression(BoundUnconvertedSwitchExpression node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            ImmutableArray<BoundSwitchExpressionArm> switchArms = VisitList(node.SwitchArms);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundUnconvertedSwitchExpression boundUnconvertedSwitchExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundUnconvertedSwitchExpression = node.Update(expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, value.Item2);
                (boundUnconvertedSwitchExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundUnconvertedSwitchExpression = node.Update(expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, node.Type);
            }
            return boundUnconvertedSwitchExpression;
        }

        public override BoundNode? VisitConvertedSwitchExpression(BoundConvertedSwitchExpression node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.NaturalTypeOpt);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            ImmutableArray<BoundSwitchExpressionArm> switchArms = VisitList(node.SwitchArms);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundConvertedSwitchExpression boundConvertedSwitchExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundConvertedSwitchExpression = node.Update(updatedSymbol, node.WasTargetTyped, node.Conversion, expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, value.Item2);
                (boundConvertedSwitchExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundConvertedSwitchExpression = node.Update(updatedSymbol, node.WasTargetTyped, node.Conversion, expression, switchArms, decisionDag, node.DefaultLabel, node.ReportedNotExhaustive, node.Type);
            }
            return boundConvertedSwitchExpression;
        }

        public override BoundNode? VisitDagDeconstructEvaluation(BoundDagDeconstructEvaluation node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.DeconstructMethod);
            BoundDagTemp input = (BoundDagTemp)Visit(node.Input);
            return node.Update(updatedSymbol, input);
        }

        public override BoundNode? VisitDagFieldEvaluation(BoundDagFieldEvaluation node)
        {
            FieldSymbol updatedSymbol = GetUpdatedSymbol(node, node.Field);
            BoundDagTemp input = (BoundDagTemp)Visit(node.Input);
            return node.Update(updatedSymbol, input);
        }

        public override BoundNode? VisitDagPropertyEvaluation(BoundDagPropertyEvaluation node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.Property);
            BoundDagTemp input = (BoundDagTemp)Visit(node.Input);
            return node.Update(updatedSymbol, input);
        }

        public override BoundNode? VisitDagIndexEvaluation(BoundDagIndexEvaluation node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.Property);
            BoundDagTemp input = (BoundDagTemp)Visit(node.Input);
            return node.Update(updatedSymbol, node.Index, input);
        }

        public override BoundNode? VisitSwitchSection(BoundSwitchSection node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundSwitchLabel> switchLabels = VisitList(node.SwitchLabels);
            ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
            return node.Update(updatedArray, switchLabels, statements);
        }

        public override BoundNode? VisitSequencePointExpression(BoundSequencePointExpression node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundSequencePointExpression boundSequencePointExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundSequencePointExpression = node.Update(expression, value.Item2);
                (boundSequencePointExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundSequencePointExpression = node.Update(expression, node.Type);
            }
            return boundSequencePointExpression;
        }

        public override BoundNode? VisitSequence(BoundSequence node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundExpression> sideEffects = VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            BoundSequence boundSequence;
            if (_updatedNullabilities.TryGetValue(node, out var value2))
            {
                boundSequence = node.Update(updatedArray, sideEffects, value, value2.Item2);
                (boundSequence.TopLevelNullability, _) = value2;
            }
            else
            {
                boundSequence = node.Update(updatedArray, sideEffects, value, node.Type);
            }
            return boundSequence;
        }

        public override BoundNode? VisitSpillSequence(BoundSpillSequence node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            ImmutableArray<BoundStatement> sideEffects = VisitList(node.SideEffects);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            BoundSpillSequence boundSpillSequence;
            if (_updatedNullabilities.TryGetValue(node, out var value2))
            {
                boundSpillSequence = node.Update(updatedArray, sideEffects, value, value2.Item2);
                (boundSpillSequence.TopLevelNullability, _) = value2;
            }
            else
            {
                boundSpillSequence = node.Update(updatedArray, sideEffects, value, node.Type);
            }
            return boundSpillSequence;
        }

        public override BoundNode? VisitDynamicMemberAccess(BoundDynamicMemberAccess node)
        {
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            BoundDynamicMemberAccess boundDynamicMemberAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDynamicMemberAccess = node.Update(receiver, node.TypeArgumentsOpt, node.Name, node.Invoked, node.Indexed, value.Item2);
                (boundDynamicMemberAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundDynamicMemberAccess = node.Update(receiver, node.TypeArgumentsOpt, node.Name, node.Invoked, node.Indexed, node.Type);
            }
            return boundDynamicMemberAccess;
        }

        public override BoundNode? VisitDynamicInvocation(BoundDynamicInvocation node)
        {
            ImmutableArray<MethodSymbol> updatedArray = GetUpdatedArray(node, node.ApplicableMethods);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundDynamicInvocation boundDynamicInvocation;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDynamicInvocation = node.Update(node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, updatedArray, expression, arguments, value.Item2);
                (boundDynamicInvocation.TopLevelNullability, _) = value;
            }
            else
            {
                boundDynamicInvocation = node.Update(node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, updatedArray, expression, arguments, node.Type);
            }
            return boundDynamicInvocation;
        }

        public override BoundNode? VisitConditionalAccess(BoundConditionalAccess node)
        {
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            BoundExpression accessExpression = (BoundExpression)Visit(node.AccessExpression);
            BoundConditionalAccess boundConditionalAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundConditionalAccess = node.Update(receiver, accessExpression, value.Item2);
                (boundConditionalAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundConditionalAccess = node.Update(receiver, accessExpression, node.Type);
            }
            return boundConditionalAccess;
        }

        public override BoundNode? VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.HasValueMethodOpt);
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            BoundExpression whenNotNull = (BoundExpression)Visit(node.WhenNotNull);
            BoundExpression whenNullOpt = (BoundExpression)Visit(node.WhenNullOpt);
            BoundLoweredConditionalAccess boundLoweredConditionalAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundLoweredConditionalAccess = node.Update(receiver, updatedSymbol, whenNotNull, whenNullOpt, node.Id, value.Item2);
                (boundLoweredConditionalAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundLoweredConditionalAccess = node.Update(receiver, updatedSymbol, whenNotNull, whenNullOpt, node.Id, node.Type);
            }
            return boundLoweredConditionalAccess;
        }

        public override BoundNode? VisitConditionalReceiver(BoundConditionalReceiver node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundConditionalReceiver boundConditionalReceiver = node.Update(node.Id, value.Item2);
            (boundConditionalReceiver.TopLevelNullability, _) = value;
            return boundConditionalReceiver;
        }

        public override BoundNode? VisitComplexConditionalReceiver(BoundComplexConditionalReceiver node)
        {
            BoundExpression valueTypeReceiver = (BoundExpression)Visit(node.ValueTypeReceiver);
            BoundExpression referenceTypeReceiver = (BoundExpression)Visit(node.ReferenceTypeReceiver);
            BoundComplexConditionalReceiver boundComplexConditionalReceiver;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundComplexConditionalReceiver = node.Update(valueTypeReceiver, referenceTypeReceiver, value.Item2);
                (boundComplexConditionalReceiver.TopLevelNullability, _) = value;
            }
            else
            {
                boundComplexConditionalReceiver = node.Update(valueTypeReceiver, referenceTypeReceiver, node.Type);
            }
            return boundComplexConditionalReceiver;
        }

        public override BoundNode? VisitMethodGroup(BoundMethodGroup node)
        {
            ImmutableArray<MethodSymbol> updatedArray = GetUpdatedArray(node, node.Methods);
            Symbol updatedSymbol = GetUpdatedSymbol(node, node.LookupSymbolOpt);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            BoundMethodGroup boundMethodGroup;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundMethodGroup = node.Update(node.TypeArgumentsOpt, node.Name, updatedArray, updatedSymbol, node.LookupError, node.Flags, receiverOpt, node.ResultKind);
                (boundMethodGroup.TopLevelNullability, _) = value;
            }
            else
            {
                boundMethodGroup = node.Update(node.TypeArgumentsOpt, node.Name, updatedArray, updatedSymbol, node.LookupError, node.Flags, receiverOpt, node.ResultKind);
            }
            return boundMethodGroup;
        }

        public override BoundNode? VisitPropertyGroup(BoundPropertyGroup node)
        {
            ImmutableArray<PropertySymbol> updatedArray = GetUpdatedArray(node, node.Properties);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            BoundPropertyGroup boundPropertyGroup;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundPropertyGroup = node.Update(updatedArray, receiverOpt, node.ResultKind);
                (boundPropertyGroup.TopLevelNullability, _) = value;
            }
            else
            {
                boundPropertyGroup = node.Update(updatedArray, receiverOpt, node.ResultKind);
            }
            return boundPropertyGroup;
        }

        public override BoundNode? VisitCall(BoundCall node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.Method);
            GetUpdatedArray(node, node.OriginalMethodsOpt);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundCall boundCall;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundCall = node.Update(receiverOpt, updatedSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, node.OriginalMethodsOpt, value.Item2);
                (boundCall.TopLevelNullability, _) = value;
            }
            else
            {
                boundCall = node.Update(receiverOpt, updatedSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.IsDelegateCall, node.Expanded, node.InvokedAsExtensionMethod, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, node.OriginalMethodsOpt, node.Type);
            }
            return boundCall;
        }

        public override BoundNode? VisitEventAssignmentOperator(BoundEventAssignmentOperator node)
        {
            EventSymbol updatedSymbol = GetUpdatedSymbol(node, node.Event);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            BoundExpression argument = (BoundExpression)Visit(node.Argument);
            BoundEventAssignmentOperator boundEventAssignmentOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundEventAssignmentOperator = node.Update(updatedSymbol, node.IsAddition, node.IsDynamic, receiverOpt, argument, value.Item2);
                (boundEventAssignmentOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundEventAssignmentOperator = node.Update(updatedSymbol, node.IsAddition, node.IsDynamic, receiverOpt, argument, node.Type);
            }
            return boundEventAssignmentOperator;
        }

        public override BoundNode? VisitAttribute(BoundAttribute node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.Constructor);
            ImmutableArray<BoundExpression> constructorArguments = VisitList(node.ConstructorArguments);
            ImmutableArray<BoundAssignmentOperator> namedArguments = VisitList(node.NamedArguments);
            BoundAttribute boundAttribute;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAttribute = node.Update(updatedSymbol, constructorArguments, node.ConstructorArgumentNamesOpt, node.ConstructorArgumentsToParamsOpt, node.ConstructorExpanded, namedArguments, node.ResultKind, value.Item2);
                (boundAttribute.TopLevelNullability, _) = value;
            }
            else
            {
                boundAttribute = node.Update(updatedSymbol, constructorArguments, node.ConstructorArgumentNamesOpt, node.ConstructorArgumentsToParamsOpt, node.ConstructorExpanded, namedArguments, node.ResultKind, node.Type);
            }
            return boundAttribute;
        }

        public override BoundNode? VisitUnconvertedObjectCreationExpression(BoundUnconvertedObjectCreationExpression node)
        {
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundUnconvertedObjectCreationExpression boundUnconvertedObjectCreationExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundUnconvertedObjectCreationExpression = node.Update(arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.InitializerOpt);
                (boundUnconvertedObjectCreationExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundUnconvertedObjectCreationExpression = node.Update(arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.InitializerOpt);
            }
            return boundUnconvertedObjectCreationExpression;
        }

        public override BoundNode? VisitObjectCreationExpression(BoundObjectCreationExpression node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.Constructor);
            ImmutableArray<MethodSymbol> updatedArray = GetUpdatedArray(node, node.ConstructorsGroup);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundObjectInitializerExpressionBase initializerExpressionOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerExpressionOpt);
            BoundObjectCreationExpression boundObjectCreationExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundObjectCreationExpression = node.Update(updatedSymbol, updatedArray, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ConstantValueOpt, initializerExpressionOpt, node.WasTargetTyped, value.Item2);
                (boundObjectCreationExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundObjectCreationExpression = node.Update(updatedSymbol, updatedArray, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ConstantValueOpt, initializerExpressionOpt, node.WasTargetTyped, node.Type);
            }
            return boundObjectCreationExpression;
        }

        public override BoundNode? VisitTupleLiteral(BoundTupleLiteral node)
        {
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundTupleLiteral boundTupleLiteral;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundTupleLiteral = node.Update(arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, value.Item2);
                (boundTupleLiteral.TopLevelNullability, _) = value;
            }
            else
            {
                boundTupleLiteral = node.Update(arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, node.Type);
            }
            return boundTupleLiteral;
        }

        public override BoundNode? VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
        {
            BoundTupleLiteral sourceTuple = (BoundTupleLiteral)Visit(node.SourceTuple);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundConvertedTupleLiteral boundConvertedTupleLiteral;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundConvertedTupleLiteral = node.Update(sourceTuple, node.WasTargetTyped, arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, value.Item2);
                (boundConvertedTupleLiteral.TopLevelNullability, _) = value;
            }
            else
            {
                boundConvertedTupleLiteral = node.Update(sourceTuple, node.WasTargetTyped, arguments, node.ArgumentNamesOpt, node.InferredNamesOpt, node.Type);
            }
            return boundConvertedTupleLiteral;
        }

        public override BoundNode? VisitDynamicObjectCreationExpression(BoundDynamicObjectCreationExpression node)
        {
            ImmutableArray<MethodSymbol> updatedArray = GetUpdatedArray(node, node.ApplicableMethods);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundObjectInitializerExpressionBase initializerExpressionOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerExpressionOpt);
            BoundDynamicObjectCreationExpression boundDynamicObjectCreationExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDynamicObjectCreationExpression = node.Update(node.Name, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, initializerExpressionOpt, updatedArray, value.Item2);
                (boundDynamicObjectCreationExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundDynamicObjectCreationExpression = node.Update(node.Name, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, initializerExpressionOpt, updatedArray, node.Type);
            }
            return boundDynamicObjectCreationExpression;
        }

        public override BoundNode? VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
        {
            BoundObjectInitializerExpressionBase initializerExpressionOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerExpressionOpt);
            BoundNoPiaObjectCreationExpression boundNoPiaObjectCreationExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundNoPiaObjectCreationExpression = node.Update(node.GuidString, initializerExpressionOpt, value.Item2);
                (boundNoPiaObjectCreationExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundNoPiaObjectCreationExpression = node.Update(node.GuidString, initializerExpressionOpt, node.Type);
            }
            return boundNoPiaObjectCreationExpression;
        }

        public override BoundNode? VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
        {
            BoundObjectOrCollectionValuePlaceholder placeholder = (BoundObjectOrCollectionValuePlaceholder)Visit(node.Placeholder);
            ImmutableArray<BoundExpression> initializers = VisitList(node.Initializers);
            BoundObjectInitializerExpression boundObjectInitializerExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundObjectInitializerExpression = node.Update(placeholder, initializers, value.Item2);
                (boundObjectInitializerExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundObjectInitializerExpression = node.Update(placeholder, initializers, node.Type);
            }
            return boundObjectInitializerExpression;
        }

        public override BoundNode? VisitObjectInitializerMember(BoundObjectInitializerMember node)
        {
            Symbol updatedSymbol = GetUpdatedSymbol(node, node.MemberSymbol);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.ReceiverType);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundObjectInitializerMember boundObjectInitializerMember;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundObjectInitializerMember = node.Update(updatedSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, updatedSymbol2, value.Item2);
                (boundObjectInitializerMember.TopLevelNullability, _) = value;
            }
            else
            {
                boundObjectInitializerMember = node.Update(updatedSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.ResultKind, updatedSymbol2, node.Type);
            }
            return boundObjectInitializerMember;
        }

        public override BoundNode? VisitDynamicObjectInitializerMember(BoundDynamicObjectInitializerMember node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.ReceiverType);
            BoundDynamicObjectInitializerMember boundDynamicObjectInitializerMember;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDynamicObjectInitializerMember = node.Update(node.MemberName, updatedSymbol, value.Item2);
                (boundDynamicObjectInitializerMember.TopLevelNullability, _) = value;
            }
            else
            {
                boundDynamicObjectInitializerMember = node.Update(node.MemberName, updatedSymbol, node.Type);
            }
            return boundDynamicObjectInitializerMember;
        }

        public override BoundNode? VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
        {
            BoundObjectOrCollectionValuePlaceholder placeholder = (BoundObjectOrCollectionValuePlaceholder)Visit(node.Placeholder);
            ImmutableArray<BoundExpression> initializers = VisitList(node.Initializers);
            BoundCollectionInitializerExpression boundCollectionInitializerExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundCollectionInitializerExpression = node.Update(placeholder, initializers, value.Item2);
                (boundCollectionInitializerExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundCollectionInitializerExpression = node.Update(placeholder, initializers, node.Type);
            }
            return boundCollectionInitializerExpression;
        }

        public override BoundNode? VisitCollectionElementInitializer(BoundCollectionElementInitializer node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.AddMethod);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundExpression implicitReceiverOpt = (BoundExpression)Visit(node.ImplicitReceiverOpt);
            BoundCollectionElementInitializer boundCollectionElementInitializer;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundCollectionElementInitializer = node.Update(updatedSymbol, arguments, implicitReceiverOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.InvokedAsExtensionMethod, node.ResultKind, value.Item2);
                (boundCollectionElementInitializer.TopLevelNullability, _) = value;
            }
            else
            {
                boundCollectionElementInitializer = node.Update(updatedSymbol, arguments, implicitReceiverOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.InvokedAsExtensionMethod, node.ResultKind, node.Type);
            }
            return boundCollectionElementInitializer;
        }

        public override BoundNode? VisitDynamicCollectionElementInitializer(BoundDynamicCollectionElementInitializer node)
        {
            ImmutableArray<MethodSymbol> updatedArray = GetUpdatedArray(node, node.ApplicableMethods);
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundDynamicCollectionElementInitializer boundDynamicCollectionElementInitializer;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDynamicCollectionElementInitializer = node.Update(updatedArray, expression, arguments, value.Item2);
                (boundDynamicCollectionElementInitializer.TopLevelNullability, _) = value;
            }
            else
            {
                boundDynamicCollectionElementInitializer = node.Update(updatedArray, expression, arguments, node.Type);
            }
            return boundDynamicCollectionElementInitializer;
        }

        public override BoundNode? VisitImplicitReceiver(BoundImplicitReceiver node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundImplicitReceiver boundImplicitReceiver = node.Update(value.Item2);
            (boundImplicitReceiver.TopLevelNullability, _) = value;
            return boundImplicitReceiver;
        }

        public override BoundNode? VisitAnonymousObjectCreationExpression(BoundAnonymousObjectCreationExpression node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.Constructor);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            ImmutableArray<BoundAnonymousPropertyDeclaration> declarations = VisitList(node.Declarations);
            BoundAnonymousObjectCreationExpression boundAnonymousObjectCreationExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAnonymousObjectCreationExpression = node.Update(updatedSymbol, arguments, declarations, value.Item2);
                (boundAnonymousObjectCreationExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundAnonymousObjectCreationExpression = node.Update(updatedSymbol, arguments, declarations, node.Type);
            }
            return boundAnonymousObjectCreationExpression;
        }

        public override BoundNode? VisitAnonymousPropertyDeclaration(BoundAnonymousPropertyDeclaration node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.Property);
            BoundAnonymousPropertyDeclaration boundAnonymousPropertyDeclaration;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundAnonymousPropertyDeclaration = node.Update(updatedSymbol, value.Item2);
                (boundAnonymousPropertyDeclaration.TopLevelNullability, _) = value;
            }
            else
            {
                boundAnonymousPropertyDeclaration = node.Update(updatedSymbol, node.Type);
            }
            return boundAnonymousPropertyDeclaration;
        }

        public override BoundNode? VisitNewT(BoundNewT node)
        {
            BoundObjectInitializerExpressionBase initializerExpressionOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerExpressionOpt);
            BoundNewT boundNewT;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundNewT = node.Update(initializerExpressionOpt, value.Item2);
                (boundNewT.TopLevelNullability, _) = value;
            }
            else
            {
                boundNewT = node.Update(initializerExpressionOpt, node.Type);
            }
            return boundNewT;
        }

        public override BoundNode? VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.MethodOpt);
            BoundExpression argument = (BoundExpression)Visit(node.Argument);
            BoundDelegateCreationExpression boundDelegateCreationExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDelegateCreationExpression = node.Update(argument, updatedSymbol, node.IsExtensionMethod, value.Item2);
                (boundDelegateCreationExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundDelegateCreationExpression = node.Update(argument, updatedSymbol, node.IsExtensionMethod, node.Type);
            }
            return boundDelegateCreationExpression;
        }

        public override BoundNode? VisitArrayCreation(BoundArrayCreation node)
        {
            ImmutableArray<BoundExpression> bounds = VisitList(node.Bounds);
            BoundArrayInitialization initializerOpt = (BoundArrayInitialization)Visit(node.InitializerOpt);
            BoundArrayCreation boundArrayCreation;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundArrayCreation = node.Update(bounds, initializerOpt, value.Item2);
                (boundArrayCreation.TopLevelNullability, _) = value;
            }
            else
            {
                boundArrayCreation = node.Update(bounds, initializerOpt, node.Type);
            }
            return boundArrayCreation;
        }

        public override BoundNode? VisitArrayInitialization(BoundArrayInitialization node)
        {
            ImmutableArray<BoundExpression> initializers = VisitList(node.Initializers);
            BoundArrayInitialization boundArrayInitialization;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundArrayInitialization = node.Update(initializers);
                (boundArrayInitialization.TopLevelNullability, _) = value;
            }
            else
            {
                boundArrayInitialization = node.Update(initializers);
            }
            return boundArrayInitialization;
        }

        public override BoundNode? VisitStackAllocArrayCreation(BoundStackAllocArrayCreation node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.ElementType);
            BoundExpression count = (BoundExpression)Visit(node.Count);
            BoundArrayInitialization initializerOpt = (BoundArrayInitialization)Visit(node.InitializerOpt);
            BoundStackAllocArrayCreation boundStackAllocArrayCreation;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundStackAllocArrayCreation = node.Update(updatedSymbol, count, initializerOpt, value.Item2);
                (boundStackAllocArrayCreation.TopLevelNullability, _) = value;
            }
            else
            {
                boundStackAllocArrayCreation = node.Update(updatedSymbol, count, initializerOpt, node.Type);
            }
            return boundStackAllocArrayCreation;
        }

        public override BoundNode? VisitConvertedStackAllocExpression(BoundConvertedStackAllocExpression node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.ElementType);
            BoundExpression count = (BoundExpression)Visit(node.Count);
            BoundArrayInitialization initializerOpt = (BoundArrayInitialization)Visit(node.InitializerOpt);
            BoundConvertedStackAllocExpression boundConvertedStackAllocExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundConvertedStackAllocExpression = node.Update(updatedSymbol, count, initializerOpt, value.Item2);
                (boundConvertedStackAllocExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundConvertedStackAllocExpression = node.Update(updatedSymbol, count, initializerOpt, node.Type);
            }
            return boundConvertedStackAllocExpression;
        }

        public override BoundNode? VisitFieldAccess(BoundFieldAccess node)
        {
            FieldSymbol updatedSymbol = GetUpdatedSymbol(node, node.FieldSymbol);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            BoundFieldAccess boundFieldAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundFieldAccess = node.Update(receiverOpt, updatedSymbol, node.ConstantValueOpt, node.ResultKind, node.IsByValue, node.IsDeclaration, value.Item2);
                (boundFieldAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundFieldAccess = node.Update(receiverOpt, updatedSymbol, node.ConstantValueOpt, node.ResultKind, node.IsByValue, node.IsDeclaration, node.Type);
            }
            return boundFieldAccess;
        }

        public override BoundNode? VisitHoistedFieldAccess(BoundHoistedFieldAccess node)
        {
            FieldSymbol updatedSymbol = GetUpdatedSymbol(node, node.FieldSymbol);
            BoundHoistedFieldAccess boundHoistedFieldAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundHoistedFieldAccess = node.Update(updatedSymbol, value.Item2);
                (boundHoistedFieldAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundHoistedFieldAccess = node.Update(updatedSymbol, node.Type);
            }
            return boundHoistedFieldAccess;
        }

        public override BoundNode? VisitPropertyAccess(BoundPropertyAccess node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.PropertySymbol);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            BoundPropertyAccess boundPropertyAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundPropertyAccess = node.Update(receiverOpt, updatedSymbol, node.ResultKind, value.Item2);
                (boundPropertyAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundPropertyAccess = node.Update(receiverOpt, updatedSymbol, node.ResultKind, node.Type);
            }
            return boundPropertyAccess;
        }

        public override BoundNode? VisitEventAccess(BoundEventAccess node)
        {
            EventSymbol updatedSymbol = GetUpdatedSymbol(node, node.EventSymbol);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            BoundEventAccess boundEventAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundEventAccess = node.Update(receiverOpt, updatedSymbol, node.IsUsableAsField, node.ResultKind, value.Item2);
                (boundEventAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundEventAccess = node.Update(receiverOpt, updatedSymbol, node.IsUsableAsField, node.ResultKind, node.Type);
            }
            return boundEventAccess;
        }

        public override BoundNode? VisitIndexerAccess(BoundIndexerAccess node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.Indexer);
            GetUpdatedArray(node, node.OriginalIndexersOpt);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundIndexerAccess boundIndexerAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundIndexerAccess = node.Update(receiverOpt, updatedSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.OriginalIndexersOpt, value.Item2);
                (boundIndexerAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundIndexerAccess = node.Update(receiverOpt, updatedSymbol, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, node.Expanded, node.ArgsToParamsOpt, node.DefaultArguments, node.OriginalIndexersOpt, node.Type);
            }
            return boundIndexerAccess;
        }

        public override BoundNode? VisitIndexOrRangePatternIndexerAccess(BoundIndexOrRangePatternIndexerAccess node)
        {
            PropertySymbol updatedSymbol = GetUpdatedSymbol(node, node.LengthOrCountProperty);
            Symbol updatedSymbol2 = GetUpdatedSymbol(node, node.PatternSymbol);
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            BoundExpression argument = (BoundExpression)Visit(node.Argument);
            BoundIndexOrRangePatternIndexerAccess boundIndexOrRangePatternIndexerAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundIndexOrRangePatternIndexerAccess = node.Update(receiver, updatedSymbol, updatedSymbol2, argument, value.Item2);
                (boundIndexOrRangePatternIndexerAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundIndexOrRangePatternIndexerAccess = node.Update(receiver, updatedSymbol, updatedSymbol2, argument, node.Type);
            }
            return boundIndexOrRangePatternIndexerAccess;
        }

        public override BoundNode? VisitDynamicIndexerAccess(BoundDynamicIndexerAccess node)
        {
            ImmutableArray<PropertySymbol> updatedArray = GetUpdatedArray(node, node.ApplicableIndexers);
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
            BoundDynamicIndexerAccess boundDynamicIndexerAccess;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundDynamicIndexerAccess = node.Update(receiver, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, updatedArray, value.Item2);
                (boundDynamicIndexerAccess.TopLevelNullability, _) = value;
            }
            else
            {
                boundDynamicIndexerAccess = node.Update(receiver, arguments, node.ArgumentNamesOpt, node.ArgumentRefKindsOpt, updatedArray, node.Type);
            }
            return boundDynamicIndexerAccess;
        }

        public override BoundNode? VisitLambda(BoundLambda node)
        {
            LambdaSymbol updatedSymbol = GetUpdatedSymbol(node, node.Symbol);
            UnboundLambda unboundLambda = node.UnboundLambda;
            BoundBlock body = (BoundBlock)Visit(node.Body);
            BoundLambda boundLambda;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundLambda = node.Update(unboundLambda, updatedSymbol, body, node.Diagnostics, node.Binder, value.Item2);
                (boundLambda.TopLevelNullability, _) = value;
            }
            else
            {
                boundLambda = node.Update(unboundLambda, updatedSymbol, body, node.Diagnostics, node.Binder, node.Type);
            }
            return boundLambda;
        }

        public override BoundNode? VisitUnboundLambda(UnboundLambda node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            UnboundLambda unboundLambda = node.Update(node.Data, node.WithDependencies);
            (unboundLambda.TopLevelNullability, _) = value;
            return unboundLambda;
        }

        public override BoundNode? VisitQueryClause(BoundQueryClause node)
        {
            RangeVariableSymbol updatedSymbol = GetUpdatedSymbol(node, node.DefinedSymbol);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            BoundExpression operation = node.Operation;
            BoundExpression cast = node.Cast;
            BoundExpression unoptimizedForm = node.UnoptimizedForm;
            BoundQueryClause boundQueryClause;
            if (_updatedNullabilities.TryGetValue(node, out var value2))
            {
                boundQueryClause = node.Update(value, updatedSymbol, operation, cast, node.Binder, unoptimizedForm, value2.Item2);
                (boundQueryClause.TopLevelNullability, _) = value2;
            }
            else
            {
                boundQueryClause = node.Update(value, updatedSymbol, operation, cast, node.Binder, unoptimizedForm, node.Type);
            }
            return boundQueryClause;
        }

        public override BoundNode? VisitNameOfOperator(BoundNameOfOperator node)
        {
            BoundExpression argument = (BoundExpression)Visit(node.Argument);
            BoundNameOfOperator boundNameOfOperator;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundNameOfOperator = node.Update(argument, node.ConstantValueOpt, value.Item2);
                (boundNameOfOperator.TopLevelNullability, _) = value;
            }
            else
            {
                boundNameOfOperator = node.Update(argument, node.ConstantValueOpt, node.Type);
            }
            return boundNameOfOperator;
        }

        public override BoundNode? VisitUnconvertedInterpolatedString(BoundUnconvertedInterpolatedString node)
        {
            ImmutableArray<BoundExpression> parts = VisitList(node.Parts);
            BoundUnconvertedInterpolatedString boundUnconvertedInterpolatedString;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundUnconvertedInterpolatedString = node.Update(parts, node.ConstantValueOpt, value.Item2);
                (boundUnconvertedInterpolatedString.TopLevelNullability, _) = value;
            }
            else
            {
                boundUnconvertedInterpolatedString = node.Update(parts, node.ConstantValueOpt, node.Type);
            }
            return boundUnconvertedInterpolatedString;
        }

        public override BoundNode? VisitInterpolatedString(BoundInterpolatedString node)
        {
            ImmutableArray<BoundExpression> parts = VisitList(node.Parts);
            BoundInterpolatedString boundInterpolatedString;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundInterpolatedString = node.Update(parts, node.ConstantValueOpt, value.Item2);
                (boundInterpolatedString.TopLevelNullability, _) = value;
            }
            else
            {
                boundInterpolatedString = node.Update(parts, node.ConstantValueOpt, node.Type);
            }
            return boundInterpolatedString;
        }

        public override BoundNode? VisitStringInsert(BoundStringInsert node)
        {
            BoundExpression value = (BoundExpression)Visit(node.Value);
            BoundExpression alignment = (BoundExpression)Visit(node.Alignment);
            BoundLiteral format = (BoundLiteral)Visit(node.Format);
            BoundStringInsert boundStringInsert;
            if (_updatedNullabilities.TryGetValue(node, out var value2))
            {
                boundStringInsert = node.Update(value, alignment, format, value2.Item2);
                (boundStringInsert.TopLevelNullability, _) = value2;
            }
            else
            {
                boundStringInsert = node.Update(value, alignment, format, node.Type);
            }
            return boundStringInsert;
        }

        public override BoundNode? VisitIsPatternExpression(BoundIsPatternExpression node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundPattern pattern = (BoundPattern)Visit(node.Pattern);
            BoundDecisionDag decisionDag = node.DecisionDag;
            BoundIsPatternExpression boundIsPatternExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundIsPatternExpression = node.Update(expression, pattern, node.IsNegated, decisionDag, node.WhenTrueLabel, node.WhenFalseLabel, value.Item2);
                (boundIsPatternExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundIsPatternExpression = node.Update(expression, pattern, node.IsNegated, decisionDag, node.WhenTrueLabel, node.WhenFalseLabel, node.Type);
            }
            return boundIsPatternExpression;
        }

        public override BoundNode? VisitConstantPattern(BoundConstantPattern node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            return node.Update(value, node.ConstantValue, updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitDiscardPattern(BoundDiscardPattern node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.NarrowedType);
            return node.Update(updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitDeclarationPattern(BoundDeclarationPattern node)
        {
            Symbol updatedSymbol = GetUpdatedSymbol(node, node.Variable);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol3 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundExpression variableAccess = (BoundExpression)Visit(node.VariableAccess);
            BoundTypeExpression declaredType = (BoundTypeExpression)Visit(node.DeclaredType);
            return node.Update(updatedSymbol, variableAccess, declaredType, node.IsVar, updatedSymbol2, updatedSymbol3);
        }

        public override BoundNode? VisitRecursivePattern(BoundRecursivePattern node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.DeconstructMethod);
            Symbol updatedSymbol2 = GetUpdatedSymbol(node, node.Variable);
            TypeSymbol updatedSymbol3 = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol4 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundTypeExpression declaredType = (BoundTypeExpression)Visit(node.DeclaredType);
            ImmutableArray<BoundSubpattern> deconstruction = VisitList(node.Deconstruction);
            ImmutableArray<BoundSubpattern> properties = VisitList(node.Properties);
            BoundExpression variableAccess = (BoundExpression)Visit(node.VariableAccess);
            return node.Update(declaredType, updatedSymbol, deconstruction, properties, updatedSymbol2, variableAccess, node.IsExplicitNotNullTest, updatedSymbol3, updatedSymbol4);
        }

        public override BoundNode? VisitITuplePattern(BoundITuplePattern node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.GetLengthMethod);
            MethodSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.GetItemMethod);
            TypeSymbol updatedSymbol3 = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol4 = GetUpdatedSymbol(node, node.NarrowedType);
            ImmutableArray<BoundSubpattern> subpatterns = VisitList(node.Subpatterns);
            return node.Update(updatedSymbol, updatedSymbol2, subpatterns, updatedSymbol3, updatedSymbol4);
        }

        public override BoundNode? VisitSubpattern(BoundSubpattern node)
        {
            Symbol updatedSymbol = GetUpdatedSymbol(node, node.Symbol);
            BoundPattern pattern = (BoundPattern)Visit(node.Pattern);
            return node.Update(updatedSymbol, pattern);
        }

        public override BoundNode? VisitTypePattern(BoundTypePattern node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundTypeExpression declaredType = (BoundTypeExpression)Visit(node.DeclaredType);
            return node.Update(declaredType, node.IsExplicitNotNullTest, updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitBinaryPattern(BoundBinaryPattern node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundPattern left = (BoundPattern)Visit(node.Left);
            BoundPattern right = (BoundPattern)Visit(node.Right);
            return node.Update(node.Disjunction, left, right, updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitNegatedPattern(BoundNegatedPattern node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundPattern negated = (BoundPattern)Visit(node.Negated);
            return node.Update(negated, updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitRelationalPattern(BoundRelationalPattern node)
        {
            TypeSymbol updatedSymbol = GetUpdatedSymbol(node, node.InputType);
            TypeSymbol updatedSymbol2 = GetUpdatedSymbol(node, node.NarrowedType);
            BoundExpression value = (BoundExpression)Visit(node.Value);
            return node.Update(node.Relation, value, node.ConstantValue, updatedSymbol, updatedSymbol2);
        }

        public override BoundNode? VisitDiscardExpression(BoundDiscardExpression node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            BoundDiscardExpression boundDiscardExpression = node.Update(value.Item2);
            (boundDiscardExpression.TopLevelNullability, _) = value;
            return boundDiscardExpression;
        }

        public override BoundNode? VisitThrowExpression(BoundThrowExpression node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundThrowExpression boundThrowExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundThrowExpression = node.Update(expression, value.Item2);
                (boundThrowExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundThrowExpression = node.Update(expression, node.Type);
            }
            return boundThrowExpression;
        }

        public override BoundNode? VisitOutVariablePendingInference(OutVariablePendingInference node)
        {
            Symbol updatedSymbol = GetUpdatedSymbol(node, node.VariableSymbol);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            OutVariablePendingInference outVariablePendingInference;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                outVariablePendingInference = node.Update(updatedSymbol, receiverOpt);
                (outVariablePendingInference.TopLevelNullability, _) = value;
            }
            else
            {
                outVariablePendingInference = node.Update(updatedSymbol, receiverOpt);
            }
            return outVariablePendingInference;
        }

        public override BoundNode? VisitDeconstructionVariablePendingInference(DeconstructionVariablePendingInference node)
        {
            Symbol updatedSymbol = GetUpdatedSymbol(node, node.VariableSymbol);
            BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
            DeconstructionVariablePendingInference deconstructionVariablePendingInference;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                deconstructionVariablePendingInference = node.Update(updatedSymbol, receiverOpt);
                (deconstructionVariablePendingInference.TopLevelNullability, _) = value;
            }
            else
            {
                deconstructionVariablePendingInference = node.Update(updatedSymbol, receiverOpt);
            }
            return deconstructionVariablePendingInference;
        }

        public override BoundNode? VisitOutDeconstructVarPendingInference(OutDeconstructVarPendingInference node)
        {
            if (!_updatedNullabilities.TryGetValue(node, out var value))
            {
                return node;
            }
            OutDeconstructVarPendingInference outDeconstructVarPendingInference = node.Update();
            (outDeconstructVarPendingInference.TopLevelNullability, _) = value;
            return outDeconstructVarPendingInference;
        }

        public override BoundNode? VisitConstructorMethodBody(BoundConstructorMethodBody node)
        {
            ImmutableArray<LocalSymbol> updatedArray = GetUpdatedArray(node, node.Locals);
            BoundExpressionStatement initializer = (BoundExpressionStatement)Visit(node.Initializer);
            BoundBlock blockBody = (BoundBlock)Visit(node.BlockBody);
            BoundBlock expressionBody = (BoundBlock)Visit(node.ExpressionBody);
            return node.Update(updatedArray, initializer, blockBody, expressionBody);
        }

        public override BoundNode? VisitExpressionWithNullability(BoundExpressionWithNullability node)
        {
            BoundExpression expression = (BoundExpression)Visit(node.Expression);
            BoundExpressionWithNullability boundExpressionWithNullability;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundExpressionWithNullability = node.Update(expression, node.NullableAnnotation, value.Item2);
                (boundExpressionWithNullability.TopLevelNullability, _) = value;
            }
            else
            {
                boundExpressionWithNullability = node.Update(expression, node.NullableAnnotation, node.Type);
            }
            return boundExpressionWithNullability;
        }

        public override BoundNode? VisitWithExpression(BoundWithExpression node)
        {
            MethodSymbol updatedSymbol = GetUpdatedSymbol(node, node.CloneMethod);
            BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
            BoundObjectInitializerExpressionBase initializerExpression = (BoundObjectInitializerExpressionBase)Visit(node.InitializerExpression);
            BoundWithExpression boundWithExpression;
            if (_updatedNullabilities.TryGetValue(node, out var value))
            {
                boundWithExpression = node.Update(receiver, updatedSymbol, initializerExpression, value.Item2);
                (boundWithExpression.TopLevelNullability, _) = value;
            }
            else
            {
                boundWithExpression = node.Update(receiver, updatedSymbol, initializerExpression, node.Type);
            }
            return boundWithExpression;
        }
    }
}
