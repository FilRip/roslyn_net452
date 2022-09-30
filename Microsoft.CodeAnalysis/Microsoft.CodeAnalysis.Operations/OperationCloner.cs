using System.Collections.Immutable;

using Microsoft.CodeAnalysis.FlowAnalysis;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Operations
{
    internal sealed class OperationCloner : OperationVisitor<object, IOperation>
    {
        private static readonly OperationCloner s_instance = new OperationCloner();

        public static T CloneOperation<T>(T operation) where T : IOperation
        {
            return s_instance.Visit(operation);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("node")]
        private T? Visit<T>(T? node) where T : IOperation?
        {
            return (T)Visit(node, null);
        }

        public override IOperation DefaultVisit(IOperation operation, object? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private ImmutableArray<T> VisitArray<T>(ImmutableArray<T> nodes) where T : IOperation
        {
            return nodes.SelectAsArray((T n, OperationCloner @this) => @this.Visit(n), this);
        }

        private ImmutableArray<(ISymbol, T)> VisitArray<T>(ImmutableArray<(ISymbol, T)> nodes) where T : IOperation
        {
            return nodes.SelectAsArray<(ISymbol, T), OperationCloner, (ISymbol, T)>(((ISymbol, T) n, OperationCloner @this) => (n.Item1, @this.Visit(n.Item2)), this);
        }

        public override IOperation VisitBlock(IBlockOperation operation, object? argument)
        {
            BlockOperation blockOperation = (BlockOperation)operation;
            return new BlockOperation(VisitArray(blockOperation.Operations), blockOperation.Locals, blockOperation.OwningSemanticModel, blockOperation.Syntax, blockOperation.IsImplicit);
        }

        public override IOperation VisitVariableDeclarationGroup(IVariableDeclarationGroupOperation operation, object? argument)
        {
            VariableDeclarationGroupOperation variableDeclarationGroupOperation = (VariableDeclarationGroupOperation)operation;
            return new VariableDeclarationGroupOperation(VisitArray(variableDeclarationGroupOperation.Declarations), variableDeclarationGroupOperation.OwningSemanticModel, variableDeclarationGroupOperation.Syntax, variableDeclarationGroupOperation.IsImplicit);
        }

        public override IOperation VisitSwitch(ISwitchOperation operation, object? argument)
        {
            SwitchOperation switchOperation = (SwitchOperation)operation;
            return new SwitchOperation(switchOperation.Locals, Visit(switchOperation.Value), VisitArray(switchOperation.Cases), switchOperation.ExitLabel, switchOperation.OwningSemanticModel, switchOperation.Syntax, switchOperation.IsImplicit);
        }

        public override IOperation VisitForEachLoop(IForEachLoopOperation operation, object? argument)
        {
            ForEachLoopOperation forEachLoopOperation = (ForEachLoopOperation)operation;
            return new ForEachLoopOperation(Visit(forEachLoopOperation.LoopControlVariable), Visit(forEachLoopOperation.Collection), VisitArray(forEachLoopOperation.NextVariables), forEachLoopOperation.Info, forEachLoopOperation.IsAsynchronous, Visit(forEachLoopOperation.Body), forEachLoopOperation.Locals, forEachLoopOperation.ContinueLabel, forEachLoopOperation.ExitLabel, forEachLoopOperation.OwningSemanticModel, forEachLoopOperation.Syntax, forEachLoopOperation.IsImplicit);
        }

        public override IOperation VisitForLoop(IForLoopOperation operation, object? argument)
        {
            ForLoopOperation forLoopOperation = (ForLoopOperation)operation;
            return new ForLoopOperation(VisitArray(forLoopOperation.Before), forLoopOperation.ConditionLocals, Visit(forLoopOperation.Condition), VisitArray(forLoopOperation.AtLoopBottom), Visit(forLoopOperation.Body), forLoopOperation.Locals, forLoopOperation.ContinueLabel, forLoopOperation.ExitLabel, forLoopOperation.OwningSemanticModel, forLoopOperation.Syntax, forLoopOperation.IsImplicit);
        }

        public override IOperation VisitForToLoop(IForToLoopOperation operation, object? argument)
        {
            ForToLoopOperation forToLoopOperation = (ForToLoopOperation)operation;
            return new ForToLoopOperation(Visit(forToLoopOperation.LoopControlVariable), Visit(forToLoopOperation.InitialValue), Visit(forToLoopOperation.LimitValue), Visit(forToLoopOperation.StepValue), forToLoopOperation.IsChecked, VisitArray(forToLoopOperation.NextVariables), forToLoopOperation.Info, Visit(forToLoopOperation.Body), forToLoopOperation.Locals, forToLoopOperation.ContinueLabel, forToLoopOperation.ExitLabel, forToLoopOperation.OwningSemanticModel, forToLoopOperation.Syntax, forToLoopOperation.IsImplicit);
        }

        public override IOperation VisitWhileLoop(IWhileLoopOperation operation, object? argument)
        {
            WhileLoopOperation whileLoopOperation = (WhileLoopOperation)operation;
            return new WhileLoopOperation(Visit(whileLoopOperation.Condition), whileLoopOperation.ConditionIsTop, whileLoopOperation.ConditionIsUntil, Visit(whileLoopOperation.IgnoredCondition), Visit(whileLoopOperation.Body), whileLoopOperation.Locals, whileLoopOperation.ContinueLabel, whileLoopOperation.ExitLabel, whileLoopOperation.OwningSemanticModel, whileLoopOperation.Syntax, whileLoopOperation.IsImplicit);
        }

        public override IOperation VisitLabeled(ILabeledOperation operation, object? argument)
        {
            LabeledOperation labeledOperation = (LabeledOperation)operation;
            return new LabeledOperation(labeledOperation.Label, Visit(labeledOperation.Operation), labeledOperation.OwningSemanticModel, labeledOperation.Syntax, labeledOperation.IsImplicit);
        }

        public override IOperation VisitBranch(IBranchOperation operation, object? argument)
        {
            BranchOperation branchOperation = (BranchOperation)operation;
            return new BranchOperation(branchOperation.Target, branchOperation.BranchKind, branchOperation.OwningSemanticModel, branchOperation.Syntax, branchOperation.IsImplicit);
        }

        public override IOperation VisitEmpty(IEmptyOperation operation, object? argument)
        {
            EmptyOperation emptyOperation = (EmptyOperation)operation;
            return new EmptyOperation(emptyOperation.OwningSemanticModel, emptyOperation.Syntax, emptyOperation.IsImplicit);
        }

        public override IOperation VisitReturn(IReturnOperation operation, object? argument)
        {
            ReturnOperation returnOperation = (ReturnOperation)operation;
            return new ReturnOperation(Visit(returnOperation.ReturnedValue), returnOperation.Kind, returnOperation.OwningSemanticModel, returnOperation.Syntax, returnOperation.IsImplicit);
        }

        public override IOperation VisitLock(ILockOperation operation, object? argument)
        {
            LockOperation lockOperation = (LockOperation)operation;
            return new LockOperation(Visit(lockOperation.LockedValue), Visit(lockOperation.Body), lockOperation.LockTakenSymbol, lockOperation.OwningSemanticModel, lockOperation.Syntax, lockOperation.IsImplicit);
        }

        public override IOperation VisitTry(ITryOperation operation, object? argument)
        {
            TryOperation tryOperation = (TryOperation)operation;
            return new TryOperation(Visit(tryOperation.Body), VisitArray(tryOperation.Catches), Visit(tryOperation.Finally), tryOperation.ExitLabel, tryOperation.OwningSemanticModel, tryOperation.Syntax, tryOperation.IsImplicit);
        }

        public override IOperation VisitUsing(IUsingOperation operation, object? argument)
        {
            UsingOperation usingOperation = (UsingOperation)operation;
            return new UsingOperation(Visit(usingOperation.Resources), Visit(usingOperation.Body), usingOperation.Locals, usingOperation.IsAsynchronous, usingOperation.DisposeInfo, usingOperation.OwningSemanticModel, usingOperation.Syntax, usingOperation.IsImplicit);
        }

        public override IOperation VisitExpressionStatement(IExpressionStatementOperation operation, object? argument)
        {
            ExpressionStatementOperation expressionStatementOperation = (ExpressionStatementOperation)operation;
            return new ExpressionStatementOperation(Visit(expressionStatementOperation.Operation), expressionStatementOperation.OwningSemanticModel, expressionStatementOperation.Syntax, expressionStatementOperation.IsImplicit);
        }

        public override IOperation VisitLocalFunction(ILocalFunctionOperation operation, object? argument)
        {
            LocalFunctionOperation localFunctionOperation = (LocalFunctionOperation)operation;
            return new LocalFunctionOperation(localFunctionOperation.Symbol, Visit(localFunctionOperation.Body), Visit(localFunctionOperation.IgnoredBody), localFunctionOperation.OwningSemanticModel, localFunctionOperation.Syntax, localFunctionOperation.IsImplicit);
        }

        public override IOperation VisitStop(IStopOperation operation, object? argument)
        {
            StopOperation stopOperation = (StopOperation)operation;
            return new StopOperation(stopOperation.OwningSemanticModel, stopOperation.Syntax, stopOperation.IsImplicit);
        }

        public override IOperation VisitEnd(IEndOperation operation, object? argument)
        {
            EndOperation endOperation = (EndOperation)operation;
            return new EndOperation(endOperation.OwningSemanticModel, endOperation.Syntax, endOperation.IsImplicit);
        }

        public override IOperation VisitRaiseEvent(IRaiseEventOperation operation, object? argument)
        {
            RaiseEventOperation raiseEventOperation = (RaiseEventOperation)operation;
            return new RaiseEventOperation(Visit(raiseEventOperation.EventReference), VisitArray(raiseEventOperation.Arguments), raiseEventOperation.OwningSemanticModel, raiseEventOperation.Syntax, raiseEventOperation.IsImplicit);
        }

        public override IOperation VisitLiteral(ILiteralOperation operation, object? argument)
        {
            LiteralOperation literalOperation = (LiteralOperation)operation;
            return new LiteralOperation(literalOperation.OwningSemanticModel, literalOperation.Syntax, literalOperation.Type, literalOperation.OperationConstantValue, literalOperation.IsImplicit);
        }

        public override IOperation VisitConversion(IConversionOperation operation, object? argument)
        {
            ConversionOperation conversionOperation = (ConversionOperation)operation;
            return new ConversionOperation(Visit(conversionOperation.Operand), conversionOperation.ConversionConvertible, conversionOperation.IsTryCast, conversionOperation.IsChecked, conversionOperation.OwningSemanticModel, conversionOperation.Syntax, conversionOperation.Type, conversionOperation.OperationConstantValue, conversionOperation.IsImplicit);
        }

        public override IOperation VisitInvocation(IInvocationOperation operation, object? argument)
        {
            InvocationOperation invocationOperation = (InvocationOperation)operation;
            return new InvocationOperation(invocationOperation.TargetMethod, Visit(invocationOperation.Instance), invocationOperation.IsVirtual, VisitArray(invocationOperation.Arguments), invocationOperation.OwningSemanticModel, invocationOperation.Syntax, invocationOperation.Type, invocationOperation.IsImplicit);
        }

        public override IOperation VisitArrayElementReference(IArrayElementReferenceOperation operation, object? argument)
        {
            ArrayElementReferenceOperation arrayElementReferenceOperation = (ArrayElementReferenceOperation)operation;
            return new ArrayElementReferenceOperation(Visit(arrayElementReferenceOperation.ArrayReference), VisitArray(arrayElementReferenceOperation.Indices), arrayElementReferenceOperation.OwningSemanticModel, arrayElementReferenceOperation.Syntax, arrayElementReferenceOperation.Type, arrayElementReferenceOperation.IsImplicit);
        }

        public override IOperation VisitLocalReference(ILocalReferenceOperation operation, object? argument)
        {
            LocalReferenceOperation localReferenceOperation = (LocalReferenceOperation)operation;
            return new LocalReferenceOperation(localReferenceOperation.Local, localReferenceOperation.IsDeclaration, localReferenceOperation.OwningSemanticModel, localReferenceOperation.Syntax, localReferenceOperation.Type, localReferenceOperation.OperationConstantValue, localReferenceOperation.IsImplicit);
        }

        public override IOperation VisitParameterReference(IParameterReferenceOperation operation, object? argument)
        {
            ParameterReferenceOperation parameterReferenceOperation = (ParameterReferenceOperation)operation;
            return new ParameterReferenceOperation(parameterReferenceOperation.Parameter, parameterReferenceOperation.OwningSemanticModel, parameterReferenceOperation.Syntax, parameterReferenceOperation.Type, parameterReferenceOperation.IsImplicit);
        }

        public override IOperation VisitFieldReference(IFieldReferenceOperation operation, object? argument)
        {
            FieldReferenceOperation fieldReferenceOperation = (FieldReferenceOperation)operation;
            return new FieldReferenceOperation(fieldReferenceOperation.Field, fieldReferenceOperation.IsDeclaration, Visit(fieldReferenceOperation.Instance), fieldReferenceOperation.OwningSemanticModel, fieldReferenceOperation.Syntax, fieldReferenceOperation.Type, fieldReferenceOperation.OperationConstantValue, fieldReferenceOperation.IsImplicit);
        }

        public override IOperation VisitMethodReference(IMethodReferenceOperation operation, object? argument)
        {
            MethodReferenceOperation methodReferenceOperation = (MethodReferenceOperation)operation;
            return new MethodReferenceOperation(methodReferenceOperation.Method, methodReferenceOperation.IsVirtual, Visit(methodReferenceOperation.Instance), methodReferenceOperation.OwningSemanticModel, methodReferenceOperation.Syntax, methodReferenceOperation.Type, methodReferenceOperation.IsImplicit);
        }

        public override IOperation VisitPropertyReference(IPropertyReferenceOperation operation, object? argument)
        {
            PropertyReferenceOperation propertyReferenceOperation = (PropertyReferenceOperation)operation;
            return new PropertyReferenceOperation(propertyReferenceOperation.Property, VisitArray(propertyReferenceOperation.Arguments), Visit(propertyReferenceOperation.Instance), propertyReferenceOperation.OwningSemanticModel, propertyReferenceOperation.Syntax, propertyReferenceOperation.Type, propertyReferenceOperation.IsImplicit);
        }

        public override IOperation VisitEventReference(IEventReferenceOperation operation, object? argument)
        {
            EventReferenceOperation eventReferenceOperation = (EventReferenceOperation)operation;
            return new EventReferenceOperation(eventReferenceOperation.Event, Visit(eventReferenceOperation.Instance), eventReferenceOperation.OwningSemanticModel, eventReferenceOperation.Syntax, eventReferenceOperation.Type, eventReferenceOperation.IsImplicit);
        }

        public override IOperation VisitUnaryOperator(IUnaryOperation operation, object? argument)
        {
            UnaryOperation unaryOperation = (UnaryOperation)operation;
            return new UnaryOperation(unaryOperation.OperatorKind, Visit(unaryOperation.Operand), unaryOperation.IsLifted, unaryOperation.IsChecked, unaryOperation.OperatorMethod, unaryOperation.OwningSemanticModel, unaryOperation.Syntax, unaryOperation.Type, unaryOperation.OperationConstantValue, unaryOperation.IsImplicit);
        }

        public override IOperation VisitBinaryOperator(IBinaryOperation operation, object? argument)
        {
            BinaryOperation binaryOperation = (BinaryOperation)operation;
            return new BinaryOperation(binaryOperation.OperatorKind, Visit(binaryOperation.LeftOperand), Visit(binaryOperation.RightOperand), binaryOperation.IsLifted, binaryOperation.IsChecked, binaryOperation.IsCompareText, binaryOperation.OperatorMethod, binaryOperation.UnaryOperatorMethod, binaryOperation.OwningSemanticModel, binaryOperation.Syntax, binaryOperation.Type, binaryOperation.OperationConstantValue, binaryOperation.IsImplicit);
        }

        public override IOperation VisitConditional(IConditionalOperation operation, object? argument)
        {
            ConditionalOperation conditionalOperation = (ConditionalOperation)operation;
            return new ConditionalOperation(Visit(conditionalOperation.Condition), Visit(conditionalOperation.WhenTrue), Visit(conditionalOperation.WhenFalse), conditionalOperation.IsRef, conditionalOperation.OwningSemanticModel, conditionalOperation.Syntax, conditionalOperation.Type, conditionalOperation.OperationConstantValue, conditionalOperation.IsImplicit);
        }

        public override IOperation VisitCoalesce(ICoalesceOperation operation, object? argument)
        {
            CoalesceOperation coalesceOperation = (CoalesceOperation)operation;
            return new CoalesceOperation(Visit(coalesceOperation.Value), Visit(coalesceOperation.WhenNull), coalesceOperation.ValueConversionConvertible, coalesceOperation.OwningSemanticModel, coalesceOperation.Syntax, coalesceOperation.Type, coalesceOperation.OperationConstantValue, coalesceOperation.IsImplicit);
        }

        public override IOperation VisitAnonymousFunction(IAnonymousFunctionOperation operation, object? argument)
        {
            AnonymousFunctionOperation anonymousFunctionOperation = (AnonymousFunctionOperation)operation;
            return new AnonymousFunctionOperation(anonymousFunctionOperation.Symbol, Visit(anonymousFunctionOperation.Body), anonymousFunctionOperation.OwningSemanticModel, anonymousFunctionOperation.Syntax, anonymousFunctionOperation.IsImplicit);
        }

        public override IOperation VisitObjectCreation(IObjectCreationOperation operation, object? argument)
        {
            ObjectCreationOperation objectCreationOperation = (ObjectCreationOperation)operation;
            return new ObjectCreationOperation(objectCreationOperation.Constructor, Visit(objectCreationOperation.Initializer), VisitArray(objectCreationOperation.Arguments), objectCreationOperation.OwningSemanticModel, objectCreationOperation.Syntax, objectCreationOperation.Type, objectCreationOperation.OperationConstantValue, objectCreationOperation.IsImplicit);
        }

        public override IOperation VisitTypeParameterObjectCreation(ITypeParameterObjectCreationOperation operation, object? argument)
        {
            TypeParameterObjectCreationOperation typeParameterObjectCreationOperation = (TypeParameterObjectCreationOperation)operation;
            return new TypeParameterObjectCreationOperation(Visit(typeParameterObjectCreationOperation.Initializer), typeParameterObjectCreationOperation.OwningSemanticModel, typeParameterObjectCreationOperation.Syntax, typeParameterObjectCreationOperation.Type, typeParameterObjectCreationOperation.IsImplicit);
        }

        public override IOperation VisitArrayCreation(IArrayCreationOperation operation, object? argument)
        {
            ArrayCreationOperation arrayCreationOperation = (ArrayCreationOperation)operation;
            return new ArrayCreationOperation(VisitArray(arrayCreationOperation.DimensionSizes), Visit(arrayCreationOperation.Initializer), arrayCreationOperation.OwningSemanticModel, arrayCreationOperation.Syntax, arrayCreationOperation.Type, arrayCreationOperation.IsImplicit);
        }

        public override IOperation VisitInstanceReference(IInstanceReferenceOperation operation, object? argument)
        {
            InstanceReferenceOperation instanceReferenceOperation = (InstanceReferenceOperation)operation;
            return new InstanceReferenceOperation(instanceReferenceOperation.ReferenceKind, instanceReferenceOperation.OwningSemanticModel, instanceReferenceOperation.Syntax, instanceReferenceOperation.Type, instanceReferenceOperation.IsImplicit);
        }

        public override IOperation VisitIsType(IIsTypeOperation operation, object? argument)
        {
            IsTypeOperation isTypeOperation = (IsTypeOperation)operation;
            return new IsTypeOperation(Visit(isTypeOperation.ValueOperand), isTypeOperation.TypeOperand, isTypeOperation.IsNegated, isTypeOperation.OwningSemanticModel, isTypeOperation.Syntax, isTypeOperation.Type, isTypeOperation.IsImplicit);
        }

        public override IOperation VisitAwait(IAwaitOperation operation, object? argument)
        {
            AwaitOperation awaitOperation = (AwaitOperation)operation;
            return new AwaitOperation(Visit(awaitOperation.Operation), awaitOperation.OwningSemanticModel, awaitOperation.Syntax, awaitOperation.Type, awaitOperation.IsImplicit);
        }

        public override IOperation VisitSimpleAssignment(ISimpleAssignmentOperation operation, object? argument)
        {
            SimpleAssignmentOperation simpleAssignmentOperation = (SimpleAssignmentOperation)operation;
            return new SimpleAssignmentOperation(simpleAssignmentOperation.IsRef, Visit(simpleAssignmentOperation.Target), Visit(simpleAssignmentOperation.Value), simpleAssignmentOperation.OwningSemanticModel, simpleAssignmentOperation.Syntax, simpleAssignmentOperation.Type, simpleAssignmentOperation.OperationConstantValue, simpleAssignmentOperation.IsImplicit);
        }

        public override IOperation VisitCompoundAssignment(ICompoundAssignmentOperation operation, object? argument)
        {
            CompoundAssignmentOperation compoundAssignmentOperation = (CompoundAssignmentOperation)operation;
            return new CompoundAssignmentOperation(compoundAssignmentOperation.InConversionConvertible, compoundAssignmentOperation.OutConversionConvertible, compoundAssignmentOperation.OperatorKind, compoundAssignmentOperation.IsLifted, compoundAssignmentOperation.IsChecked, compoundAssignmentOperation.OperatorMethod, Visit(compoundAssignmentOperation.Target), Visit(compoundAssignmentOperation.Value), compoundAssignmentOperation.OwningSemanticModel, compoundAssignmentOperation.Syntax, compoundAssignmentOperation.Type, compoundAssignmentOperation.IsImplicit);
        }

        public override IOperation VisitParenthesized(IParenthesizedOperation operation, object? argument)
        {
            ParenthesizedOperation parenthesizedOperation = (ParenthesizedOperation)operation;
            return new ParenthesizedOperation(Visit(parenthesizedOperation.Operand), parenthesizedOperation.OwningSemanticModel, parenthesizedOperation.Syntax, parenthesizedOperation.Type, parenthesizedOperation.OperationConstantValue, parenthesizedOperation.IsImplicit);
        }

        public override IOperation VisitEventAssignment(IEventAssignmentOperation operation, object? argument)
        {
            EventAssignmentOperation eventAssignmentOperation = (EventAssignmentOperation)operation;
            return new EventAssignmentOperation(Visit(eventAssignmentOperation.EventReference), Visit(eventAssignmentOperation.HandlerValue), eventAssignmentOperation.Adds, eventAssignmentOperation.OwningSemanticModel, eventAssignmentOperation.Syntax, eventAssignmentOperation.Type, eventAssignmentOperation.IsImplicit);
        }

        public override IOperation VisitConditionalAccess(IConditionalAccessOperation operation, object? argument)
        {
            ConditionalAccessOperation conditionalAccessOperation = (ConditionalAccessOperation)operation;
            return new ConditionalAccessOperation(Visit(conditionalAccessOperation.Operation), Visit(conditionalAccessOperation.WhenNotNull), conditionalAccessOperation.OwningSemanticModel, conditionalAccessOperation.Syntax, conditionalAccessOperation.Type, conditionalAccessOperation.IsImplicit);
        }

        public override IOperation VisitConditionalAccessInstance(IConditionalAccessInstanceOperation operation, object? argument)
        {
            ConditionalAccessInstanceOperation conditionalAccessInstanceOperation = (ConditionalAccessInstanceOperation)operation;
            return new ConditionalAccessInstanceOperation(conditionalAccessInstanceOperation.OwningSemanticModel, conditionalAccessInstanceOperation.Syntax, conditionalAccessInstanceOperation.Type, conditionalAccessInstanceOperation.IsImplicit);
        }

        public override IOperation VisitInterpolatedString(IInterpolatedStringOperation operation, object? argument)
        {
            InterpolatedStringOperation interpolatedStringOperation = (InterpolatedStringOperation)operation;
            return new InterpolatedStringOperation(VisitArray(interpolatedStringOperation.Parts), interpolatedStringOperation.OwningSemanticModel, interpolatedStringOperation.Syntax, interpolatedStringOperation.Type, interpolatedStringOperation.OperationConstantValue, interpolatedStringOperation.IsImplicit);
        }

        public override IOperation VisitAnonymousObjectCreation(IAnonymousObjectCreationOperation operation, object? argument)
        {
            AnonymousObjectCreationOperation anonymousObjectCreationOperation = (AnonymousObjectCreationOperation)operation;
            return new AnonymousObjectCreationOperation(VisitArray(anonymousObjectCreationOperation.Initializers), anonymousObjectCreationOperation.OwningSemanticModel, anonymousObjectCreationOperation.Syntax, anonymousObjectCreationOperation.Type, anonymousObjectCreationOperation.IsImplicit);
        }

        public override IOperation VisitObjectOrCollectionInitializer(IObjectOrCollectionInitializerOperation operation, object? argument)
        {
            ObjectOrCollectionInitializerOperation objectOrCollectionInitializerOperation = (ObjectOrCollectionInitializerOperation)operation;
            return new ObjectOrCollectionInitializerOperation(VisitArray(objectOrCollectionInitializerOperation.Initializers), objectOrCollectionInitializerOperation.OwningSemanticModel, objectOrCollectionInitializerOperation.Syntax, objectOrCollectionInitializerOperation.Type, objectOrCollectionInitializerOperation.IsImplicit);
        }

        public override IOperation VisitMemberInitializer(IMemberInitializerOperation operation, object? argument)
        {
            MemberInitializerOperation memberInitializerOperation = (MemberInitializerOperation)operation;
            return new MemberInitializerOperation(Visit(memberInitializerOperation.InitializedMember), Visit(memberInitializerOperation.Initializer), memberInitializerOperation.OwningSemanticModel, memberInitializerOperation.Syntax, memberInitializerOperation.Type, memberInitializerOperation.IsImplicit);
        }

        public override IOperation VisitNameOf(INameOfOperation operation, object? argument)
        {
            NameOfOperation nameOfOperation = (NameOfOperation)operation;
            return new NameOfOperation(Visit(nameOfOperation.Argument), nameOfOperation.OwningSemanticModel, nameOfOperation.Syntax, nameOfOperation.Type, nameOfOperation.OperationConstantValue, nameOfOperation.IsImplicit);
        }

        public override IOperation VisitTuple(ITupleOperation operation, object? argument)
        {
            TupleOperation tupleOperation = (TupleOperation)operation;
            return new TupleOperation(VisitArray(tupleOperation.Elements), tupleOperation.NaturalType, tupleOperation.OwningSemanticModel, tupleOperation.Syntax, tupleOperation.Type, tupleOperation.IsImplicit);
        }

        public override IOperation VisitDynamicMemberReference(IDynamicMemberReferenceOperation operation, object? argument)
        {
            DynamicMemberReferenceOperation dynamicMemberReferenceOperation = (DynamicMemberReferenceOperation)operation;
            return new DynamicMemberReferenceOperation(Visit(dynamicMemberReferenceOperation.Instance), dynamicMemberReferenceOperation.MemberName, dynamicMemberReferenceOperation.TypeArguments, dynamicMemberReferenceOperation.ContainingType, dynamicMemberReferenceOperation.OwningSemanticModel, dynamicMemberReferenceOperation.Syntax, dynamicMemberReferenceOperation.Type, dynamicMemberReferenceOperation.IsImplicit);
        }

        public override IOperation VisitTranslatedQuery(ITranslatedQueryOperation operation, object? argument)
        {
            TranslatedQueryOperation translatedQueryOperation = (TranslatedQueryOperation)operation;
            return new TranslatedQueryOperation(Visit(translatedQueryOperation.Operation), translatedQueryOperation.OwningSemanticModel, translatedQueryOperation.Syntax, translatedQueryOperation.Type, translatedQueryOperation.IsImplicit);
        }

        public override IOperation VisitDelegateCreation(IDelegateCreationOperation operation, object? argument)
        {
            DelegateCreationOperation delegateCreationOperation = (DelegateCreationOperation)operation;
            return new DelegateCreationOperation(Visit(delegateCreationOperation.Target), delegateCreationOperation.OwningSemanticModel, delegateCreationOperation.Syntax, delegateCreationOperation.Type, delegateCreationOperation.IsImplicit);
        }

        public override IOperation VisitDefaultValue(IDefaultValueOperation operation, object? argument)
        {
            DefaultValueOperation defaultValueOperation = (DefaultValueOperation)operation;
            return new DefaultValueOperation(defaultValueOperation.OwningSemanticModel, defaultValueOperation.Syntax, defaultValueOperation.Type, defaultValueOperation.OperationConstantValue, defaultValueOperation.IsImplicit);
        }

        public override IOperation VisitTypeOf(ITypeOfOperation operation, object? argument)
        {
            TypeOfOperation typeOfOperation = (TypeOfOperation)operation;
            return new TypeOfOperation(typeOfOperation.TypeOperand, typeOfOperation.OwningSemanticModel, typeOfOperation.Syntax, typeOfOperation.Type, typeOfOperation.IsImplicit);
        }

        public override IOperation VisitSizeOf(ISizeOfOperation operation, object? argument)
        {
            SizeOfOperation sizeOfOperation = (SizeOfOperation)operation;
            return new SizeOfOperation(sizeOfOperation.TypeOperand, sizeOfOperation.OwningSemanticModel, sizeOfOperation.Syntax, sizeOfOperation.Type, sizeOfOperation.OperationConstantValue, sizeOfOperation.IsImplicit);
        }

        public override IOperation VisitAddressOf(IAddressOfOperation operation, object? argument)
        {
            AddressOfOperation addressOfOperation = (AddressOfOperation)operation;
            return new AddressOfOperation(Visit(addressOfOperation.Reference), addressOfOperation.OwningSemanticModel, addressOfOperation.Syntax, addressOfOperation.Type, addressOfOperation.IsImplicit);
        }

        public override IOperation VisitIsPattern(IIsPatternOperation operation, object? argument)
        {
            IsPatternOperation isPatternOperation = (IsPatternOperation)operation;
            return new IsPatternOperation(Visit(isPatternOperation.Value), Visit(isPatternOperation.Pattern), isPatternOperation.OwningSemanticModel, isPatternOperation.Syntax, isPatternOperation.Type, isPatternOperation.IsImplicit);
        }

        public override IOperation VisitIncrementOrDecrement(IIncrementOrDecrementOperation operation, object? argument)
        {
            IncrementOrDecrementOperation incrementOrDecrementOperation = (IncrementOrDecrementOperation)operation;
            return new IncrementOrDecrementOperation(incrementOrDecrementOperation.IsPostfix, incrementOrDecrementOperation.IsLifted, incrementOrDecrementOperation.IsChecked, Visit(incrementOrDecrementOperation.Target), incrementOrDecrementOperation.OperatorMethod, incrementOrDecrementOperation.Kind, incrementOrDecrementOperation.OwningSemanticModel, incrementOrDecrementOperation.Syntax, incrementOrDecrementOperation.Type, incrementOrDecrementOperation.IsImplicit);
        }

        public override IOperation VisitThrow(IThrowOperation operation, object? argument)
        {
            ThrowOperation throwOperation = (ThrowOperation)operation;
            return new ThrowOperation(Visit(throwOperation.Exception), throwOperation.OwningSemanticModel, throwOperation.Syntax, throwOperation.Type, throwOperation.IsImplicit);
        }

        public override IOperation VisitDeconstructionAssignment(IDeconstructionAssignmentOperation operation, object? argument)
        {
            DeconstructionAssignmentOperation deconstructionAssignmentOperation = (DeconstructionAssignmentOperation)operation;
            return new DeconstructionAssignmentOperation(Visit(deconstructionAssignmentOperation.Target), Visit(deconstructionAssignmentOperation.Value), deconstructionAssignmentOperation.OwningSemanticModel, deconstructionAssignmentOperation.Syntax, deconstructionAssignmentOperation.Type, deconstructionAssignmentOperation.IsImplicit);
        }

        public override IOperation VisitDeclarationExpression(IDeclarationExpressionOperation operation, object? argument)
        {
            DeclarationExpressionOperation declarationExpressionOperation = (DeclarationExpressionOperation)operation;
            return new DeclarationExpressionOperation(Visit(declarationExpressionOperation.Expression), declarationExpressionOperation.OwningSemanticModel, declarationExpressionOperation.Syntax, declarationExpressionOperation.Type, declarationExpressionOperation.IsImplicit);
        }

        public override IOperation VisitOmittedArgument(IOmittedArgumentOperation operation, object? argument)
        {
            OmittedArgumentOperation omittedArgumentOperation = (OmittedArgumentOperation)operation;
            return new OmittedArgumentOperation(omittedArgumentOperation.OwningSemanticModel, omittedArgumentOperation.Syntax, omittedArgumentOperation.Type, omittedArgumentOperation.IsImplicit);
        }

        public override IOperation VisitFieldInitializer(IFieldInitializerOperation operation, object? argument)
        {
            FieldInitializerOperation fieldInitializerOperation = (FieldInitializerOperation)operation;
            return new FieldInitializerOperation(fieldInitializerOperation.InitializedFields, fieldInitializerOperation.Locals, Visit(fieldInitializerOperation.Value), fieldInitializerOperation.OwningSemanticModel, fieldInitializerOperation.Syntax, fieldInitializerOperation.IsImplicit);
        }

        public override IOperation VisitVariableInitializer(IVariableInitializerOperation operation, object? argument)
        {
            VariableInitializerOperation variableInitializerOperation = (VariableInitializerOperation)operation;
            return new VariableInitializerOperation(variableInitializerOperation.Locals, Visit(variableInitializerOperation.Value), variableInitializerOperation.OwningSemanticModel, variableInitializerOperation.Syntax, variableInitializerOperation.IsImplicit);
        }

        public override IOperation VisitPropertyInitializer(IPropertyInitializerOperation operation, object? argument)
        {
            PropertyInitializerOperation propertyInitializerOperation = (PropertyInitializerOperation)operation;
            return new PropertyInitializerOperation(propertyInitializerOperation.InitializedProperties, propertyInitializerOperation.Locals, Visit(propertyInitializerOperation.Value), propertyInitializerOperation.OwningSemanticModel, propertyInitializerOperation.Syntax, propertyInitializerOperation.IsImplicit);
        }

        public override IOperation VisitParameterInitializer(IParameterInitializerOperation operation, object? argument)
        {
            ParameterInitializerOperation parameterInitializerOperation = (ParameterInitializerOperation)operation;
            return new ParameterInitializerOperation(parameterInitializerOperation.Parameter, parameterInitializerOperation.Locals, Visit(parameterInitializerOperation.Value), parameterInitializerOperation.OwningSemanticModel, parameterInitializerOperation.Syntax, parameterInitializerOperation.IsImplicit);
        }

        public override IOperation VisitArrayInitializer(IArrayInitializerOperation operation, object? argument)
        {
            ArrayInitializerOperation arrayInitializerOperation = (ArrayInitializerOperation)operation;
            return new ArrayInitializerOperation(VisitArray(arrayInitializerOperation.ElementValues), arrayInitializerOperation.OwningSemanticModel, arrayInitializerOperation.Syntax, arrayInitializerOperation.IsImplicit);
        }

        public override IOperation VisitVariableDeclarator(IVariableDeclaratorOperation operation, object? argument)
        {
            VariableDeclaratorOperation variableDeclaratorOperation = (VariableDeclaratorOperation)operation;
            return new VariableDeclaratorOperation(variableDeclaratorOperation.Symbol, Visit(variableDeclaratorOperation.Initializer), VisitArray(variableDeclaratorOperation.IgnoredArguments), variableDeclaratorOperation.OwningSemanticModel, variableDeclaratorOperation.Syntax, variableDeclaratorOperation.IsImplicit);
        }

        public override IOperation VisitVariableDeclaration(IVariableDeclarationOperation operation, object? argument)
        {
            VariableDeclarationOperation variableDeclarationOperation = (VariableDeclarationOperation)operation;
            return new VariableDeclarationOperation(VisitArray(variableDeclarationOperation.Declarators), Visit(variableDeclarationOperation.Initializer), VisitArray(variableDeclarationOperation.IgnoredDimensions), variableDeclarationOperation.OwningSemanticModel, variableDeclarationOperation.Syntax, variableDeclarationOperation.IsImplicit);
        }

        public override IOperation VisitArgument(IArgumentOperation operation, object? argument)
        {
            ArgumentOperation argumentOperation = (ArgumentOperation)operation;
            return new ArgumentOperation(argumentOperation.ArgumentKind, argumentOperation.Parameter, Visit(argumentOperation.Value), argumentOperation.InConversionConvertible, argumentOperation.OutConversionConvertible, argumentOperation.OwningSemanticModel, argumentOperation.Syntax, argumentOperation.IsImplicit);
        }

        public override IOperation VisitCatchClause(ICatchClauseOperation operation, object? argument)
        {
            CatchClauseOperation catchClauseOperation = (CatchClauseOperation)operation;
            return new CatchClauseOperation(Visit(catchClauseOperation.ExceptionDeclarationOrExpression), catchClauseOperation.ExceptionType, catchClauseOperation.Locals, Visit(catchClauseOperation.Filter), Visit(catchClauseOperation.Handler), catchClauseOperation.OwningSemanticModel, catchClauseOperation.Syntax, catchClauseOperation.IsImplicit);
        }

        public override IOperation VisitSwitchCase(ISwitchCaseOperation operation, object? argument)
        {
            SwitchCaseOperation switchCaseOperation = (SwitchCaseOperation)operation;
            return new SwitchCaseOperation(VisitArray(switchCaseOperation.Clauses), VisitArray(switchCaseOperation.Body), switchCaseOperation.Locals, Visit(switchCaseOperation.Condition), switchCaseOperation.OwningSemanticModel, switchCaseOperation.Syntax, switchCaseOperation.IsImplicit);
        }

        public override IOperation VisitDefaultCaseClause(IDefaultCaseClauseOperation operation, object? argument)
        {
            DefaultCaseClauseOperation defaultCaseClauseOperation = (DefaultCaseClauseOperation)operation;
            return new DefaultCaseClauseOperation(defaultCaseClauseOperation.Label, defaultCaseClauseOperation.OwningSemanticModel, defaultCaseClauseOperation.Syntax, defaultCaseClauseOperation.IsImplicit);
        }

        public override IOperation VisitPatternCaseClause(IPatternCaseClauseOperation operation, object? argument)
        {
            PatternCaseClauseOperation patternCaseClauseOperation = (PatternCaseClauseOperation)operation;
            return new PatternCaseClauseOperation(patternCaseClauseOperation.Label, Visit(patternCaseClauseOperation.Pattern), Visit(patternCaseClauseOperation.Guard), patternCaseClauseOperation.OwningSemanticModel, patternCaseClauseOperation.Syntax, patternCaseClauseOperation.IsImplicit);
        }

        public override IOperation VisitRangeCaseClause(IRangeCaseClauseOperation operation, object? argument)
        {
            RangeCaseClauseOperation rangeCaseClauseOperation = (RangeCaseClauseOperation)operation;
            return new RangeCaseClauseOperation(Visit(rangeCaseClauseOperation.MinimumValue), Visit(rangeCaseClauseOperation.MaximumValue), rangeCaseClauseOperation.Label, rangeCaseClauseOperation.OwningSemanticModel, rangeCaseClauseOperation.Syntax, rangeCaseClauseOperation.IsImplicit);
        }

        public override IOperation VisitRelationalCaseClause(IRelationalCaseClauseOperation operation, object? argument)
        {
            RelationalCaseClauseOperation relationalCaseClauseOperation = (RelationalCaseClauseOperation)operation;
            return new RelationalCaseClauseOperation(Visit(relationalCaseClauseOperation.Value), relationalCaseClauseOperation.Relation, relationalCaseClauseOperation.Label, relationalCaseClauseOperation.OwningSemanticModel, relationalCaseClauseOperation.Syntax, relationalCaseClauseOperation.IsImplicit);
        }

        public override IOperation VisitSingleValueCaseClause(ISingleValueCaseClauseOperation operation, object? argument)
        {
            SingleValueCaseClauseOperation singleValueCaseClauseOperation = (SingleValueCaseClauseOperation)operation;
            return new SingleValueCaseClauseOperation(Visit(singleValueCaseClauseOperation.Value), singleValueCaseClauseOperation.Label, singleValueCaseClauseOperation.OwningSemanticModel, singleValueCaseClauseOperation.Syntax, singleValueCaseClauseOperation.IsImplicit);
        }

        public override IOperation VisitInterpolatedStringText(IInterpolatedStringTextOperation operation, object? argument)
        {
            InterpolatedStringTextOperation interpolatedStringTextOperation = (InterpolatedStringTextOperation)operation;
            return new InterpolatedStringTextOperation(Visit(interpolatedStringTextOperation.Text), interpolatedStringTextOperation.OwningSemanticModel, interpolatedStringTextOperation.Syntax, interpolatedStringTextOperation.IsImplicit);
        }

        public override IOperation VisitInterpolation(IInterpolationOperation operation, object? argument)
        {
            InterpolationOperation interpolationOperation = (InterpolationOperation)operation;
            return new InterpolationOperation(Visit(interpolationOperation.Expression), Visit(interpolationOperation.Alignment), Visit(interpolationOperation.FormatString), interpolationOperation.OwningSemanticModel, interpolationOperation.Syntax, interpolationOperation.IsImplicit);
        }

        public override IOperation VisitConstantPattern(IConstantPatternOperation operation, object? argument)
        {
            ConstantPatternOperation constantPatternOperation = (ConstantPatternOperation)operation;
            return new ConstantPatternOperation(Visit(constantPatternOperation.Value), constantPatternOperation.InputType, constantPatternOperation.NarrowedType, constantPatternOperation.OwningSemanticModel, constantPatternOperation.Syntax, constantPatternOperation.IsImplicit);
        }

        public override IOperation VisitDeclarationPattern(IDeclarationPatternOperation operation, object? argument)
        {
            DeclarationPatternOperation declarationPatternOperation = (DeclarationPatternOperation)operation;
            return new DeclarationPatternOperation(declarationPatternOperation.MatchedType, declarationPatternOperation.MatchesNull, declarationPatternOperation.DeclaredSymbol, declarationPatternOperation.InputType, declarationPatternOperation.NarrowedType, declarationPatternOperation.OwningSemanticModel, declarationPatternOperation.Syntax, declarationPatternOperation.IsImplicit);
        }

        public override IOperation VisitTupleBinaryOperator(ITupleBinaryOperation operation, object? argument)
        {
            TupleBinaryOperation tupleBinaryOperation = (TupleBinaryOperation)operation;
            return new TupleBinaryOperation(tupleBinaryOperation.OperatorKind, Visit(tupleBinaryOperation.LeftOperand), Visit(tupleBinaryOperation.RightOperand), tupleBinaryOperation.OwningSemanticModel, tupleBinaryOperation.Syntax, tupleBinaryOperation.Type, tupleBinaryOperation.IsImplicit);
        }

        public override IOperation VisitMethodBodyOperation(IMethodBodyOperation operation, object? argument)
        {
            MethodBodyOperation methodBodyOperation = (MethodBodyOperation)operation;
            return new MethodBodyOperation(Visit(methodBodyOperation.BlockBody), Visit(methodBodyOperation.ExpressionBody), methodBodyOperation.OwningSemanticModel, methodBodyOperation.Syntax, methodBodyOperation.IsImplicit);
        }

        public override IOperation VisitConstructorBodyOperation(IConstructorBodyOperation operation, object? argument)
        {
            ConstructorBodyOperation constructorBodyOperation = (ConstructorBodyOperation)operation;
            return new ConstructorBodyOperation(constructorBodyOperation.Locals, Visit(constructorBodyOperation.Initializer), Visit(constructorBodyOperation.BlockBody), Visit(constructorBodyOperation.ExpressionBody), constructorBodyOperation.OwningSemanticModel, constructorBodyOperation.Syntax, constructorBodyOperation.IsImplicit);
        }

        public override IOperation VisitDiscardOperation(IDiscardOperation operation, object? argument)
        {
            DiscardOperation discardOperation = (DiscardOperation)operation;
            return new DiscardOperation(discardOperation.DiscardSymbol, discardOperation.OwningSemanticModel, discardOperation.Syntax, discardOperation.Type, discardOperation.IsImplicit);
        }

        public override IOperation VisitFlowCaptureReference(IFlowCaptureReferenceOperation operation, object? argument)
        {
            FlowCaptureReferenceOperation flowCaptureReferenceOperation = (FlowCaptureReferenceOperation)operation;
            return new FlowCaptureReferenceOperation(flowCaptureReferenceOperation.Id, flowCaptureReferenceOperation.OwningSemanticModel, flowCaptureReferenceOperation.Syntax, flowCaptureReferenceOperation.Type, flowCaptureReferenceOperation.OperationConstantValue, flowCaptureReferenceOperation.IsImplicit);
        }

        public override IOperation VisitCoalesceAssignment(ICoalesceAssignmentOperation operation, object? argument)
        {
            CoalesceAssignmentOperation coalesceAssignmentOperation = (CoalesceAssignmentOperation)operation;
            return new CoalesceAssignmentOperation(Visit(coalesceAssignmentOperation.Target), Visit(coalesceAssignmentOperation.Value), coalesceAssignmentOperation.OwningSemanticModel, coalesceAssignmentOperation.Syntax, coalesceAssignmentOperation.Type, coalesceAssignmentOperation.IsImplicit);
        }

        public override IOperation VisitRangeOperation(IRangeOperation operation, object? argument)
        {
            RangeOperation rangeOperation = (RangeOperation)operation;
            return new RangeOperation(Visit(rangeOperation.LeftOperand), Visit(rangeOperation.RightOperand), rangeOperation.IsLifted, rangeOperation.Method, rangeOperation.OwningSemanticModel, rangeOperation.Syntax, rangeOperation.Type, rangeOperation.IsImplicit);
        }

        public override IOperation VisitReDim(IReDimOperation operation, object? argument)
        {
            ReDimOperation reDimOperation = (ReDimOperation)operation;
            return new ReDimOperation(VisitArray(reDimOperation.Clauses), reDimOperation.Preserve, reDimOperation.OwningSemanticModel, reDimOperation.Syntax, reDimOperation.IsImplicit);
        }

        public override IOperation VisitReDimClause(IReDimClauseOperation operation, object? argument)
        {
            ReDimClauseOperation reDimClauseOperation = (ReDimClauseOperation)operation;
            return new ReDimClauseOperation(Visit(reDimClauseOperation.Operand), VisitArray(reDimClauseOperation.DimensionSizes), reDimClauseOperation.OwningSemanticModel, reDimClauseOperation.Syntax, reDimClauseOperation.IsImplicit);
        }

        public override IOperation VisitRecursivePattern(IRecursivePatternOperation operation, object? argument)
        {
            RecursivePatternOperation recursivePatternOperation = (RecursivePatternOperation)operation;
            return new RecursivePatternOperation(recursivePatternOperation.MatchedType, recursivePatternOperation.DeconstructSymbol, VisitArray(recursivePatternOperation.DeconstructionSubpatterns), VisitArray(recursivePatternOperation.PropertySubpatterns), recursivePatternOperation.DeclaredSymbol, recursivePatternOperation.InputType, recursivePatternOperation.NarrowedType, recursivePatternOperation.OwningSemanticModel, recursivePatternOperation.Syntax, recursivePatternOperation.IsImplicit);
        }

        public override IOperation VisitDiscardPattern(IDiscardPatternOperation operation, object? argument)
        {
            DiscardPatternOperation discardPatternOperation = (DiscardPatternOperation)operation;
            return new DiscardPatternOperation(discardPatternOperation.InputType, discardPatternOperation.NarrowedType, discardPatternOperation.OwningSemanticModel, discardPatternOperation.Syntax, discardPatternOperation.IsImplicit);
        }

        public override IOperation VisitSwitchExpression(ISwitchExpressionOperation operation, object? argument)
        {
            SwitchExpressionOperation switchExpressionOperation = (SwitchExpressionOperation)operation;
            return new SwitchExpressionOperation(Visit(switchExpressionOperation.Value), VisitArray(switchExpressionOperation.Arms), switchExpressionOperation.OwningSemanticModel, switchExpressionOperation.Syntax, switchExpressionOperation.Type, switchExpressionOperation.IsImplicit);
        }

        public override IOperation VisitSwitchExpressionArm(ISwitchExpressionArmOperation operation, object? argument)
        {
            SwitchExpressionArmOperation switchExpressionArmOperation = (SwitchExpressionArmOperation)operation;
            return new SwitchExpressionArmOperation(Visit(switchExpressionArmOperation.Pattern), Visit(switchExpressionArmOperation.Guard), Visit(switchExpressionArmOperation.Value), switchExpressionArmOperation.Locals, switchExpressionArmOperation.OwningSemanticModel, switchExpressionArmOperation.Syntax, switchExpressionArmOperation.IsImplicit);
        }

        public override IOperation VisitPropertySubpattern(IPropertySubpatternOperation operation, object? argument)
        {
            PropertySubpatternOperation propertySubpatternOperation = (PropertySubpatternOperation)operation;
            return new PropertySubpatternOperation(Visit(propertySubpatternOperation.Member), Visit(propertySubpatternOperation.Pattern), propertySubpatternOperation.OwningSemanticModel, propertySubpatternOperation.Syntax, propertySubpatternOperation.IsImplicit);
        }

        internal override IOperation VisitAggregateQuery(IAggregateQueryOperation operation, object? argument)
        {
            AggregateQueryOperation aggregateQueryOperation = (AggregateQueryOperation)operation;
            return new AggregateQueryOperation(Visit(aggregateQueryOperation.Group), Visit(aggregateQueryOperation.Aggregation), aggregateQueryOperation.OwningSemanticModel, aggregateQueryOperation.Syntax, aggregateQueryOperation.Type, aggregateQueryOperation.IsImplicit);
        }

        internal override IOperation VisitFixed(IFixedOperation operation, object? argument)
        {
            FixedOperation fixedOperation = (FixedOperation)operation;
            return new FixedOperation(fixedOperation.Locals, Visit(fixedOperation.Variables), Visit(fixedOperation.Body), fixedOperation.OwningSemanticModel, fixedOperation.Syntax, fixedOperation.IsImplicit);
        }

        internal override IOperation VisitNoPiaObjectCreation(INoPiaObjectCreationOperation operation, object? argument)
        {
            NoPiaObjectCreationOperation noPiaObjectCreationOperation = (NoPiaObjectCreationOperation)operation;
            return new NoPiaObjectCreationOperation(Visit(noPiaObjectCreationOperation.Initializer), noPiaObjectCreationOperation.OwningSemanticModel, noPiaObjectCreationOperation.Syntax, noPiaObjectCreationOperation.Type, noPiaObjectCreationOperation.IsImplicit);
        }

        internal override IOperation VisitPlaceholder(IPlaceholderOperation operation, object? argument)
        {
            PlaceholderOperation placeholderOperation = (PlaceholderOperation)operation;
            return new PlaceholderOperation(placeholderOperation.PlaceholderKind, placeholderOperation.OwningSemanticModel, placeholderOperation.Syntax, placeholderOperation.Type, placeholderOperation.IsImplicit);
        }

        internal override IOperation VisitWithStatement(IWithStatementOperation operation, object? argument)
        {
            WithStatementOperation withStatementOperation = (WithStatementOperation)operation;
            return new WithStatementOperation(Visit(withStatementOperation.Body), Visit(withStatementOperation.Value), withStatementOperation.OwningSemanticModel, withStatementOperation.Syntax, withStatementOperation.IsImplicit);
        }

        public override IOperation VisitUsingDeclaration(IUsingDeclarationOperation operation, object? argument)
        {
            UsingDeclarationOperation usingDeclarationOperation = (UsingDeclarationOperation)operation;
            return new UsingDeclarationOperation(Visit(usingDeclarationOperation.DeclarationGroup), usingDeclarationOperation.IsAsynchronous, usingDeclarationOperation.DisposeInfo, usingDeclarationOperation.OwningSemanticModel, usingDeclarationOperation.Syntax, usingDeclarationOperation.IsImplicit);
        }

        public override IOperation VisitNegatedPattern(INegatedPatternOperation operation, object? argument)
        {
            NegatedPatternOperation negatedPatternOperation = (NegatedPatternOperation)operation;
            return new NegatedPatternOperation(Visit(negatedPatternOperation.Pattern), negatedPatternOperation.InputType, negatedPatternOperation.NarrowedType, negatedPatternOperation.OwningSemanticModel, negatedPatternOperation.Syntax, negatedPatternOperation.IsImplicit);
        }

        public override IOperation VisitBinaryPattern(IBinaryPatternOperation operation, object? argument)
        {
            BinaryPatternOperation binaryPatternOperation = (BinaryPatternOperation)operation;
            return new BinaryPatternOperation(binaryPatternOperation.OperatorKind, Visit(binaryPatternOperation.LeftPattern), Visit(binaryPatternOperation.RightPattern), binaryPatternOperation.InputType, binaryPatternOperation.NarrowedType, binaryPatternOperation.OwningSemanticModel, binaryPatternOperation.Syntax, binaryPatternOperation.IsImplicit);
        }

        public override IOperation VisitTypePattern(ITypePatternOperation operation, object? argument)
        {
            TypePatternOperation typePatternOperation = (TypePatternOperation)operation;
            return new TypePatternOperation(typePatternOperation.MatchedType, typePatternOperation.InputType, typePatternOperation.NarrowedType, typePatternOperation.OwningSemanticModel, typePatternOperation.Syntax, typePatternOperation.IsImplicit);
        }

        public override IOperation VisitRelationalPattern(IRelationalPatternOperation operation, object? argument)
        {
            RelationalPatternOperation relationalPatternOperation = (RelationalPatternOperation)operation;
            return new RelationalPatternOperation(relationalPatternOperation.OperatorKind, Visit(relationalPatternOperation.Value), relationalPatternOperation.InputType, relationalPatternOperation.NarrowedType, relationalPatternOperation.OwningSemanticModel, relationalPatternOperation.Syntax, relationalPatternOperation.IsImplicit);
        }

        public override IOperation VisitWith(IWithOperation operation, object? argument)
        {
            WithOperation withOperation = (WithOperation)operation;
            return new WithOperation(Visit(withOperation.Operand), withOperation.CloneMethod, Visit(withOperation.Initializer), withOperation.OwningSemanticModel, withOperation.Syntax, withOperation.Type, withOperation.IsImplicit);
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("operation")]
        public IOperation? Visit(IOperation? operation)
        {
            return Visit(operation, null);
        }

        internal override IOperation VisitNoneOperation(IOperation operation, object? argument)
        {
            return new NoneOperation(VisitArray(((Operation)operation).ChildOperations.ToImmutableArray()), ((Operation)operation).OwningSemanticModel, operation.Syntax, operation.Type, operation.GetConstantValue(), operation.IsImplicit);
        }

        public override IOperation VisitFlowAnonymousFunction(IFlowAnonymousFunctionOperation operation, object? argument)
        {
            FlowAnonymousFunctionOperation flowAnonymousFunctionOperation = (FlowAnonymousFunctionOperation)operation;
            return new FlowAnonymousFunctionOperation(in flowAnonymousFunctionOperation.Context, flowAnonymousFunctionOperation.Original, operation.IsImplicit);
        }

        public override IOperation VisitDynamicObjectCreation(IDynamicObjectCreationOperation operation, object? argument)
        {
            return new DynamicObjectCreationOperation(Visit(operation.Initializer), VisitArray(operation.Arguments), ((HasDynamicArgumentsExpression)operation).ArgumentNames, ((HasDynamicArgumentsExpression)operation).ArgumentRefKinds, ((Operation)operation).OwningSemanticModel, operation.Syntax, operation.Type, operation.IsImplicit);
        }

        public override IOperation VisitDynamicInvocation(IDynamicInvocationOperation operation, object? argument)
        {
            return new DynamicInvocationOperation(Visit(operation.Operation), VisitArray(operation.Arguments), ((HasDynamicArgumentsExpression)operation).ArgumentNames, ((HasDynamicArgumentsExpression)operation).ArgumentRefKinds, ((Operation)operation).OwningSemanticModel, operation.Syntax, operation.Type, operation.IsImplicit);
        }

        public override IOperation VisitDynamicIndexerAccess(IDynamicIndexerAccessOperation operation, object? argument)
        {
            return new DynamicIndexerAccessOperation(Visit(operation.Operation), VisitArray(operation.Arguments), ((HasDynamicArgumentsExpression)operation).ArgumentNames, ((HasDynamicArgumentsExpression)operation).ArgumentRefKinds, ((Operation)operation).OwningSemanticModel, operation.Syntax, operation.Type, operation.IsImplicit);
        }

        public override IOperation VisitInvalid(IInvalidOperation operation, object? argument)
        {
            return new InvalidOperation(VisitArray(((InvalidOperation)operation).Children), ((Operation)operation).OwningSemanticModel, operation.Syntax, operation.Type, operation.GetConstantValue(), operation.IsImplicit);
        }

        public override IOperation VisitFlowCapture(IFlowCaptureOperation operation, object? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitIsNull(IIsNullOperation operation, object? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitCaughtException(ICaughtExceptionOperation operation, object? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override IOperation VisitStaticLocalInitializationSemaphore(IStaticLocalInitializationSemaphoreOperation operation, object? argument)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
