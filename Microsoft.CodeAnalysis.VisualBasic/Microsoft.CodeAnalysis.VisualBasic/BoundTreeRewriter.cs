using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundTreeRewriter : BoundTreeVisitor
	{
		public virtual TypeSymbol VisitType(TypeSymbol type)
		{
			return type;
		}

		public virtual ImmutableArray<T> VisitList<T>(ImmutableArray<T> list) where T : BoundNode
		{
			ArrayBuilder<T> arrayBuilder = null;
			int i = 0;
			for (int num = ((!list.IsDefault) ? list.Length : 0); i < num; i++)
			{
				T val = list[i];
				BoundNode boundNode = Visit(val);
				if (val != boundNode && arrayBuilder == null)
				{
					arrayBuilder = ArrayBuilder<T>.GetInstance();
					if (i > 0)
					{
						arrayBuilder.AddRange(list, i);
					}
				}
				if (arrayBuilder != null && boundNode != null)
				{
					arrayBuilder.Add((T)boundNode);
				}
			}
			return arrayBuilder?.ToImmutableAndFree() ?? list;
		}

		public void VisitList<T>(ImmutableArray<T> list, ArrayBuilder<T> results) where T : BoundNode
		{
			ImmutableArray<T>.Enumerator enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				results.Add((T)Visit(current));
			}
		}

		public override BoundNode VisitTypeArguments(BoundTypeArguments node)
		{
			VisitType(node.Type);
			return node.Update(node.Arguments);
		}

		public override BoundNode VisitOmittedArgument(BoundOmittedArgument node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node)
		{
			BoundExpression underlyingLValue = (BoundExpression)Visit(node.UnderlyingLValue);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(underlyingLValue, type);
		}

		public override BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitRValuePlaceholder(BoundRValuePlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitLValuePlaceholder(BoundLValuePlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitDup(BoundDup node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.IsReference, type);
		}

		public override BoundNode VisitBadExpression(BoundBadExpression node)
		{
			ImmutableArray<BoundExpression> childBoundNodes = VisitList(node.ChildBoundNodes);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.ResultKind, node.Symbols, childBoundNodes, type);
		}

		public override BoundNode VisitBadStatement(BoundBadStatement node)
		{
			ImmutableArray<BoundNode> childBoundNodes = VisitList(node.ChildBoundNodes);
			return node.Update(childBoundNodes);
		}

		public override BoundNode VisitParenthesized(BoundParenthesized node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, type);
		}

		public override BoundNode VisitBadVariable(BoundBadVariable node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, node.IsLValue, type);
		}

		public override BoundNode VisitArrayAccess(BoundArrayAccess node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			ImmutableArray<BoundExpression> indices = VisitList(node.Indices);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, indices, node.IsLValue, type);
		}

		public override BoundNode VisitArrayLength(BoundArrayLength node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, type);
		}

		public override BoundNode VisitGetType(BoundGetType node)
		{
			BoundTypeExpression sourceType = (BoundTypeExpression)Visit(node.SourceType);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(sourceType, type);
		}

		public override BoundNode VisitFieldInfo(BoundFieldInfo node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Field, type);
		}

		public override BoundNode VisitMethodInfo(BoundMethodInfo node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Method, type);
		}

		public override BoundNode VisitTypeExpression(BoundTypeExpression node)
		{
			BoundExpression unevaluatedReceiverOpt = node.UnevaluatedReceiverOpt;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(unevaluatedReceiverOpt, node.AliasOpt, type);
		}

		public override BoundNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Data, type);
		}

		public override BoundNode VisitNamespaceExpression(BoundNamespaceExpression node)
		{
			BoundExpression unevaluatedReceiverOpt = (BoundExpression)Visit(node.UnevaluatedReceiverOpt);
			VisitType(node.Type);
			return node.Update(unevaluatedReceiverOpt, node.AliasOpt, node.NamespaceSymbol);
		}

		public override BoundNode VisitMethodDefIndex(BoundMethodDefIndex node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Method, type);
		}

		public override BoundNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.AnalysisKind, node.IsLValue, type);
		}

		public override BoundNode VisitModuleVersionId(BoundModuleVersionId node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.IsLValue, type);
		}

		public override BoundNode VisitModuleVersionIdString(BoundModuleVersionIdString node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Document, type);
		}

		public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.OperatorKind, operand, node.Checked, node.ConstantValueOpt, type);
		}

		public override BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			BoundExpression underlyingExpression = (BoundExpression)Visit(node.UnderlyingExpression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.OperatorKind, underlyingExpression, type);
		}

		public override BoundNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(operand, type);
		}

		public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
		{
			BoundExpression left = (BoundExpression)Visit(node.Left);
			BoundExpression right = (BoundExpression)Visit(node.Right);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.OperatorKind, left, right, node.Checked, node.ConstantValueOpt, type);
		}

		public override BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			BoundExpression underlyingExpression = (BoundExpression)Visit(node.UnderlyingExpression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.OperatorKind, underlyingExpression, node.Checked, type);
		}

		public override BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			BoundExpression leftOperand = (BoundExpression)Visit(node.LeftOperand);
			BoundRValuePlaceholder leftOperandPlaceholder = (BoundRValuePlaceholder)Visit(node.LeftOperandPlaceholder);
			BoundExpression leftTest = (BoundExpression)Visit(node.LeftTest);
			BoundUserDefinedBinaryOperator bitwiseOperator = (BoundUserDefinedBinaryOperator)Visit(node.BitwiseOperator);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(leftOperand, leftOperandPlaceholder, leftTest, bitwiseOperator, type);
		}

		public override BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			BoundExpression left = (BoundExpression)Visit(node.Left);
			BoundCompoundAssignmentTargetPlaceholder leftOnTheRightOpt = (BoundCompoundAssignmentTargetPlaceholder)Visit(node.LeftOnTheRightOpt);
			BoundExpression right = (BoundExpression)Visit(node.Right);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(left, leftOnTheRightOpt, right, node.SuppressObjectClone, type);
		}

		public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
		{
			BoundLocal byRefLocal = (BoundLocal)Visit(node.ByRefLocal);
			BoundExpression lValue = (BoundExpression)Visit(node.LValue);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(byRefLocal, lValue, node.IsLValue, type);
		}

		public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
		{
			BoundMethodGroup methodGroup = (BoundMethodGroup)Visit(node.MethodGroup);
			VisitType(node.Type);
			return node.Update(node.Binder, node.WithDependencies, methodGroup);
		}

		public override BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			BoundExpression condition = (BoundExpression)Visit(node.Condition);
			BoundExpression whenTrue = (BoundExpression)Visit(node.WhenTrue);
			BoundExpression whenFalse = (BoundExpression)Visit(node.WhenFalse);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(condition, whenTrue, whenFalse, node.ConstantValueOpt, type);
		}

		public override BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			BoundExpression testExpression = (BoundExpression)Visit(node.TestExpression);
			BoundExpression convertedTestExpression = node.ConvertedTestExpression;
			BoundRValuePlaceholder testExpressionPlaceholder = node.TestExpressionPlaceholder;
			BoundExpression elseExpression = (BoundExpression)Visit(node.ElseExpression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(testExpression, convertedTestExpression, testExpressionPlaceholder, elseExpression, node.ConstantValueOpt, type);
		}

		public override BoundNode VisitConversion(BoundConversion node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			BoundExtendedConversionInfo extendedInfoOpt = (BoundExtendedConversionInfo)Visit(node.ExtendedInfoOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(operand, node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, extendedInfoOpt, type);
		}

		public override BoundNode VisitRelaxationLambda(BoundRelaxationLambda node)
		{
			BoundLambda lambda = (BoundLambda)Visit(node.Lambda);
			BoundRValuePlaceholder receiverPlaceholderOpt = (BoundRValuePlaceholder)Visit(node.ReceiverPlaceholderOpt);
			return node.Update(lambda, receiverPlaceholderOpt);
		}

		public override BoundNode VisitConvertedTupleElements(BoundConvertedTupleElements node)
		{
			ImmutableArray<BoundRValuePlaceholder> elementPlaceholders = VisitList(node.ElementPlaceholders);
			ImmutableArray<BoundExpression> convertedElements = VisitList(node.ConvertedElements);
			return node.Update(elementPlaceholders, convertedElements);
		}

		public override BoundNode VisitUserDefinedConversion(BoundUserDefinedConversion node)
		{
			BoundExpression underlyingExpression = (BoundExpression)Visit(node.UnderlyingExpression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(underlyingExpression, node.InOutConversionFlags, type);
		}

		public override BoundNode VisitDirectCast(BoundDirectCast node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			BoundLambda relaxationLambdaOpt = (BoundLambda)Visit(node.RelaxationLambdaOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(operand, node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, relaxationLambdaOpt, type);
		}

		public override BoundNode VisitTryCast(BoundTryCast node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			BoundLambda relaxationLambdaOpt = (BoundLambda)Visit(node.RelaxationLambdaOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(operand, node.ConversionKind, node.ConstantValueOpt, relaxationLambdaOpt, type);
		}

		public override BoundNode VisitTypeOf(BoundTypeOf node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			TypeSymbol targetType = VisitType(node.TargetType);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(operand, node.IsTypeOfIsNotExpression, targetType, type);
		}

		public override BoundNode VisitSequencePoint(BoundSequencePoint node)
		{
			BoundStatement statementOpt = (BoundStatement)Visit(node.StatementOpt);
			return node.Update(statementOpt);
		}

		public override BoundNode VisitSequencePointExpression(BoundSequencePointExpression node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, type);
		}

		public override BoundNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
		{
			BoundStatement statementOpt = (BoundStatement)Visit(node.StatementOpt);
			return node.Update(statementOpt, node.Span);
		}

		public override BoundNode VisitNoOpStatement(BoundNoOpStatement node)
		{
			return node;
		}

		public override BoundNode VisitMethodGroup(BoundMethodGroup node)
		{
			BoundTypeArguments typeArgumentsOpt = (BoundTypeArguments)Visit(node.TypeArgumentsOpt);
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			VisitType(node.Type);
			return node.Update(typeArgumentsOpt, node.Methods, node.PendingExtensionMethodsOpt, node.ResultKind, receiverOpt, node.QualificationKind);
		}

		public override BoundNode VisitPropertyGroup(BoundPropertyGroup node)
		{
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			VisitType(node.Type);
			return node.Update(node.Properties, node.ResultKind, receiverOpt, node.QualificationKind);
		}

		public override BoundNode VisitReturnStatement(BoundReturnStatement node)
		{
			BoundExpression expressionOpt = (BoundExpression)Visit(node.ExpressionOpt);
			return node.Update(expressionOpt, node.FunctionLocalOpt, node.ExitLabelOpt);
		}

		public override BoundNode VisitYieldStatement(BoundYieldStatement node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			return node.Update(expression);
		}

		public override BoundNode VisitThrowStatement(BoundThrowStatement node)
		{
			BoundExpression expressionOpt = (BoundExpression)Visit(node.ExpressionOpt);
			return node.Update(expressionOpt);
		}

		public override BoundNode VisitRedimStatement(BoundRedimStatement node)
		{
			ImmutableArray<BoundRedimClause> clauses = VisitList(node.Clauses);
			return node.Update(clauses);
		}

		public override BoundNode VisitRedimClause(BoundRedimClause node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			ImmutableArray<BoundExpression> indices = VisitList(node.Indices);
			return node.Update(operand, indices, node.ArrayTypeOpt, node.Preserve);
		}

		public override BoundNode VisitEraseStatement(BoundEraseStatement node)
		{
			ImmutableArray<BoundAssignmentOperator> clauses = VisitList(node.Clauses);
			return node.Update(clauses);
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Method, methodGroupOpt, receiverOpt, arguments, node.DefaultArguments, node.ConstantValueOpt, node.IsLValue, node.SuppressObjectClone, type);
		}

		public override BoundNode VisitAttribute(BoundAttribute node)
		{
			ImmutableArray<BoundExpression> constructorArguments = VisitList(node.ConstructorArguments);
			ImmutableArray<BoundExpression> namedArguments = VisitList(node.NamedArguments);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Constructor, constructorArguments, namedArguments, node.ResultKind, type);
		}

		public override BoundNode VisitLateMemberAccess(BoundLateMemberAccess node)
		{
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			BoundTypeArguments typeArgumentsOpt = (BoundTypeArguments)Visit(node.TypeArgumentsOpt);
			TypeSymbol containerTypeOpt = VisitType(node.ContainerTypeOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.NameOpt, containerTypeOpt, receiverOpt, typeArgumentsOpt, node.AccessKind, type);
		}

		public override BoundNode VisitLateInvocation(BoundLateInvocation node)
		{
			BoundExpression member = (BoundExpression)Visit(node.Member);
			ImmutableArray<BoundExpression> argumentsOpt = VisitList(node.ArgumentsOpt);
			BoundMethodOrPropertyGroup methodOrPropertyGroupOpt = node.MethodOrPropertyGroupOpt;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(member, argumentsOpt, node.ArgumentNamesOpt, node.AccessKind, methodOrPropertyGroupOpt, type);
		}

		public override BoundNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node)
		{
			BoundLateMemberAccess memberAccess = (BoundLateMemberAccess)Visit(node.MemberAccess);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Binder, memberAccess, type);
		}

		public override BoundNode VisitTupleLiteral(BoundTupleLiteral node)
		{
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.InferredType, node.ArgumentNamesOpt, node.InferredNamesOpt, arguments, type);
		}

		public override BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
		{
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			TypeSymbol naturalTypeOpt = VisitType(node.NaturalTypeOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(naturalTypeOpt, arguments, type);
		}

		public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			BoundObjectInitializerExpressionBase initializerOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.ConstructorOpt, methodGroupOpt, arguments, node.DefaultArguments, initializerOpt, type);
		}

		public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
		{
			BoundObjectInitializerExpressionBase initializerOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.GuidString, initializerOpt, type);
		}

		public override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			ImmutableArray<BoundAnonymousTypePropertyAccess> declarations = VisitList(node.Declarations);
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.BinderOpt, declarations, arguments, type);
		}

		public override BoundNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Binder, node.PropertyIndex, type);
		}

		public override BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
		{
			BoundExpression value = (BoundExpression)Visit(node.Value);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Binder, value, type);
		}

		public override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
		{
			BoundWithLValueExpressionPlaceholder placeholderOpt = (BoundWithLValueExpressionPlaceholder)Visit(node.PlaceholderOpt);
			ImmutableArray<BoundExpression> initializers = VisitList(node.Initializers);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.CreateTemporaryLocalForInitialization, node.Binder, placeholderOpt, initializers, type);
		}

		public override BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
		{
			BoundWithLValueExpressionPlaceholder placeholderOpt = (BoundWithLValueExpressionPlaceholder)Visit(node.PlaceholderOpt);
			ImmutableArray<BoundExpression> initializers = VisitList(node.Initializers);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(placeholderOpt, initializers, type);
		}

		public override BoundNode VisitNewT(BoundNewT node)
		{
			BoundObjectInitializerExpressionBase initializerOpt = (BoundObjectInitializerExpressionBase)Visit(node.InitializerOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(initializerOpt, type);
		}

		public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			BoundLambda relaxationLambdaOpt = (BoundLambda)Visit(node.RelaxationLambdaOpt);
			BoundRValuePlaceholder relaxationReceiverPlaceholderOpt = (BoundRValuePlaceholder)Visit(node.RelaxationReceiverPlaceholderOpt);
			BoundMethodGroup methodGroupOpt = node.MethodGroupOpt;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(receiverOpt, node.Method, relaxationLambdaOpt, relaxationReceiverPlaceholderOpt, methodGroupOpt, type);
		}

		public override BoundNode VisitArrayCreation(BoundArrayCreation node)
		{
			ImmutableArray<BoundExpression> bounds = VisitList(node.Bounds);
			BoundArrayInitialization initializerOpt = (BoundArrayInitialization)Visit(node.InitializerOpt);
			BoundArrayLiteral arrayLiteralOpt = node.ArrayLiteralOpt;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.IsParamArrayArgument, bounds, initializerOpt, arrayLiteralOpt, node.ArrayLiteralConversion, type);
		}

		public override BoundNode VisitArrayLiteral(BoundArrayLiteral node)
		{
			ImmutableArray<BoundExpression> bounds = VisitList(node.Bounds);
			BoundArrayInitialization initializer = (BoundArrayInitialization)Visit(node.Initializer);
			VisitType(node.Type);
			return node.Update(node.HasDominantType, node.NumberOfCandidates, node.InferredType, bounds, initializer, node.Binder);
		}

		public override BoundNode VisitArrayInitialization(BoundArrayInitialization node)
		{
			ImmutableArray<BoundExpression> initializers = VisitList(node.Initializers);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(initializers, type);
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(receiverOpt, node.FieldSymbol, node.IsLValue, node.SuppressVirtualCalls, node.ConstantsInProgressOpt, type);
		}

		public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			BoundPropertyGroup propertyGroupOpt = node.PropertyGroupOpt;
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			ImmutableArray<BoundExpression> arguments = VisitList(node.Arguments);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.PropertySymbol, propertyGroupOpt, node.AccessKind, node.IsWriteable, node.IsLValue, receiverOpt, arguments, node.DefaultArguments, type);
		}

		public override BoundNode VisitEventAccess(BoundEventAccess node)
		{
			BoundExpression receiverOpt = (BoundExpression)Visit(node.ReceiverOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(receiverOpt, node.EventSymbol, type);
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
			return node.Update(node.StatementListSyntax, node.Locals, statements);
		}

		public override BoundNode VisitStateMachineScope(BoundStateMachineScope node)
		{
			BoundStatement statement = (BoundStatement)Visit(node.Statement);
			return node.Update(node.Fields, statement);
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			BoundExpression declarationInitializerOpt = (BoundExpression)Visit(node.DeclarationInitializerOpt);
			BoundArrayCreation identifierInitializerOpt = (BoundArrayCreation)Visit(node.IdentifierInitializerOpt);
			return node.Update(node.LocalSymbol, declarationInitializerOpt, identifierInitializerOpt, node.InitializedByAsNew);
		}

		public override BoundNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node)
		{
			ImmutableArray<BoundLocalDeclaration> localDeclarations = VisitList(node.LocalDeclarations);
			BoundExpression initializer = (BoundExpression)Visit(node.Initializer);
			return node.Update(localDeclarations, initializer);
		}

		public override BoundNode VisitDimStatement(BoundDimStatement node)
		{
			ImmutableArray<BoundLocalDeclarationBase> localDeclarations = VisitList(node.LocalDeclarations);
			BoundExpression initializerOpt = (BoundExpression)Visit(node.InitializerOpt);
			return node.Update(localDeclarations, initializerOpt);
		}

		public override BoundNode VisitInitializer(BoundInitializer node)
		{
			return node;
		}

		public override BoundNode VisitFieldInitializer(BoundFieldInitializer node)
		{
			BoundExpression memberAccessExpressionOpt = (BoundExpression)Visit(node.MemberAccessExpressionOpt);
			BoundExpression initialValue = (BoundExpression)Visit(node.InitialValue);
			return node.Update(node.InitializedFields, memberAccessExpressionOpt, initialValue);
		}

		public override BoundNode VisitPropertyInitializer(BoundPropertyInitializer node)
		{
			BoundExpression memberAccessExpressionOpt = (BoundExpression)Visit(node.MemberAccessExpressionOpt);
			BoundExpression initialValue = (BoundExpression)Visit(node.InitialValue);
			return node.Update(node.InitializedProperties, memberAccessExpressionOpt, initialValue);
		}

		public override BoundNode VisitParameterEqualsValue(BoundParameterEqualsValue node)
		{
			BoundExpression value = (BoundExpression)Visit(node.Value);
			return node.Update(node.Parameter, value);
		}

		public override BoundNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
		{
			BoundStatement statement = (BoundStatement)Visit(node.Statement);
			return node.Update(statement);
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			ImmutableArray<BoundExpression> sideEffects = VisitList(node.SideEffects);
			BoundExpression valueOpt = (BoundExpression)Visit(node.ValueOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Locals, sideEffects, valueOpt, type);
		}

		public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			return node.Update(expression);
		}

		public override BoundNode VisitIfStatement(BoundIfStatement node)
		{
			BoundExpression condition = (BoundExpression)Visit(node.Condition);
			BoundStatement consequence = (BoundStatement)Visit(node.Consequence);
			BoundStatement alternativeOpt = (BoundStatement)Visit(node.AlternativeOpt);
			return node.Update(condition, consequence, alternativeOpt);
		}

		public override BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			BoundExpressionStatement expressionStatement = (BoundExpressionStatement)Visit(node.ExpressionStatement);
			BoundRValuePlaceholder exprPlaceholderOpt = (BoundRValuePlaceholder)Visit(node.ExprPlaceholderOpt);
			ImmutableArray<BoundCaseBlock> caseBlocks = VisitList(node.CaseBlocks);
			return node.Update(expressionStatement, exprPlaceholderOpt, caseBlocks, node.RecommendSwitchTable, node.ExitLabel);
		}

		public override BoundNode VisitCaseBlock(BoundCaseBlock node)
		{
			BoundCaseStatement caseStatement = (BoundCaseStatement)Visit(node.CaseStatement);
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(caseStatement, body);
		}

		public override BoundNode VisitCaseStatement(BoundCaseStatement node)
		{
			ImmutableArray<BoundCaseClause> caseClauses = VisitList(node.CaseClauses);
			BoundExpression conditionOpt = (BoundExpression)Visit(node.ConditionOpt);
			return node.Update(caseClauses, conditionOpt);
		}

		public override BoundNode VisitSimpleCaseClause(BoundSimpleCaseClause node)
		{
			BoundExpression valueOpt = (BoundExpression)Visit(node.ValueOpt);
			BoundExpression conditionOpt = (BoundExpression)Visit(node.ConditionOpt);
			return node.Update(valueOpt, conditionOpt);
		}

		public override BoundNode VisitRangeCaseClause(BoundRangeCaseClause node)
		{
			BoundExpression lowerBoundOpt = (BoundExpression)Visit(node.LowerBoundOpt);
			BoundExpression upperBoundOpt = (BoundExpression)Visit(node.UpperBoundOpt);
			BoundExpression lowerBoundConditionOpt = (BoundExpression)Visit(node.LowerBoundConditionOpt);
			BoundExpression upperBoundConditionOpt = (BoundExpression)Visit(node.UpperBoundConditionOpt);
			return node.Update(lowerBoundOpt, upperBoundOpt, lowerBoundConditionOpt, upperBoundConditionOpt);
		}

		public override BoundNode VisitRelationalCaseClause(BoundRelationalCaseClause node)
		{
			BoundExpression valueOpt = (BoundExpression)Visit(node.ValueOpt);
			BoundExpression conditionOpt = (BoundExpression)Visit(node.ConditionOpt);
			return node.Update(node.OperatorKind, valueOpt, conditionOpt);
		}

		public override BoundNode VisitDoLoopStatement(BoundDoLoopStatement node)
		{
			BoundExpression topConditionOpt = (BoundExpression)Visit(node.TopConditionOpt);
			BoundExpression bottomConditionOpt = (BoundExpression)Visit(node.BottomConditionOpt);
			BoundStatement body = (BoundStatement)Visit(node.Body);
			return node.Update(topConditionOpt, bottomConditionOpt, node.TopConditionIsUntil, node.BottomConditionIsUntil, body, node.ContinueLabel, node.ExitLabel);
		}

		public override BoundNode VisitWhileStatement(BoundWhileStatement node)
		{
			BoundExpression condition = (BoundExpression)Visit(node.Condition);
			BoundStatement body = (BoundStatement)Visit(node.Body);
			return node.Update(condition, body, node.ContinueLabel, node.ExitLabel);
		}

		public override BoundNode VisitForToUserDefinedOperators(BoundForToUserDefinedOperators node)
		{
			BoundRValuePlaceholder leftOperandPlaceholder = (BoundRValuePlaceholder)Visit(node.LeftOperandPlaceholder);
			BoundRValuePlaceholder rightOperandPlaceholder = (BoundRValuePlaceholder)Visit(node.RightOperandPlaceholder);
			BoundUserDefinedBinaryOperator addition = (BoundUserDefinedBinaryOperator)Visit(node.Addition);
			BoundUserDefinedBinaryOperator subtraction = (BoundUserDefinedBinaryOperator)Visit(node.Subtraction);
			BoundExpression lessThanOrEqual = (BoundExpression)Visit(node.LessThanOrEqual);
			BoundExpression greaterThanOrEqual = (BoundExpression)Visit(node.GreaterThanOrEqual);
			return node.Update(leftOperandPlaceholder, rightOperandPlaceholder, addition, subtraction, lessThanOrEqual, greaterThanOrEqual);
		}

		public override BoundNode VisitForToStatement(BoundForToStatement node)
		{
			BoundExpression initialValue = (BoundExpression)Visit(node.InitialValue);
			BoundExpression limitValue = (BoundExpression)Visit(node.LimitValue);
			BoundExpression stepValue = (BoundExpression)Visit(node.StepValue);
			BoundForToUserDefinedOperators operatorsOpt = (BoundForToUserDefinedOperators)Visit(node.OperatorsOpt);
			BoundExpression controlVariable = (BoundExpression)Visit(node.ControlVariable);
			BoundStatement body = (BoundStatement)Visit(node.Body);
			ImmutableArray<BoundExpression> nextVariablesOpt = VisitList(node.NextVariablesOpt);
			return node.Update(initialValue, limitValue, stepValue, node.Checked, operatorsOpt, node.DeclaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, node.ContinueLabel, node.ExitLabel);
		}

		public override BoundNode VisitForEachStatement(BoundForEachStatement node)
		{
			BoundExpression collection = (BoundExpression)Visit(node.Collection);
			BoundExpression controlVariable = (BoundExpression)Visit(node.ControlVariable);
			BoundStatement body = (BoundStatement)Visit(node.Body);
			ImmutableArray<BoundExpression> nextVariablesOpt = VisitList(node.NextVariablesOpt);
			return node.Update(collection, node.EnumeratorInfo, node.DeclaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, node.ContinueLabel, node.ExitLabel);
		}

		public override BoundNode VisitExitStatement(BoundExitStatement node)
		{
			return node;
		}

		public override BoundNode VisitContinueStatement(BoundContinueStatement node)
		{
			return node;
		}

		public override BoundNode VisitTryStatement(BoundTryStatement node)
		{
			BoundBlock tryBlock = (BoundBlock)Visit(node.TryBlock);
			ImmutableArray<BoundCatchBlock> catchBlocks = VisitList(node.CatchBlocks);
			BoundBlock finallyBlockOpt = (BoundBlock)Visit(node.FinallyBlockOpt);
			return node.Update(tryBlock, catchBlocks, finallyBlockOpt, node.ExitLabelOpt);
		}

		public override BoundNode VisitCatchBlock(BoundCatchBlock node)
		{
			BoundExpression exceptionSourceOpt = (BoundExpression)Visit(node.ExceptionSourceOpt);
			BoundExpression errorLineNumberOpt = (BoundExpression)Visit(node.ErrorLineNumberOpt);
			BoundExpression exceptionFilterOpt = (BoundExpression)Visit(node.ExceptionFilterOpt);
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(node.LocalOpt, exceptionSourceOpt, errorLineNumberOpt, exceptionFilterOpt, body, node.IsSynthesizedAsyncCatchAll);
		}

		public override BoundNode VisitLiteral(BoundLiteral node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Value, type);
		}

		public override BoundNode VisitMeReference(BoundMeReference node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitValueTypeMeReference(BoundValueTypeMeReference node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitMyClassReference(BoundMyClassReference node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.SourceType, type);
		}

		public override BoundNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(type);
		}

		public override BoundNode VisitLocal(BoundLocal node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.LocalSymbol, node.IsLValue, type);
		}

		public override BoundNode VisitPseudoVariable(BoundPseudoVariable node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.LocalSymbol, node.IsLValue, node.EmitExpressions, type);
		}

		public override BoundNode VisitParameter(BoundParameter node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.ParameterSymbol, node.IsLValue, node.SuppressVirtualCalls, type);
		}

		public override BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.IsOut, type);
		}

		public override BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
		{
			BoundExpression originalArgument = (BoundExpression)Visit(node.OriginalArgument);
			BoundExpression inConversion = (BoundExpression)Visit(node.InConversion);
			BoundByRefArgumentPlaceholder inPlaceholder = (BoundByRefArgumentPlaceholder)Visit(node.InPlaceholder);
			BoundExpression outConversion = (BoundExpression)Visit(node.OutConversion);
			BoundRValuePlaceholder outPlaceholder = (BoundRValuePlaceholder)Visit(node.OutPlaceholder);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(originalArgument, inConversion, inPlaceholder, outConversion, outPlaceholder, type);
		}

		public override BoundNode VisitLateBoundArgumentSupportingAssignmentWithCapture(BoundLateBoundArgumentSupportingAssignmentWithCapture node)
		{
			BoundExpression originalArgument = (BoundExpression)Visit(node.OriginalArgument);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(originalArgument, node.LocalSymbol, type);
		}

		public override BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			return node;
		}

		public override BoundNode VisitLabel(BoundLabel node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Label, type);
		}

		public override BoundNode VisitGotoStatement(BoundGotoStatement node)
		{
			BoundLabel labelExpressionOpt = (BoundLabel)Visit(node.LabelExpressionOpt);
			return node.Update(node.Label, labelExpressionOpt);
		}

		public override BoundNode VisitStatementList(BoundStatementList node)
		{
			ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
			return node.Update(statements);
		}

		public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
		{
			BoundExpression condition = (BoundExpression)Visit(node.Condition);
			return node.Update(condition, node.JumpIfTrue, node.Label);
		}

		public override BoundNode VisitWithStatement(BoundWithStatement node)
		{
			BoundExpression originalExpression = (BoundExpression)Visit(node.OriginalExpression);
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(originalExpression, body, node.Binder);
		}

		public override BoundNode VisitUnboundLambda(UnboundLambda node)
		{
			TypeSymbol returnType = VisitType(node.ReturnType);
			VisitType(node.Type);
			return node.Update(node.Binder, node.Flags, node.Parameters, returnType, node.BindingCache);
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			BoundBlock body = (BoundBlock)Visit(node.Body);
			VisitType(node.Type);
			return node.Update(node.LambdaSymbol, body, node.Diagnostics, node.LambdaBinderOpt, node.DelegateRelaxation, node.MethodConversionKind);
		}

		public override BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			BoundQueryClauseBase lastOperator = (BoundQueryClauseBase)Visit(node.LastOperator);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(lastOperator, type);
		}

		public override BoundNode VisitQuerySource(BoundQuerySource node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, type);
		}

		public override BoundNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node)
		{
			BoundCall conversionCall = (BoundCall)Visit(node.ConversionCall);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(conversionCall, type);
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			BoundQueryPart source = (BoundQueryPart)Visit(node.Source);
			TypeSymbol compoundVariableType = VisitType(node.CompoundVariableType);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(source, node.RangeVariableOpt, node.RangeVariables, compoundVariableType, node.Binders, type);
		}

		public override BoundNode VisitQueryClause(BoundQueryClause node)
		{
			BoundExpression underlyingExpression = (BoundExpression)Visit(node.UnderlyingExpression);
			TypeSymbol compoundVariableType = VisitType(node.CompoundVariableType);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(underlyingExpression, node.RangeVariables, compoundVariableType, node.Binders, type);
		}

		public override BoundNode VisitOrdering(BoundOrdering node)
		{
			BoundExpression underlyingExpression = (BoundExpression)Visit(node.UnderlyingExpression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(underlyingExpression, type);
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			VisitType(node.Type);
			return node.Update(node.LambdaSymbol, node.RangeVariables, expression, node.ExprIsOperandOfConditionalBranch);
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			BoundExpression value = (BoundExpression)Visit(node.Value);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.RangeVariable, value, type);
		}

		public override BoundNode VisitGroupTypeInferenceLambda(GroupTypeInferenceLambda node)
		{
			VisitType(node.Type);
			return node.Update(node.Binder, node.Parameters, node.Compilation);
		}

		public override BoundNode VisitAggregateClause(BoundAggregateClause node)
		{
			BoundQueryClauseBase capturedGroupOpt = (BoundQueryClauseBase)Visit(node.CapturedGroupOpt);
			BoundRValuePlaceholder groupPlaceholderOpt = (BoundRValuePlaceholder)Visit(node.GroupPlaceholderOpt);
			BoundExpression underlyingExpression = (BoundExpression)Visit(node.UnderlyingExpression);
			TypeSymbol compoundVariableType = VisitType(node.CompoundVariableType);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(capturedGroupOpt, groupPlaceholderOpt, underlyingExpression, node.RangeVariables, compoundVariableType, node.Binders, type);
		}

		public override BoundNode VisitGroupAggregation(BoundGroupAggregation node)
		{
			BoundExpression group = (BoundExpression)Visit(node.Group);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(group, type);
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.RangeVariable, type);
		}

		public override BoundNode VisitAddHandlerStatement(BoundAddHandlerStatement node)
		{
			BoundExpression eventAccess = (BoundExpression)Visit(node.EventAccess);
			BoundExpression handler = (BoundExpression)Visit(node.Handler);
			return node.Update(eventAccess, handler);
		}

		public override BoundNode VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node)
		{
			BoundExpression eventAccess = (BoundExpression)Visit(node.EventAccess);
			BoundExpression handler = (BoundExpression)Visit(node.Handler);
			return node.Update(eventAccess, handler);
		}

		public override BoundNode VisitRaiseEventStatement(BoundRaiseEventStatement node)
		{
			BoundExpression eventInvocation = (BoundExpression)Visit(node.EventInvocation);
			return node.Update(node.EventSymbol, eventInvocation);
		}

		public override BoundNode VisitUsingStatement(BoundUsingStatement node)
		{
			ImmutableArray<BoundLocalDeclarationBase> resourceList = VisitList(node.ResourceList);
			BoundExpression resourceExpressionOpt = (BoundExpression)Visit(node.ResourceExpressionOpt);
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(resourceList, resourceExpressionOpt, body, node.UsingInfo, node.Locals);
		}

		public override BoundNode VisitSyncLockStatement(BoundSyncLockStatement node)
		{
			BoundExpression lockExpression = (BoundExpression)Visit(node.LockExpression);
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(lockExpression, body);
		}

		public override BoundNode VisitXmlName(BoundXmlName node)
		{
			BoundExpression xmlNamespace = (BoundExpression)Visit(node.XmlNamespace);
			BoundExpression localName = (BoundExpression)Visit(node.LocalName);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(xmlNamespace, localName, objectCreation, type);
		}

		public override BoundNode VisitXmlNamespace(BoundXmlNamespace node)
		{
			BoundExpression xmlNamespace = (BoundExpression)Visit(node.XmlNamespace);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(xmlNamespace, objectCreation, type);
		}

		public override BoundNode VisitXmlDocument(BoundXmlDocument node)
		{
			BoundExpression declaration = (BoundExpression)Visit(node.Declaration);
			ImmutableArray<BoundExpression> childNodes = VisitList(node.ChildNodes);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(declaration, childNodes, node.RewriterInfo, type);
		}

		public override BoundNode VisitXmlDeclaration(BoundXmlDeclaration node)
		{
			BoundExpression version = (BoundExpression)Visit(node.Version);
			BoundExpression encoding = (BoundExpression)Visit(node.Encoding);
			BoundExpression standalone = (BoundExpression)Visit(node.Standalone);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(version, encoding, standalone, objectCreation, type);
		}

		public override BoundNode VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node)
		{
			BoundExpression target = (BoundExpression)Visit(node.Target);
			BoundExpression data = (BoundExpression)Visit(node.Data);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(target, data, objectCreation, type);
		}

		public override BoundNode VisitXmlComment(BoundXmlComment node)
		{
			BoundExpression value = (BoundExpression)Visit(node.Value);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(value, objectCreation, type);
		}

		public override BoundNode VisitXmlAttribute(BoundXmlAttribute node)
		{
			BoundExpression name = (BoundExpression)Visit(node.Name);
			BoundExpression value = (BoundExpression)Visit(node.Value);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(name, value, node.MatchesImport, objectCreation, type);
		}

		public override BoundNode VisitXmlElement(BoundXmlElement node)
		{
			BoundExpression argument = (BoundExpression)Visit(node.Argument);
			ImmutableArray<BoundExpression> childNodes = VisitList(node.ChildNodes);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(argument, childNodes, node.RewriterInfo, type);
		}

		public override BoundNode VisitXmlMemberAccess(BoundXmlMemberAccess node)
		{
			BoundExpression memberAccess = (BoundExpression)Visit(node.MemberAccess);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(memberAccess, type);
		}

		public override BoundNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, type);
		}

		public override BoundNode VisitXmlCData(BoundXmlCData node)
		{
			BoundLiteral value = (BoundLiteral)Visit(node.Value);
			BoundExpression objectCreation = node.ObjectCreation;
			TypeSymbol type = VisitType(node.Type);
			return node.Update(value, objectCreation, type);
		}

		public override BoundNode VisitResumeStatement(BoundResumeStatement node)
		{
			BoundExpression labelExpressionOpt = (BoundExpression)Visit(node.LabelExpressionOpt);
			return node.Update(node.ResumeKind, node.LabelOpt, labelExpressionOpt);
		}

		public override BoundNode VisitOnErrorStatement(BoundOnErrorStatement node)
		{
			BoundExpression labelExpressionOpt = (BoundExpression)Visit(node.LabelExpressionOpt);
			return node.Update(node.OnErrorKind, node.LabelOpt, labelExpressionOpt);
		}

		public override BoundNode VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node)
		{
			BoundBlock body = (BoundBlock)Visit(node.Body);
			return node.Update(node.ContainsOnError, node.ContainsResume, node.ResumeWithoutLabelOpt, node.TrackLineNumber, body);
		}

		public override BoundNode VisitUnstructuredExceptionHandlingCatchFilter(BoundUnstructuredExceptionHandlingCatchFilter node)
		{
			BoundLocal activeHandlerLocal = (BoundLocal)Visit(node.ActiveHandlerLocal);
			BoundLocal resumeTargetLocal = (BoundLocal)Visit(node.ResumeTargetLocal);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(activeHandlerLocal, resumeTargetLocal, type);
		}

		public override BoundNode VisitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node)
		{
			BoundExpression value = (BoundExpression)Visit(node.Value);
			ImmutableArray<BoundGotoStatement> jumps = VisitList(node.Jumps);
			return node.Update(value, jumps);
		}

		public override BoundNode VisitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node)
		{
			BoundLocal resumeTargetTemporary = (BoundLocal)Visit(node.ResumeTargetTemporary);
			BoundLabelStatement resumeLabel = (BoundLabelStatement)Visit(node.ResumeLabel);
			BoundLabelStatement resumeNextLabel = (BoundLabelStatement)Visit(node.ResumeNextLabel);
			ImmutableArray<BoundGotoStatement> jumps = VisitList(node.Jumps);
			return node.Update(resumeTargetTemporary, resumeLabel, resumeNextLabel, jumps);
		}

		public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			BoundExpression operand = (BoundExpression)Visit(node.Operand);
			BoundRValuePlaceholder awaitableInstancePlaceholder = (BoundRValuePlaceholder)Visit(node.AwaitableInstancePlaceholder);
			BoundExpression getAwaiter = (BoundExpression)Visit(node.GetAwaiter);
			BoundLValuePlaceholder awaiterInstancePlaceholder = (BoundLValuePlaceholder)Visit(node.AwaiterInstancePlaceholder);
			BoundExpression isCompleted = (BoundExpression)Visit(node.IsCompleted);
			BoundExpression getResult = (BoundExpression)Visit(node.GetResult);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(operand, awaitableInstancePlaceholder, getAwaiter, awaiterInstancePlaceholder, isCompleted, getResult, type);
		}

		public override BoundNode VisitSpillSequence(BoundSpillSequence node)
		{
			ImmutableArray<BoundStatement> statements = VisitList(node.Statements);
			BoundExpression valueOpt = (BoundExpression)Visit(node.ValueOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.Locals, node.SpillFields, statements, valueOpt, type);
		}

		public override BoundNode VisitStopStatement(BoundStopStatement node)
		{
			return node;
		}

		public override BoundNode VisitEndStatement(BoundEndStatement node)
		{
			return node;
		}

		public override BoundNode VisitMidResult(BoundMidResult node)
		{
			BoundExpression original = (BoundExpression)Visit(node.Original);
			BoundExpression start = (BoundExpression)Visit(node.Start);
			BoundExpression lengthOpt = (BoundExpression)Visit(node.LengthOpt);
			BoundExpression source = (BoundExpression)Visit(node.Source);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(original, start, lengthOpt, source, type);
		}

		public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
		{
			BoundExpression receiver = (BoundExpression)Visit(node.Receiver);
			BoundRValuePlaceholder placeholder = (BoundRValuePlaceholder)Visit(node.Placeholder);
			BoundExpression accessExpression = (BoundExpression)Visit(node.AccessExpression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(receiver, placeholder, accessExpression, type);
		}

		public override BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
		{
			TypeSymbol type = VisitType(node.Type);
			return node.Update(node.PlaceholderId, type);
		}

		public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
		{
			BoundExpression receiverOrCondition = (BoundExpression)Visit(node.ReceiverOrCondition);
			BoundExpression whenNotNull = (BoundExpression)Visit(node.WhenNotNull);
			BoundExpression whenNullOpt = (BoundExpression)Visit(node.WhenNullOpt);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(receiverOrCondition, node.CaptureReceiver, node.PlaceholderId, whenNotNull, whenNullOpt, type);
		}

		public override BoundNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node)
		{
			BoundExpression valueTypeReceiver = (BoundExpression)Visit(node.ValueTypeReceiver);
			BoundExpression referenceTypeReceiver = (BoundExpression)Visit(node.ReferenceTypeReceiver);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(valueTypeReceiver, referenceTypeReceiver, type);
		}

		public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
		{
			BoundExpression argument = (BoundExpression)Visit(node.Argument);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(argument, node.ConstantValueOpt, type);
		}

		public override BoundNode VisitTypeAsValueExpression(BoundTypeAsValueExpression node)
		{
			BoundTypeExpression expression = (BoundTypeExpression)Visit(node.Expression);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(expression, type);
		}

		public override BoundNode VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node)
		{
			ImmutableArray<BoundNode> contents = VisitList(node.Contents);
			TypeSymbol type = VisitType(node.Type);
			return node.Update(contents, node.Binder, type);
		}

		public override BoundNode VisitInterpolation(BoundInterpolation node)
		{
			BoundExpression expression = (BoundExpression)Visit(node.Expression);
			BoundExpression alignmentOpt = (BoundExpression)Visit(node.AlignmentOpt);
			BoundLiteral formatStringOpt = (BoundLiteral)Visit(node.FormatStringOpt);
			return node.Update(expression, alignmentOpt, formatStringOpt);
		}
	}
}
