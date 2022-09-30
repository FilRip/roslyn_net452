using System;
using System.Linq;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundTreeDumperNodeProducer : BoundTreeVisitor<object, TreeDumperNode>
	{
		private BoundTreeDumperNodeProducer()
		{
		}

		public static TreeDumperNode MakeTree(BoundNode node)
		{
			return new BoundTreeDumperNodeProducer().Visit(node, null);
		}

		public override TreeDumperNode VisitTypeArguments(BoundTypeArguments node, object arg)
		{
			return new TreeDumperNode("typeArguments", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("arguments", node.Arguments, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitOmittedArgument(BoundOmittedArgument node, object arg)
		{
			return new TreeDumperNode("omittedArgument", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node, object arg)
		{
			return new TreeDumperNode("lValueToRValueWrapper", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("underlyingLValue", null, new TreeDumperNode[1] { Visit(node.UnderlyingLValue, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node, object arg)
		{
			return new TreeDumperNode("withLValueExpressionPlaceholder", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node, object arg)
		{
			return new TreeDumperNode("withRValueExpressionPlaceholder", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitRValuePlaceholder(BoundRValuePlaceholder node, object arg)
		{
			return new TreeDumperNode("rValuePlaceholder", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLValuePlaceholder(BoundLValuePlaceholder node, object arg)
		{
			return new TreeDumperNode("lValuePlaceholder", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitDup(BoundDup node, object arg)
		{
			return new TreeDumperNode("dup", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("isReference", node.IsReference, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitBadExpression(BoundBadExpression node, object arg)
		{
			return new TreeDumperNode("badExpression", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("resultKind", node.ResultKind, null),
				new TreeDumperNode("symbols", node.Symbols, null),
				new TreeDumperNode("childBoundNodes", null, node.ChildBoundNodes.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitBadStatement(BoundBadStatement node, object arg)
		{
			return new TreeDumperNode("badStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("childBoundNodes", null, node.ChildBoundNodes.Select((BoundNode x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitParenthesized(BoundParenthesized node, object arg)
		{
			return new TreeDumperNode("parenthesized", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitBadVariable(BoundBadVariable node, object arg)
		{
			return new TreeDumperNode("badVariable", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitArrayAccess(BoundArrayAccess node, object arg)
		{
			return new TreeDumperNode("arrayAccess", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("indices", null, node.Indices.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitArrayLength(BoundArrayLength node, object arg)
		{
			return new TreeDumperNode("arrayLength", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitGetType(BoundGetType node, object arg)
		{
			return new TreeDumperNode("[getType]", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("sourceType", null, new TreeDumperNode[1] { Visit(node.SourceType, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitFieldInfo(BoundFieldInfo node, object arg)
		{
			return new TreeDumperNode("fieldInfo", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("field", node.Field, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitMethodInfo(BoundMethodInfo node, object arg)
		{
			return new TreeDumperNode("methodInfo", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("method", node.Method, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTypeExpression(BoundTypeExpression node, object arg)
		{
			return new TreeDumperNode("typeExpression", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("unevaluatedReceiverOpt", null, new TreeDumperNode[1] { Visit(node.UnevaluatedReceiverOpt, null) }),
				new TreeDumperNode("aliasOpt", node.AliasOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node, object arg)
		{
			return new TreeDumperNode("typeOrValueExpression", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("data", node.Data, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitNamespaceExpression(BoundNamespaceExpression node, object arg)
		{
			return new TreeDumperNode("namespaceExpression", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("unevaluatedReceiverOpt", null, new TreeDumperNode[1] { Visit(node.UnevaluatedReceiverOpt, null) }),
				new TreeDumperNode("aliasOpt", node.AliasOpt, null),
				new TreeDumperNode("namespaceSymbol", node.NamespaceSymbol, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitMethodDefIndex(BoundMethodDefIndex node, object arg)
		{
			return new TreeDumperNode("methodDefIndex", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("method", node.Method, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node, object arg)
		{
			return new TreeDumperNode("maximumMethodDefIndex", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node, object arg)
		{
			return new TreeDumperNode("instrumentationPayloadRoot", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("analysisKind", node.AnalysisKind, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitModuleVersionId(BoundModuleVersionId node, object arg)
		{
			return new TreeDumperNode("moduleVersionId", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitModuleVersionIdString(BoundModuleVersionIdString node, object arg)
		{
			return new TreeDumperNode("moduleVersionIdString", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node, object arg)
		{
			return new TreeDumperNode("sourceDocumentIndex", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("document", node.Document, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitUnaryOperator(BoundUnaryOperator node, object arg)
		{
			return new TreeDumperNode("unaryOperator", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("operatorKind", node.OperatorKind, null),
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("checked", node.Checked, null),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node, object arg)
		{
			return new TreeDumperNode("userDefinedUnaryOperator", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("operatorKind", node.OperatorKind, null),
				new TreeDumperNode("underlyingExpression", null, new TreeDumperNode[1] { Visit(node.UnderlyingExpression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node, object arg)
		{
			return new TreeDumperNode("nullableIsTrueOperator", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitBinaryOperator(BoundBinaryOperator node, object arg)
		{
			return new TreeDumperNode("binaryOperator", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("operatorKind", node.OperatorKind, null),
				new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
				new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
				new TreeDumperNode("checked", node.Checked, null),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node, object arg)
		{
			return new TreeDumperNode("userDefinedBinaryOperator", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("operatorKind", node.OperatorKind, null),
				new TreeDumperNode("underlyingExpression", null, new TreeDumperNode[1] { Visit(node.UnderlyingExpression, null) }),
				new TreeDumperNode("checked", node.Checked, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node, object arg)
		{
			return new TreeDumperNode("userDefinedShortCircuitingOperator", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("leftOperand", null, new TreeDumperNode[1] { Visit(node.LeftOperand, null) }),
				new TreeDumperNode("leftOperandPlaceholder", null, new TreeDumperNode[1] { Visit(node.LeftOperandPlaceholder, null) }),
				new TreeDumperNode("leftTest", null, new TreeDumperNode[1] { Visit(node.LeftTest, null) }),
				new TreeDumperNode("bitwiseOperator", null, new TreeDumperNode[1] { Visit(node.BitwiseOperator, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node, object arg)
		{
			return new TreeDumperNode("compoundAssignmentTargetPlaceholder", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAssignmentOperator(BoundAssignmentOperator node, object arg)
		{
			return new TreeDumperNode("assignmentOperator", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("left", null, new TreeDumperNode[1] { Visit(node.Left, null) }),
				new TreeDumperNode("leftOnTheRightOpt", null, new TreeDumperNode[1] { Visit(node.LeftOnTheRightOpt, null) }),
				new TreeDumperNode("right", null, new TreeDumperNode[1] { Visit(node.Right, null) }),
				new TreeDumperNode("suppressObjectClone", node.SuppressObjectClone, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitReferenceAssignment(BoundReferenceAssignment node, object arg)
		{
			return new TreeDumperNode("referenceAssignment", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("byRefLocal", null, new TreeDumperNode[1] { Visit(node.ByRefLocal, null) }),
				new TreeDumperNode("lValue", null, new TreeDumperNode[1] { Visit(node.LValue, null) }),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAddressOfOperator(BoundAddressOfOperator node, object arg)
		{
			return new TreeDumperNode("addressOfOperator", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("withDependencies", node.WithDependencies, null),
				new TreeDumperNode("methodGroup", null, new TreeDumperNode[1] { Visit(node.MethodGroup, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node, object arg)
		{
			return new TreeDumperNode("ternaryConditionalExpression", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
				new TreeDumperNode("whenTrue", null, new TreeDumperNode[1] { Visit(node.WhenTrue, null) }),
				new TreeDumperNode("whenFalse", null, new TreeDumperNode[1] { Visit(node.WhenFalse, null) }),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node, object arg)
		{
			return new TreeDumperNode("binaryConditionalExpression", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("testExpression", null, new TreeDumperNode[1] { Visit(node.TestExpression, null) }),
				new TreeDumperNode("convertedTestExpression", null, new TreeDumperNode[1] { Visit(node.ConvertedTestExpression, null) }),
				new TreeDumperNode("testExpressionPlaceholder", null, new TreeDumperNode[1] { Visit(node.TestExpressionPlaceholder, null) }),
				new TreeDumperNode("elseExpression", null, new TreeDumperNode[1] { Visit(node.ElseExpression, null) }),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitConversion(BoundConversion node, object arg)
		{
			return new TreeDumperNode("conversion", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("conversionKind", node.ConversionKind, null),
				new TreeDumperNode("checked", node.Checked, null),
				new TreeDumperNode("explicitCastInCode", node.ExplicitCastInCode, null),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("extendedInfoOpt", null, new TreeDumperNode[1] { Visit(node.ExtendedInfoOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitRelaxationLambda(BoundRelaxationLambda node, object arg)
		{
			return new TreeDumperNode("relaxationLambda", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("lambda", null, new TreeDumperNode[1] { Visit(node.Lambda, null) }),
				new TreeDumperNode("receiverPlaceholderOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverPlaceholderOpt, null) })
			});
		}

		public override TreeDumperNode VisitConvertedTupleElements(BoundConvertedTupleElements node, object arg)
		{
			return new TreeDumperNode("convertedTupleElements", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("elementPlaceholders", null, node.ElementPlaceholders.Select((BoundRValuePlaceholder x) => Visit(x, null))),
				new TreeDumperNode("convertedElements", null, node.ConvertedElements.Select((BoundExpression x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitUserDefinedConversion(BoundUserDefinedConversion node, object arg)
		{
			return new TreeDumperNode("userDefinedConversion", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("underlyingExpression", null, new TreeDumperNode[1] { Visit(node.UnderlyingExpression, null) }),
				new TreeDumperNode("inOutConversionFlags", node.InOutConversionFlags, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitDirectCast(BoundDirectCast node, object arg)
		{
			return new TreeDumperNode("[directCast]", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("conversionKind", node.ConversionKind, null),
				new TreeDumperNode("suppressVirtualCalls", node.SuppressVirtualCalls, null),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("relaxationLambdaOpt", null, new TreeDumperNode[1] { Visit(node.RelaxationLambdaOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTryCast(BoundTryCast node, object arg)
		{
			return new TreeDumperNode("[tryCast]", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("conversionKind", node.ConversionKind, null),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("relaxationLambdaOpt", null, new TreeDumperNode[1] { Visit(node.RelaxationLambdaOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTypeOf(BoundTypeOf node, object arg)
		{
			return new TreeDumperNode("[typeOf]", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("isTypeOfIsNotExpression", node.IsTypeOfIsNotExpression, null),
				new TreeDumperNode("targetType", node.TargetType, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitSequencePoint(BoundSequencePoint node, object arg)
		{
			return new TreeDumperNode("sequencePoint", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("statementOpt", null, new TreeDumperNode[1] { Visit(node.StatementOpt, null) })
			});
		}

		public override TreeDumperNode VisitSequencePointExpression(BoundSequencePointExpression node, object arg)
		{
			return new TreeDumperNode("sequencePointExpression", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node, object arg)
		{
			return new TreeDumperNode("sequencePointWithSpan", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("statementOpt", null, new TreeDumperNode[1] { Visit(node.StatementOpt, null) }),
				new TreeDumperNode("span", node.Span, null)
			});
		}

		public override TreeDumperNode VisitNoOpStatement(BoundNoOpStatement node, object arg)
		{
			return new TreeDumperNode("noOpStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("flavor", node.Flavor, null)
			});
		}

		public override TreeDumperNode VisitMethodGroup(BoundMethodGroup node, object arg)
		{
			return new TreeDumperNode("methodGroup", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("typeArgumentsOpt", null, new TreeDumperNode[1] { Visit(node.TypeArgumentsOpt, null) }),
				new TreeDumperNode("methods", node.Methods, null),
				new TreeDumperNode("pendingExtensionMethodsOpt", node.PendingExtensionMethodsOpt, null),
				new TreeDumperNode("resultKind", node.ResultKind, null),
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("qualificationKind", node.QualificationKind, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitPropertyGroup(BoundPropertyGroup node, object arg)
		{
			return new TreeDumperNode("propertyGroup", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("properties", node.Properties, null),
				new TreeDumperNode("resultKind", node.ResultKind, null),
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("qualificationKind", node.QualificationKind, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitReturnStatement(BoundReturnStatement node, object arg)
		{
			return new TreeDumperNode("returnStatement", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("expressionOpt", null, new TreeDumperNode[1] { Visit(node.ExpressionOpt, null) }),
				new TreeDumperNode("functionLocalOpt", node.FunctionLocalOpt, null),
				new TreeDumperNode("exitLabelOpt", node.ExitLabelOpt, null)
			});
		}

		public override TreeDumperNode VisitYieldStatement(BoundYieldStatement node, object arg)
		{
			return new TreeDumperNode("yieldStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) })
			});
		}

		public override TreeDumperNode VisitThrowStatement(BoundThrowStatement node, object arg)
		{
			return new TreeDumperNode("throwStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("expressionOpt", null, new TreeDumperNode[1] { Visit(node.ExpressionOpt, null) })
			});
		}

		public override TreeDumperNode VisitRedimStatement(BoundRedimStatement node, object arg)
		{
			return new TreeDumperNode("redimStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("clauses", null, node.Clauses.Select((BoundRedimClause x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitRedimClause(BoundRedimClause node, object arg)
		{
			return new TreeDumperNode("redimClause", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("indices", null, node.Indices.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("arrayTypeOpt", node.ArrayTypeOpt, null),
				new TreeDumperNode("preserve", node.Preserve, null)
			});
		}

		public override TreeDumperNode VisitEraseStatement(BoundEraseStatement node, object arg)
		{
			return new TreeDumperNode("eraseStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("clauses", null, node.Clauses.Select((BoundAssignmentOperator x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitCall(BoundCall node, object arg)
		{
			return new TreeDumperNode("[call]", null, new TreeDumperNode[9]
			{
				new TreeDumperNode("method", node.Method, null),
				new TreeDumperNode("methodGroupOpt", null, new TreeDumperNode[1] { Visit(node.MethodGroupOpt, null) }),
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("suppressObjectClone", node.SuppressObjectClone, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAttribute(BoundAttribute node, object arg)
		{
			return new TreeDumperNode("attribute", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("constructor", node.Constructor, null),
				new TreeDumperNode("constructorArguments", null, node.ConstructorArguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("namedArguments", null, node.NamedArguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("resultKind", node.ResultKind, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLateMemberAccess(BoundLateMemberAccess node, object arg)
		{
			return new TreeDumperNode("lateMemberAccess", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("nameOpt", node.NameOpt, null),
				new TreeDumperNode("containerTypeOpt", node.ContainerTypeOpt, null),
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("typeArgumentsOpt", null, new TreeDumperNode[1] { Visit(node.TypeArgumentsOpt, null) }),
				new TreeDumperNode("accessKind", node.AccessKind, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLateInvocation(BoundLateInvocation node, object arg)
		{
			return new TreeDumperNode("lateInvocation", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("member", null, new TreeDumperNode[1] { Visit(node.Member, null) }),
				new TreeDumperNode("argumentsOpt", null, node.ArgumentsOpt.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
				new TreeDumperNode("accessKind", node.AccessKind, null),
				new TreeDumperNode("methodOrPropertyGroupOpt", null, new TreeDumperNode[1] { Visit(node.MethodOrPropertyGroupOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node, object arg)
		{
			return new TreeDumperNode("lateAddressOfOperator", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("memberAccess", null, new TreeDumperNode[1] { Visit(node.MemberAccess, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTupleLiteral(BoundTupleLiteral node, object arg)
		{
			return new TreeDumperNode("tupleLiteral", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("inferredType", node.InferredType, null),
				new TreeDumperNode("argumentNamesOpt", node.ArgumentNamesOpt, null),
				new TreeDumperNode("inferredNamesOpt", node.InferredNamesOpt, null),
				new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node, object arg)
		{
			return new TreeDumperNode("convertedTupleLiteral", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("naturalTypeOpt", node.NaturalTypeOpt, null),
				new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitObjectCreationExpression(BoundObjectCreationExpression node, object arg)
		{
			return new TreeDumperNode("objectCreationExpression", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("constructorOpt", node.ConstructorOpt, null),
				new TreeDumperNode("methodGroupOpt", null, new TreeDumperNode[1] { Visit(node.MethodGroupOpt, null) }),
				new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
				new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node, object arg)
		{
			return new TreeDumperNode("noPiaObjectCreationExpression", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("guidString", node.GuidString, null),
				new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node, object arg)
		{
			return new TreeDumperNode("anonymousTypeCreationExpression", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("binderOpt", node.BinderOpt, null),
				new TreeDumperNode("declarations", null, node.Declarations.Select((BoundAnonymousTypePropertyAccess x) => Visit(x, null))),
				new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node, object arg)
		{
			return new TreeDumperNode("anonymousTypePropertyAccess", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("propertyIndex", node.PropertyIndex, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node, object arg)
		{
			return new TreeDumperNode("anonymousTypeFieldInitializer", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node, object arg)
		{
			return new TreeDumperNode("objectInitializerExpression", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("createTemporaryLocalForInitialization", node.CreateTemporaryLocalForInitialization, null),
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("placeholderOpt", null, new TreeDumperNode[1] { Visit(node.PlaceholderOpt, null) }),
				new TreeDumperNode("initializers", null, node.Initializers.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node, object arg)
		{
			return new TreeDumperNode("collectionInitializerExpression", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("placeholderOpt", null, new TreeDumperNode[1] { Visit(node.PlaceholderOpt, null) }),
				new TreeDumperNode("initializers", null, node.Initializers.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitNewT(BoundNewT node, object arg)
		{
			return new TreeDumperNode("newT", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node, object arg)
		{
			return new TreeDumperNode("delegateCreationExpression", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("method", node.Method, null),
				new TreeDumperNode("relaxationLambdaOpt", null, new TreeDumperNode[1] { Visit(node.RelaxationLambdaOpt, null) }),
				new TreeDumperNode("relaxationReceiverPlaceholderOpt", null, new TreeDumperNode[1] { Visit(node.RelaxationReceiverPlaceholderOpt, null) }),
				new TreeDumperNode("methodGroupOpt", null, new TreeDumperNode[1] { Visit(node.MethodGroupOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitArrayCreation(BoundArrayCreation node, object arg)
		{
			return new TreeDumperNode("arrayCreation", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("isParamArrayArgument", node.IsParamArrayArgument, null),
				new TreeDumperNode("bounds", null, node.Bounds.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) }),
				new TreeDumperNode("arrayLiteralOpt", null, new TreeDumperNode[1] { Visit(node.ArrayLiteralOpt, null) }),
				new TreeDumperNode("arrayLiteralConversion", node.ArrayLiteralConversion, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitArrayLiteral(BoundArrayLiteral node, object arg)
		{
			return new TreeDumperNode("arrayLiteral", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("hasDominantType", node.HasDominantType, null),
				new TreeDumperNode("numberOfCandidates", node.NumberOfCandidates, null),
				new TreeDumperNode("inferredType", node.InferredType, null),
				new TreeDumperNode("bounds", null, node.Bounds.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("initializer", null, new TreeDumperNode[1] { Visit(node.Initializer, null) }),
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitArrayInitialization(BoundArrayInitialization node, object arg)
		{
			return new TreeDumperNode("arrayInitialization", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("initializers", null, node.Initializers.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitFieldAccess(BoundFieldAccess node, object arg)
		{
			return new TreeDumperNode("fieldAccess", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("fieldSymbol", node.FieldSymbol, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("suppressVirtualCalls", node.SuppressVirtualCalls, null),
				new TreeDumperNode("constantsInProgressOpt", node.ConstantsInProgressOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitPropertyAccess(BoundPropertyAccess node, object arg)
		{
			return new TreeDumperNode("propertyAccess", null, new TreeDumperNode[9]
			{
				new TreeDumperNode("propertySymbol", node.PropertySymbol, null),
				new TreeDumperNode("propertyGroupOpt", null, new TreeDumperNode[1] { Visit(node.PropertyGroupOpt, null) }),
				new TreeDumperNode("accessKind", node.AccessKind, null),
				new TreeDumperNode("isWriteable", node.IsWriteable, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("arguments", null, node.Arguments.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("defaultArguments", node.DefaultArguments, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitEventAccess(BoundEventAccess node, object arg)
		{
			return new TreeDumperNode("eventAccess", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("receiverOpt", null, new TreeDumperNode[1] { Visit(node.ReceiverOpt, null) }),
				new TreeDumperNode("eventSymbol", node.EventSymbol, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitBlock(BoundBlock node, object arg)
		{
			return new TreeDumperNode("block", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("statementListSyntax", node.StatementListSyntax, null),
				new TreeDumperNode("locals", node.Locals, null),
				new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitStateMachineScope(BoundStateMachineScope node, object arg)
		{
			return new TreeDumperNode("stateMachineScope", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("fields", node.Fields, null),
				new TreeDumperNode("statement", null, new TreeDumperNode[1] { Visit(node.Statement, null) })
			});
		}

		public override TreeDumperNode VisitLocalDeclaration(BoundLocalDeclaration node, object arg)
		{
			return new TreeDumperNode("localDeclaration", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("localSymbol", node.LocalSymbol, null),
				new TreeDumperNode("declarationInitializerOpt", null, new TreeDumperNode[1] { Visit(node.DeclarationInitializerOpt, null) }),
				new TreeDumperNode("identifierInitializerOpt", null, new TreeDumperNode[1] { Visit(node.IdentifierInitializerOpt, null) }),
				new TreeDumperNode("initializedByAsNew", node.InitializedByAsNew, null)
			});
		}

		public override TreeDumperNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node, object arg)
		{
			return new TreeDumperNode("asNewLocalDeclarations", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("localDeclarations", null, node.LocalDeclarations.Select((BoundLocalDeclaration x) => Visit(x, null))),
				new TreeDumperNode("initializer", null, new TreeDumperNode[1] { Visit(node.Initializer, null) })
			});
		}

		public override TreeDumperNode VisitDimStatement(BoundDimStatement node, object arg)
		{
			return new TreeDumperNode("dimStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("localDeclarations", null, node.LocalDeclarations.Select((BoundLocalDeclarationBase x) => Visit(x, null))),
				new TreeDumperNode("initializerOpt", null, new TreeDumperNode[1] { Visit(node.InitializerOpt, null) })
			});
		}

		public override TreeDumperNode VisitInitializer(BoundInitializer node, object arg)
		{
			return new TreeDumperNode("initializer", null, Array.Empty<TreeDumperNode>());
		}

		public override TreeDumperNode VisitFieldInitializer(BoundFieldInitializer node, object arg)
		{
			return new TreeDumperNode("fieldInitializer", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("initializedFields", node.InitializedFields, null),
				new TreeDumperNode("memberAccessExpressionOpt", null, new TreeDumperNode[1] { Visit(node.MemberAccessExpressionOpt, null) }),
				new TreeDumperNode("initialValue", null, new TreeDumperNode[1] { Visit(node.InitialValue, null) })
			});
		}

		public override TreeDumperNode VisitPropertyInitializer(BoundPropertyInitializer node, object arg)
		{
			return new TreeDumperNode("propertyInitializer", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("initializedProperties", node.InitializedProperties, null),
				new TreeDumperNode("memberAccessExpressionOpt", null, new TreeDumperNode[1] { Visit(node.MemberAccessExpressionOpt, null) }),
				new TreeDumperNode("initialValue", null, new TreeDumperNode[1] { Visit(node.InitialValue, null) })
			});
		}

		public override TreeDumperNode VisitParameterEqualsValue(BoundParameterEqualsValue node, object arg)
		{
			return new TreeDumperNode("parameterEqualsValue", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("parameter", node.Parameter, null),
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) })
			});
		}

		public override TreeDumperNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node, object arg)
		{
			return new TreeDumperNode("globalStatementInitializer", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("statement", null, new TreeDumperNode[1] { Visit(node.Statement, null) })
			});
		}

		public override TreeDumperNode VisitSequence(BoundSequence node, object arg)
		{
			return new TreeDumperNode("sequence", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("locals", node.Locals, null),
				new TreeDumperNode("sideEffects", null, node.SideEffects.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("valueOpt", null, new TreeDumperNode[1] { Visit(node.ValueOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitExpressionStatement(BoundExpressionStatement node, object arg)
		{
			return new TreeDumperNode("expressionStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) })
			});
		}

		public override TreeDumperNode VisitIfStatement(BoundIfStatement node, object arg)
		{
			return new TreeDumperNode("ifStatement", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
				new TreeDumperNode("consequence", null, new TreeDumperNode[1] { Visit(node.Consequence, null) }),
				new TreeDumperNode("alternativeOpt", null, new TreeDumperNode[1] { Visit(node.AlternativeOpt, null) })
			});
		}

		public override TreeDumperNode VisitSelectStatement(BoundSelectStatement node, object arg)
		{
			return new TreeDumperNode("selectStatement", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("expressionStatement", null, new TreeDumperNode[1] { Visit(node.ExpressionStatement, null) }),
				new TreeDumperNode("exprPlaceholderOpt", null, new TreeDumperNode[1] { Visit(node.ExprPlaceholderOpt, null) }),
				new TreeDumperNode("caseBlocks", null, node.CaseBlocks.Select((BoundCaseBlock x) => Visit(x, null))),
				new TreeDumperNode("recommendSwitchTable", node.RecommendSwitchTable, null),
				new TreeDumperNode("exitLabel", node.ExitLabel, null)
			});
		}

		public override TreeDumperNode VisitCaseBlock(BoundCaseBlock node, object arg)
		{
			return new TreeDumperNode("caseBlock", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("caseStatement", null, new TreeDumperNode[1] { Visit(node.CaseStatement, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) })
			});
		}

		public override TreeDumperNode VisitCaseStatement(BoundCaseStatement node, object arg)
		{
			return new TreeDumperNode("caseStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("caseClauses", null, node.CaseClauses.Select((BoundCaseClause x) => Visit(x, null))),
				new TreeDumperNode("conditionOpt", null, new TreeDumperNode[1] { Visit(node.ConditionOpt, null) })
			});
		}

		public override TreeDumperNode VisitSimpleCaseClause(BoundSimpleCaseClause node, object arg)
		{
			return new TreeDumperNode("simpleCaseClause", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("valueOpt", null, new TreeDumperNode[1] { Visit(node.ValueOpt, null) }),
				new TreeDumperNode("conditionOpt", null, new TreeDumperNode[1] { Visit(node.ConditionOpt, null) })
			});
		}

		public override TreeDumperNode VisitRangeCaseClause(BoundRangeCaseClause node, object arg)
		{
			return new TreeDumperNode("rangeCaseClause", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("lowerBoundOpt", null, new TreeDumperNode[1] { Visit(node.LowerBoundOpt, null) }),
				new TreeDumperNode("upperBoundOpt", null, new TreeDumperNode[1] { Visit(node.UpperBoundOpt, null) }),
				new TreeDumperNode("lowerBoundConditionOpt", null, new TreeDumperNode[1] { Visit(node.LowerBoundConditionOpt, null) }),
				new TreeDumperNode("upperBoundConditionOpt", null, new TreeDumperNode[1] { Visit(node.UpperBoundConditionOpt, null) })
			});
		}

		public override TreeDumperNode VisitRelationalCaseClause(BoundRelationalCaseClause node, object arg)
		{
			return new TreeDumperNode("relationalCaseClause", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("operatorKind", node.OperatorKind, null),
				new TreeDumperNode("valueOpt", null, new TreeDumperNode[1] { Visit(node.ValueOpt, null) }),
				new TreeDumperNode("conditionOpt", null, new TreeDumperNode[1] { Visit(node.ConditionOpt, null) })
			});
		}

		public override TreeDumperNode VisitDoLoopStatement(BoundDoLoopStatement node, object arg)
		{
			return new TreeDumperNode("doLoopStatement", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("topConditionOpt", null, new TreeDumperNode[1] { Visit(node.TopConditionOpt, null) }),
				new TreeDumperNode("bottomConditionOpt", null, new TreeDumperNode[1] { Visit(node.BottomConditionOpt, null) }),
				new TreeDumperNode("topConditionIsUntil", node.TopConditionIsUntil, null),
				new TreeDumperNode("bottomConditionIsUntil", node.BottomConditionIsUntil, null),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("continueLabel", node.ContinueLabel, null),
				new TreeDumperNode("exitLabel", node.ExitLabel, null)
			});
		}

		public override TreeDumperNode VisitWhileStatement(BoundWhileStatement node, object arg)
		{
			return new TreeDumperNode("whileStatement", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("continueLabel", node.ContinueLabel, null),
				new TreeDumperNode("exitLabel", node.ExitLabel, null)
			});
		}

		public override TreeDumperNode VisitForToUserDefinedOperators(BoundForToUserDefinedOperators node, object arg)
		{
			return new TreeDumperNode("forToUserDefinedOperators", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("leftOperandPlaceholder", null, new TreeDumperNode[1] { Visit(node.LeftOperandPlaceholder, null) }),
				new TreeDumperNode("rightOperandPlaceholder", null, new TreeDumperNode[1] { Visit(node.RightOperandPlaceholder, null) }),
				new TreeDumperNode("addition", null, new TreeDumperNode[1] { Visit(node.Addition, null) }),
				new TreeDumperNode("subtraction", null, new TreeDumperNode[1] { Visit(node.Subtraction, null) }),
				new TreeDumperNode("lessThanOrEqual", null, new TreeDumperNode[1] { Visit(node.LessThanOrEqual, null) }),
				new TreeDumperNode("greaterThanOrEqual", null, new TreeDumperNode[1] { Visit(node.GreaterThanOrEqual, null) })
			});
		}

		public override TreeDumperNode VisitForToStatement(BoundForToStatement node, object arg)
		{
			return new TreeDumperNode("forToStatement", null, new TreeDumperNode[11]
			{
				new TreeDumperNode("initialValue", null, new TreeDumperNode[1] { Visit(node.InitialValue, null) }),
				new TreeDumperNode("limitValue", null, new TreeDumperNode[1] { Visit(node.LimitValue, null) }),
				new TreeDumperNode("stepValue", null, new TreeDumperNode[1] { Visit(node.StepValue, null) }),
				new TreeDumperNode("checked", node.Checked, null),
				new TreeDumperNode("operatorsOpt", null, new TreeDumperNode[1] { Visit(node.OperatorsOpt, null) }),
				new TreeDumperNode("declaredOrInferredLocalOpt", node.DeclaredOrInferredLocalOpt, null),
				new TreeDumperNode("controlVariable", null, new TreeDumperNode[1] { Visit(node.ControlVariable, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("nextVariablesOpt", null, node.NextVariablesOpt.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("continueLabel", node.ContinueLabel, null),
				new TreeDumperNode("exitLabel", node.ExitLabel, null)
			});
		}

		public override TreeDumperNode VisitForEachStatement(BoundForEachStatement node, object arg)
		{
			return new TreeDumperNode("forEachStatement", null, new TreeDumperNode[8]
			{
				new TreeDumperNode("collection", null, new TreeDumperNode[1] { Visit(node.Collection, null) }),
				new TreeDumperNode("enumeratorInfo", node.EnumeratorInfo, null),
				new TreeDumperNode("declaredOrInferredLocalOpt", node.DeclaredOrInferredLocalOpt, null),
				new TreeDumperNode("controlVariable", null, new TreeDumperNode[1] { Visit(node.ControlVariable, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("nextVariablesOpt", null, node.NextVariablesOpt.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("continueLabel", node.ContinueLabel, null),
				new TreeDumperNode("exitLabel", node.ExitLabel, null)
			});
		}

		public override TreeDumperNode VisitExitStatement(BoundExitStatement node, object arg)
		{
			return new TreeDumperNode("exitStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("label", node.Label, null)
			});
		}

		public override TreeDumperNode VisitContinueStatement(BoundContinueStatement node, object arg)
		{
			return new TreeDumperNode("continueStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("label", node.Label, null)
			});
		}

		public override TreeDumperNode VisitTryStatement(BoundTryStatement node, object arg)
		{
			return new TreeDumperNode("tryStatement", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("tryBlock", null, new TreeDumperNode[1] { Visit(node.TryBlock, null) }),
				new TreeDumperNode("catchBlocks", null, node.CatchBlocks.Select((BoundCatchBlock x) => Visit(x, null))),
				new TreeDumperNode("finallyBlockOpt", null, new TreeDumperNode[1] { Visit(node.FinallyBlockOpt, null) }),
				new TreeDumperNode("exitLabelOpt", node.ExitLabelOpt, null)
			});
		}

		public override TreeDumperNode VisitCatchBlock(BoundCatchBlock node, object arg)
		{
			return new TreeDumperNode("catchBlock", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("localOpt", node.LocalOpt, null),
				new TreeDumperNode("exceptionSourceOpt", null, new TreeDumperNode[1] { Visit(node.ExceptionSourceOpt, null) }),
				new TreeDumperNode("errorLineNumberOpt", null, new TreeDumperNode[1] { Visit(node.ErrorLineNumberOpt, null) }),
				new TreeDumperNode("exceptionFilterOpt", null, new TreeDumperNode[1] { Visit(node.ExceptionFilterOpt, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("isSynthesizedAsyncCatchAll", node.IsSynthesizedAsyncCatchAll, null)
			});
		}

		public override TreeDumperNode VisitLiteral(BoundLiteral node, object arg)
		{
			return new TreeDumperNode("literal", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("value", node.Value, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitMeReference(BoundMeReference node, object arg)
		{
			return new TreeDumperNode("meReference", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitValueTypeMeReference(BoundValueTypeMeReference node, object arg)
		{
			return new TreeDumperNode("valueTypeMeReference", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitMyBaseReference(BoundMyBaseReference node, object arg)
		{
			return new TreeDumperNode("myBaseReference", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitMyClassReference(BoundMyClassReference node, object arg)
		{
			return new TreeDumperNode("myClassReference", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node, object arg)
		{
			return new TreeDumperNode("previousSubmissionReference", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("sourceType", node.SourceType, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node, object arg)
		{
			return new TreeDumperNode("hostObjectMemberReference", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLocal(BoundLocal node, object arg)
		{
			return new TreeDumperNode("local", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("localSymbol", node.LocalSymbol, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitPseudoVariable(BoundPseudoVariable node, object arg)
		{
			return new TreeDumperNode("pseudoVariable", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("localSymbol", node.LocalSymbol, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("emitExpressions", node.EmitExpressions, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitParameter(BoundParameter node, object arg)
		{
			return new TreeDumperNode("parameter", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("parameterSymbol", node.ParameterSymbol, null),
				new TreeDumperNode("isLValue", node.IsLValue, null),
				new TreeDumperNode("suppressVirtualCalls", node.SuppressVirtualCalls, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node, object arg)
		{
			return new TreeDumperNode("byRefArgumentPlaceholder", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("isOut", node.IsOut, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node, object arg)
		{
			return new TreeDumperNode("byRefArgumentWithCopyBack", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("originalArgument", null, new TreeDumperNode[1] { Visit(node.OriginalArgument, null) }),
				new TreeDumperNode("inConversion", null, new TreeDumperNode[1] { Visit(node.InConversion, null) }),
				new TreeDumperNode("inPlaceholder", null, new TreeDumperNode[1] { Visit(node.InPlaceholder, null) }),
				new TreeDumperNode("outConversion", null, new TreeDumperNode[1] { Visit(node.OutConversion, null) }),
				new TreeDumperNode("outPlaceholder", null, new TreeDumperNode[1] { Visit(node.OutPlaceholder, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLateBoundArgumentSupportingAssignmentWithCapture(BoundLateBoundArgumentSupportingAssignmentWithCapture node, object arg)
		{
			return new TreeDumperNode("lateBoundArgumentSupportingAssignmentWithCapture", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("originalArgument", null, new TreeDumperNode[1] { Visit(node.OriginalArgument, null) }),
				new TreeDumperNode("localSymbol", node.LocalSymbol, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLabelStatement(BoundLabelStatement node, object arg)
		{
			return new TreeDumperNode("labelStatement", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("label", node.Label, null)
			});
		}

		public override TreeDumperNode VisitLabel(BoundLabel node, object arg)
		{
			return new TreeDumperNode("label", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("label", node.Label, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitGotoStatement(BoundGotoStatement node, object arg)
		{
			return new TreeDumperNode("gotoStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("label", node.Label, null),
				new TreeDumperNode("labelExpressionOpt", null, new TreeDumperNode[1] { Visit(node.LabelExpressionOpt, null) })
			});
		}

		public override TreeDumperNode VisitStatementList(BoundStatementList node, object arg)
		{
			return new TreeDumperNode("statementList", null, new TreeDumperNode[1]
			{
				new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitConditionalGoto(BoundConditionalGoto node, object arg)
		{
			return new TreeDumperNode("conditionalGoto", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("condition", null, new TreeDumperNode[1] { Visit(node.Condition, null) }),
				new TreeDumperNode("jumpIfTrue", node.JumpIfTrue, null),
				new TreeDumperNode("label", node.Label, null)
			});
		}

		public override TreeDumperNode VisitWithStatement(BoundWithStatement node, object arg)
		{
			return new TreeDumperNode("withStatement", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("originalExpression", null, new TreeDumperNode[1] { Visit(node.OriginalExpression, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("binder", node.Binder, null)
			});
		}

		public override TreeDumperNode VisitUnboundLambda(UnboundLambda node, object arg)
		{
			return new TreeDumperNode("unboundLambda", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("flags", node.Flags, null),
				new TreeDumperNode("parameters", node.Parameters, null),
				new TreeDumperNode("returnType", node.ReturnType, null),
				new TreeDumperNode("bindingCache", node.BindingCache, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLambda(BoundLambda node, object arg)
		{
			return new TreeDumperNode("lambda", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("lambdaSymbol", node.LambdaSymbol, null),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("diagnostics", node.Diagnostics, null),
				new TreeDumperNode("lambdaBinderOpt", node.LambdaBinderOpt, null),
				new TreeDumperNode("delegateRelaxation", node.DelegateRelaxation, null),
				new TreeDumperNode("methodConversionKind", node.MethodConversionKind, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitQueryExpression(BoundQueryExpression node, object arg)
		{
			return new TreeDumperNode("queryExpression", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("lastOperator", null, new TreeDumperNode[1] { Visit(node.LastOperator, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitQuerySource(BoundQuerySource node, object arg)
		{
			return new TreeDumperNode("querySource", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node, object arg)
		{
			return new TreeDumperNode("toQueryableCollectionConversion", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("conversionCall", null, new TreeDumperNode[1] { Visit(node.ConversionCall, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitQueryableSource(BoundQueryableSource node, object arg)
		{
			return new TreeDumperNode("queryableSource", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("source", null, new TreeDumperNode[1] { Visit(node.Source, null) }),
				new TreeDumperNode("rangeVariableOpt", node.RangeVariableOpt, null),
				new TreeDumperNode("rangeVariables", node.RangeVariables, null),
				new TreeDumperNode("compoundVariableType", node.CompoundVariableType, null),
				new TreeDumperNode("binders", node.Binders, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitQueryClause(BoundQueryClause node, object arg)
		{
			return new TreeDumperNode("queryClause", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("underlyingExpression", null, new TreeDumperNode[1] { Visit(node.UnderlyingExpression, null) }),
				new TreeDumperNode("rangeVariables", node.RangeVariables, null),
				new TreeDumperNode("compoundVariableType", node.CompoundVariableType, null),
				new TreeDumperNode("binders", node.Binders, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitOrdering(BoundOrdering node, object arg)
		{
			return new TreeDumperNode("ordering", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("underlyingExpression", null, new TreeDumperNode[1] { Visit(node.UnderlyingExpression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitQueryLambda(BoundQueryLambda node, object arg)
		{
			return new TreeDumperNode("queryLambda", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("lambdaSymbol", node.LambdaSymbol, null),
				new TreeDumperNode("rangeVariables", node.RangeVariables, null),
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("exprIsOperandOfConditionalBranch", node.ExprIsOperandOfConditionalBranch, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node, object arg)
		{
			return new TreeDumperNode("rangeVariableAssignment", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("rangeVariable", node.RangeVariable, null),
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitGroupTypeInferenceLambda(GroupTypeInferenceLambda node, object arg)
		{
			return new TreeDumperNode("groupTypeInferenceLambda", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("parameters", node.Parameters, null),
				new TreeDumperNode("compilation", node.Compilation, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAggregateClause(BoundAggregateClause node, object arg)
		{
			return new TreeDumperNode("aggregateClause", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("capturedGroupOpt", null, new TreeDumperNode[1] { Visit(node.CapturedGroupOpt, null) }),
				new TreeDumperNode("groupPlaceholderOpt", null, new TreeDumperNode[1] { Visit(node.GroupPlaceholderOpt, null) }),
				new TreeDumperNode("underlyingExpression", null, new TreeDumperNode[1] { Visit(node.UnderlyingExpression, null) }),
				new TreeDumperNode("rangeVariables", node.RangeVariables, null),
				new TreeDumperNode("compoundVariableType", node.CompoundVariableType, null),
				new TreeDumperNode("binders", node.Binders, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitGroupAggregation(BoundGroupAggregation node, object arg)
		{
			return new TreeDumperNode("groupAggregation", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("group", null, new TreeDumperNode[1] { Visit(node.Group, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitRangeVariable(BoundRangeVariable node, object arg)
		{
			return new TreeDumperNode("rangeVariable", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("rangeVariable", node.RangeVariable, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitAddHandlerStatement(BoundAddHandlerStatement node, object arg)
		{
			return new TreeDumperNode("addHandlerStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("eventAccess", null, new TreeDumperNode[1] { Visit(node.EventAccess, null) }),
				new TreeDumperNode("handler", null, new TreeDumperNode[1] { Visit(node.Handler, null) })
			});
		}

		public override TreeDumperNode VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node, object arg)
		{
			return new TreeDumperNode("removeHandlerStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("eventAccess", null, new TreeDumperNode[1] { Visit(node.EventAccess, null) }),
				new TreeDumperNode("handler", null, new TreeDumperNode[1] { Visit(node.Handler, null) })
			});
		}

		public override TreeDumperNode VisitRaiseEventStatement(BoundRaiseEventStatement node, object arg)
		{
			return new TreeDumperNode("raiseEventStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("eventSymbol", node.EventSymbol, null),
				new TreeDumperNode("eventInvocation", null, new TreeDumperNode[1] { Visit(node.EventInvocation, null) })
			});
		}

		public override TreeDumperNode VisitUsingStatement(BoundUsingStatement node, object arg)
		{
			return new TreeDumperNode("usingStatement", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("resourceList", null, node.ResourceList.Select((BoundLocalDeclarationBase x) => Visit(x, null))),
				new TreeDumperNode("resourceExpressionOpt", null, new TreeDumperNode[1] { Visit(node.ResourceExpressionOpt, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) }),
				new TreeDumperNode("usingInfo", node.UsingInfo, null),
				new TreeDumperNode("locals", node.Locals, null)
			});
		}

		public override TreeDumperNode VisitSyncLockStatement(BoundSyncLockStatement node, object arg)
		{
			return new TreeDumperNode("syncLockStatement", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("lockExpression", null, new TreeDumperNode[1] { Visit(node.LockExpression, null) }),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) })
			});
		}

		public override TreeDumperNode VisitXmlName(BoundXmlName node, object arg)
		{
			return new TreeDumperNode("xmlName", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("xmlNamespace", null, new TreeDumperNode[1] { Visit(node.XmlNamespace, null) }),
				new TreeDumperNode("localName", null, new TreeDumperNode[1] { Visit(node.LocalName, null) }),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlNamespace(BoundXmlNamespace node, object arg)
		{
			return new TreeDumperNode("xmlNamespace", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("xmlNamespace", null, new TreeDumperNode[1] { Visit(node.XmlNamespace, null) }),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlDocument(BoundXmlDocument node, object arg)
		{
			return new TreeDumperNode("xmlDocument", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("declaration", null, new TreeDumperNode[1] { Visit(node.Declaration, null) }),
				new TreeDumperNode("childNodes", null, node.ChildNodes.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("rewriterInfo", node.RewriterInfo, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlDeclaration(BoundXmlDeclaration node, object arg)
		{
			return new TreeDumperNode("xmlDeclaration", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("version", null, new TreeDumperNode[1] { Visit(node.Version, null) }),
				new TreeDumperNode("encoding", null, new TreeDumperNode[1] { Visit(node.Encoding, null) }),
				new TreeDumperNode("standalone", null, new TreeDumperNode[1] { Visit(node.Standalone, null) }),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node, object arg)
		{
			return new TreeDumperNode("xmlProcessingInstruction", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("target", null, new TreeDumperNode[1] { Visit(node.Target, null) }),
				new TreeDumperNode("data", null, new TreeDumperNode[1] { Visit(node.Data, null) }),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlComment(BoundXmlComment node, object arg)
		{
			return new TreeDumperNode("xmlComment", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlAttribute(BoundXmlAttribute node, object arg)
		{
			return new TreeDumperNode("xmlAttribute", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("name", null, new TreeDumperNode[1] { Visit(node.Name, null) }),
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
				new TreeDumperNode("matchesImport", node.MatchesImport, null),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlElement(BoundXmlElement node, object arg)
		{
			return new TreeDumperNode("xmlElement", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
				new TreeDumperNode("childNodes", null, node.ChildNodes.Select((BoundExpression x) => Visit(x, null))),
				new TreeDumperNode("rewriterInfo", node.RewriterInfo, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlMemberAccess(BoundXmlMemberAccess node, object arg)
		{
			return new TreeDumperNode("xmlMemberAccess", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("memberAccess", null, new TreeDumperNode[1] { Visit(node.MemberAccess, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node, object arg)
		{
			return new TreeDumperNode("xmlEmbeddedExpression", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitXmlCData(BoundXmlCData node, object arg)
		{
			return new TreeDumperNode("xmlCData", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
				new TreeDumperNode("objectCreation", null, new TreeDumperNode[1] { Visit(node.ObjectCreation, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitResumeStatement(BoundResumeStatement node, object arg)
		{
			return new TreeDumperNode("resumeStatement", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("resumeKind", node.ResumeKind, null),
				new TreeDumperNode("labelOpt", node.LabelOpt, null),
				new TreeDumperNode("labelExpressionOpt", null, new TreeDumperNode[1] { Visit(node.LabelExpressionOpt, null) })
			});
		}

		public override TreeDumperNode VisitOnErrorStatement(BoundOnErrorStatement node, object arg)
		{
			return new TreeDumperNode("onErrorStatement", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("onErrorKind", node.OnErrorKind, null),
				new TreeDumperNode("labelOpt", node.LabelOpt, null),
				new TreeDumperNode("labelExpressionOpt", null, new TreeDumperNode[1] { Visit(node.LabelExpressionOpt, null) })
			});
		}

		public override TreeDumperNode VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node, object arg)
		{
			return new TreeDumperNode("unstructuredExceptionHandlingStatement", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("containsOnError", node.ContainsOnError, null),
				new TreeDumperNode("containsResume", node.ContainsResume, null),
				new TreeDumperNode("resumeWithoutLabelOpt", node.ResumeWithoutLabelOpt, null),
				new TreeDumperNode("trackLineNumber", node.TrackLineNumber, null),
				new TreeDumperNode("body", null, new TreeDumperNode[1] { Visit(node.Body, null) })
			});
		}

		public override TreeDumperNode VisitUnstructuredExceptionHandlingCatchFilter(BoundUnstructuredExceptionHandlingCatchFilter node, object arg)
		{
			return new TreeDumperNode("unstructuredExceptionHandlingCatchFilter", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("activeHandlerLocal", null, new TreeDumperNode[1] { Visit(node.ActiveHandlerLocal, null) }),
				new TreeDumperNode("resumeTargetLocal", null, new TreeDumperNode[1] { Visit(node.ResumeTargetLocal, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node, object arg)
		{
			return new TreeDumperNode("unstructuredExceptionOnErrorSwitch", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("value", null, new TreeDumperNode[1] { Visit(node.Value, null) }),
				new TreeDumperNode("jumps", null, node.Jumps.Select((BoundGotoStatement x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node, object arg)
		{
			return new TreeDumperNode("unstructuredExceptionResumeSwitch", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("resumeTargetTemporary", null, new TreeDumperNode[1] { Visit(node.ResumeTargetTemporary, null) }),
				new TreeDumperNode("resumeLabel", null, new TreeDumperNode[1] { Visit(node.ResumeLabel, null) }),
				new TreeDumperNode("resumeNextLabel", null, new TreeDumperNode[1] { Visit(node.ResumeNextLabel, null) }),
				new TreeDumperNode("jumps", null, node.Jumps.Select((BoundGotoStatement x) => Visit(x, null)))
			});
		}

		public override TreeDumperNode VisitAwaitOperator(BoundAwaitOperator node, object arg)
		{
			return new TreeDumperNode("awaitOperator", null, new TreeDumperNode[7]
			{
				new TreeDumperNode("operand", null, new TreeDumperNode[1] { Visit(node.Operand, null) }),
				new TreeDumperNode("awaitableInstancePlaceholder", null, new TreeDumperNode[1] { Visit(node.AwaitableInstancePlaceholder, null) }),
				new TreeDumperNode("getAwaiter", null, new TreeDumperNode[1] { Visit(node.GetAwaiter, null) }),
				new TreeDumperNode("awaiterInstancePlaceholder", null, new TreeDumperNode[1] { Visit(node.AwaiterInstancePlaceholder, null) }),
				new TreeDumperNode("isCompleted", null, new TreeDumperNode[1] { Visit(node.IsCompleted, null) }),
				new TreeDumperNode("getResult", null, new TreeDumperNode[1] { Visit(node.GetResult, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitSpillSequence(BoundSpillSequence node, object arg)
		{
			return new TreeDumperNode("spillSequence", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("locals", node.Locals, null),
				new TreeDumperNode("spillFields", node.SpillFields, null),
				new TreeDumperNode("statements", null, node.Statements.Select((BoundStatement x) => Visit(x, null))),
				new TreeDumperNode("valueOpt", null, new TreeDumperNode[1] { Visit(node.ValueOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitStopStatement(BoundStopStatement node, object arg)
		{
			return new TreeDumperNode("stopStatement", null, Array.Empty<TreeDumperNode>());
		}

		public override TreeDumperNode VisitEndStatement(BoundEndStatement node, object arg)
		{
			return new TreeDumperNode("endStatement", null, Array.Empty<TreeDumperNode>());
		}

		public override TreeDumperNode VisitMidResult(BoundMidResult node, object arg)
		{
			return new TreeDumperNode("midResult", null, new TreeDumperNode[5]
			{
				new TreeDumperNode("original", null, new TreeDumperNode[1] { Visit(node.Original, null) }),
				new TreeDumperNode("start", null, new TreeDumperNode[1] { Visit(node.Start, null) }),
				new TreeDumperNode("lengthOpt", null, new TreeDumperNode[1] { Visit(node.LengthOpt, null) }),
				new TreeDumperNode("source", null, new TreeDumperNode[1] { Visit(node.Source, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitConditionalAccess(BoundConditionalAccess node, object arg)
		{
			return new TreeDumperNode("conditionalAccess", null, new TreeDumperNode[4]
			{
				new TreeDumperNode("receiver", null, new TreeDumperNode[1] { Visit(node.Receiver, null) }),
				new TreeDumperNode("placeholder", null, new TreeDumperNode[1] { Visit(node.Placeholder, null) }),
				new TreeDumperNode("accessExpression", null, new TreeDumperNode[1] { Visit(node.AccessExpression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node, object arg)
		{
			return new TreeDumperNode("conditionalAccessReceiverPlaceholder", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("placeholderId", node.PlaceholderId, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node, object arg)
		{
			return new TreeDumperNode("loweredConditionalAccess", null, new TreeDumperNode[6]
			{
				new TreeDumperNode("receiverOrCondition", null, new TreeDumperNode[1] { Visit(node.ReceiverOrCondition, null) }),
				new TreeDumperNode("captureReceiver", node.CaptureReceiver, null),
				new TreeDumperNode("placeholderId", node.PlaceholderId, null),
				new TreeDumperNode("whenNotNull", null, new TreeDumperNode[1] { Visit(node.WhenNotNull, null) }),
				new TreeDumperNode("whenNullOpt", null, new TreeDumperNode[1] { Visit(node.WhenNullOpt, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node, object arg)
		{
			return new TreeDumperNode("complexConditionalAccessReceiver", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("valueTypeReceiver", null, new TreeDumperNode[1] { Visit(node.ValueTypeReceiver, null) }),
				new TreeDumperNode("referenceTypeReceiver", null, new TreeDumperNode[1] { Visit(node.ReferenceTypeReceiver, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitNameOfOperator(BoundNameOfOperator node, object arg)
		{
			return new TreeDumperNode("nameOfOperator", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("argument", null, new TreeDumperNode[1] { Visit(node.Argument, null) }),
				new TreeDumperNode("constantValueOpt", node.ConstantValueOpt, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitTypeAsValueExpression(BoundTypeAsValueExpression node, object arg)
		{
			return new TreeDumperNode("typeAsValueExpression", null, new TreeDumperNode[2]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node, object arg)
		{
			return new TreeDumperNode("interpolatedStringExpression", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("contents", null, node.Contents.Select((BoundNode x) => Visit(x, null))),
				new TreeDumperNode("binder", node.Binder, null),
				new TreeDumperNode("type", node.Type, null)
			});
		}

		public override TreeDumperNode VisitInterpolation(BoundInterpolation node, object arg)
		{
			return new TreeDumperNode("interpolation", null, new TreeDumperNode[3]
			{
				new TreeDumperNode("expression", null, new TreeDumperNode[1] { Visit(node.Expression, null) }),
				new TreeDumperNode("alignmentOpt", null, new TreeDumperNode[1] { Visit(node.AlignmentOpt, null) }),
				new TreeDumperNode("formatStringOpt", null, new TreeDumperNode[1] { Visit(node.FormatStringOpt, null) })
			});
		}
	}
}
