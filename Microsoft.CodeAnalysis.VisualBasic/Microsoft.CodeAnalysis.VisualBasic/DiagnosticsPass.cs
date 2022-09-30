using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class DiagnosticsPass : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
	{
		private readonly DiagnosticBag _diagnostics;

		private readonly VisualBasicCompilation _compilation;

		private MethodSymbol _containingSymbol;

		private Dictionary<BoundValuePlaceholderBase, BoundWithStatement> _withExpressionPlaceholderMap;

		private Stack<BoundExpression> _expressionsBeingVisited;

		private bool _insideNameof;

		private bool _inExpressionLambda;

		private readonly HashSet<BoundNode> _expressionTreePlaceholders;

		private bool IsInExpressionLambda => _inExpressionLambda;

		public static void IssueDiagnostics(BoundNode node, DiagnosticBag diagnostics, MethodSymbol containingSymbol)
		{
			try
			{
				new DiagnosticsPass(containingSymbol.DeclaringCompilation, diagnostics, containingSymbol).Visit(node);
			}
			catch (CancelledByStackGuardException ex)
			{
				ProjectData.SetProjectError(ex);
				CancelledByStackGuardException ex2 = ex;
				ex2.AddAnError(diagnostics);
				ProjectData.ClearProjectError();
			}
		}

		private DiagnosticsPass(VisualBasicCompilation compilation, DiagnosticBag diagnostics, MethodSymbol containingSymbol)
		{
			_insideNameof = false;
			_expressionTreePlaceholders = new HashSet<BoundNode>(ReferenceEqualityComparer.Instance);
			_compilation = compilation;
			_diagnostics = diagnostics;
			_containingSymbol = containingSymbol;
			_inExpressionLambda = false;
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			MethodSymbol containingSymbol = _containingSymbol;
			_containingSymbol = node.LambdaSymbol;
			Visit(node.Expression);
			_containingSymbol = containingSymbol;
			return null;
		}

		public override BoundNode VisitParameter(BoundParameter node)
		{
			ParameterSymbol parameterSymbol = node.ParameterSymbol;
			if (parameterSymbol.IsByRef)
			{
				Symbol containingSymbol = parameterSymbol.ContainingSymbol;
				if ((object)_containingSymbol != containingSymbol && !_insideNameof)
				{
					if (Binder.IsTopMostEnclosingLambdaAQueryLambda(_containingSymbol, containingSymbol))
					{
						Binder.ReportDiagnostic(_diagnostics, node.Syntax, ERRID.ERR_CannotLiftByRefParamQuery1, parameterSymbol.Name);
					}
					else
					{
						Binder.ReportDiagnostic(_diagnostics, node.Syntax, ERRID.ERR_CannotLiftByRefParamLambda1, parameterSymbol.Name);
					}
				}
			}
			return base.VisitParameter(node);
		}

		public override BoundNode VisitMeReference(BoundMeReference node)
		{
			ERRID meAccessError = GetMeAccessError();
			if (meAccessError != 0)
			{
				Binder.ReportDiagnostic(_diagnostics, node.Syntax, meAccessError);
			}
			return base.VisitMeReference(node);
		}

		public override BoundNode VisitMyClassReference(BoundMyClassReference node)
		{
			ERRID meAccessError = GetMeAccessError();
			if (meAccessError != 0)
			{
				Binder.ReportDiagnostic(_diagnostics, node.Syntax, meAccessError);
			}
			return base.VisitMyClassReference(node);
		}

		private ERRID GetMeAccessError()
		{
			ParameterSymbol meParameter = _containingSymbol.MeParameter;
			if ((object)meParameter != null && meParameter.IsByRef && _containingSymbol.MethodKind == MethodKind.AnonymousFunction)
			{
				if (Binder.IsTopMostEnclosingLambdaAQueryLambda(_containingSymbol, null))
				{
					return ERRID.ERR_CannotLiftStructureMeQuery;
				}
				return ERRID.ERR_CannotLiftStructureMeLambda;
			}
			return ERRID.ERR_None;
		}

		public override BoundNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node)
		{
			CheckMeAccessInWithExpression(node);
			return base.VisitWithRValueExpressionPlaceholder(node);
		}

		private void CheckMeAccessInWithExpression(BoundValuePlaceholderBase node)
		{
			BoundWithStatement value = null;
			if (_withExpressionPlaceholderMap == null || !_withExpressionPlaceholderMap.TryGetValue(node, out value))
			{
				return;
			}
			WithBlockBinder binder = value.Binder;
			if ((object)binder.ContainingMember == _containingSymbol || binder.IsInLambda)
			{
				return;
			}
			NamedTypeSymbol containingType = binder.ContainingType;
			if ((object)containingType == null || !containingType.IsValueType || !binder.Info.ExpressionHasByRefMeReference(base.RecursionDepth))
			{
				return;
			}
			ERRID meAccessError = GetMeAccessError();
			if (meAccessError == ERRID.ERR_None)
			{
				return;
			}
			SyntaxNode syntax = node.Syntax;
			foreach (BoundExpression item in _expressionsBeingVisited)
			{
				if (item.Syntax != syntax)
				{
					syntax = item.Syntax;
					break;
				}
			}
			Binder.ReportDiagnostic(_diagnostics, syntax, meAccessError);
		}

		public override BoundNode VisitWithStatement(BoundWithStatement node)
		{
			Visit(node.OriginalExpression);
			WithBlockBinder.WithBlockInfo withBlockInfo = node.Binder.Info;
			if (withBlockInfo != null && withBlockInfo.ExpressionIsAccessedFromNestedLambda)
			{
				if (_withExpressionPlaceholderMap == null)
				{
					_withExpressionPlaceholderMap = new Dictionary<BoundValuePlaceholderBase, BoundWithStatement>();
					_expressionsBeingVisited = new Stack<BoundExpression>();
				}
				_withExpressionPlaceholderMap.Add(withBlockInfo.ExpressionPlaceholder, node);
			}
			else
			{
				withBlockInfo = null;
			}
			Visit(node.Body);
			if (withBlockInfo != null)
			{
				_withExpressionPlaceholderMap.Remove(withBlockInfo.ExpressionPlaceholder);
			}
			return null;
		}

		public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
		{
			_insideNameof = true;
			BoundNode result = base.VisitNameOfOperator(node);
			_insideNameof = false;
			return result;
		}

		public override BoundNode Visit(BoundNode node)
		{
			bool flag = false;
			if (_withExpressionPlaceholderMap != null && _withExpressionPlaceholderMap.Count > 0 && node is BoundExpression item)
			{
				_expressionsBeingVisited.Push(item);
				flag = true;
			}
			BoundNode result = base.Visit(node);
			if (flag)
			{
				_expressionsBeingVisited.Pop();
			}
			return result;
		}

		public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			if (IsInExpressionLambda)
			{
				BoundObjectInitializerExpressionBase initializerOpt = node.InitializerOpt;
				if (initializerOpt != null && initializerOpt.Kind == BoundKind.ObjectInitializerExpression && (object)node.ConstantValueOpt == null && initializerOpt.Type.IsValueType && (object)node.ConstructorOpt != null && node.Arguments.Length > 0)
				{
					GenerateExpressionTreeNotSupportedDiagnostic(initializerOpt);
				}
			}
			return base.VisitObjectCreationExpression(node);
		}

		public override BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			if (IsInExpressionLambda)
			{
				UnaryOperatorKind unaryOperatorKind = node.OperatorKind & UnaryOperatorKind.OpMask;
				bool flag = (node.OperatorKind & UnaryOperatorKind.Lifted) != 0;
				if ((uint)(unaryOperatorKind - 1) <= 2u && flag && TypeSymbolExtensions.IsNullableType(node.Call.Method.ReturnType))
				{
					GenerateExpressionTreeNotSupportedDiagnostic(node);
				}
			}
			return base.VisitUserDefinedUnaryOperator(node);
		}

		public override BoundNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node)
		{
			if (IsInExpressionLambda)
			{
				GenerateDiagnostic(ERRID.ERR_BadAnonymousTypeForExprTree, node);
			}
			return base.VisitAnonymousTypePropertyAccess(node);
		}

		public override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			if (!node.Locals.IsEmpty && IsInExpressionLambda)
			{
				GenerateExpressionTreeNotSupportedDiagnostic(node);
			}
			return base.VisitSequence(node);
		}

		public override BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			if (IsInExpressionLambda)
			{
				BinaryOperatorKind binaryOperatorKind = node.OperatorKind & BinaryOperatorKind.OpMask;
				if ((uint)(binaryOperatorKind - 2) > 1u && (node.OperatorKind & BinaryOperatorKind.Lifted) != 0 && TypeSymbolExtensions.IsNullableType(node.Call.Method.ReturnType))
				{
					GenerateExpressionTreeNotSupportedDiagnostic(node);
				}
			}
			return base.VisitUserDefinedBinaryOperator(node);
		}

		public override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
		{
			if (!IsInExpressionLambda)
			{
				return base.VisitObjectInitializerExpression(node);
			}
			BoundWithLValueExpressionPlaceholder placeholderOpt = node.PlaceholderOpt;
			Visit(placeholderOpt);
			_expressionTreePlaceholders.Add(placeholderOpt);
			ImmutableArray<BoundExpression>.Enumerator enumerator = node.Initializers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundAssignmentOperator boundAssignmentOperator = (BoundAssignmentOperator)enumerator.Current;
				if (boundAssignmentOperator.Left is BoundPropertyAccess node2)
				{
					CheckRefReturningPropertyAccess(node2);
				}
				Visit(boundAssignmentOperator.Right);
			}
			_expressionTreePlaceholders.Remove(placeholderOpt);
			return null;
		}

		public override BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
		{
			if (_expressionTreePlaceholders.Contains(node))
			{
				GenerateExpressionTreeNotSupportedDiagnostic(node);
			}
			CheckMeAccessInWithExpression(node);
			return base.VisitWithLValueExpressionPlaceholder(node);
		}

		public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			if (IsInExpressionLambda && (node.Left.Kind != BoundKind.PropertyAccess || node.LeftOnTheRightOpt != null))
			{
				GenerateExpressionTreeNotSupportedDiagnostic(node);
			}
			return base.VisitAssignmentOperator(node);
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			if (!node.FieldSymbol.IsShared)
			{
				Visit(node.ReceiverOpt);
			}
			return null;
		}

		public override BoundNode VisitArrayCreation(BoundArrayCreation node)
		{
			if (IsInExpressionLambda && !((ArrayTypeSymbol)node.Type).IsSZArray)
			{
				BoundArrayInitialization initializerOpt = node.InitializerOpt;
				if (initializerOpt != null && !initializerOpt.Initializers.IsEmpty)
				{
					GenerateDiagnostic(ERRID.ERR_ExprTreeNoMultiDimArrayCreation, node);
				}
			}
			return base.VisitArrayCreation(node);
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			if (IsInExpressionLambda)
			{
				LambdaSymbol lambdaSymbol = node.LambdaSymbol;
				if (lambdaSymbol.IsAsync || lambdaSymbol.IsIterator)
				{
					GenerateDiagnostic(ERRID.ERR_ResumableLambdaInExpressionTree, node);
				}
				else if (!node.WasCompilerGenerated && !node.IsSingleLine)
				{
					GenerateDiagnostic(ERRID.ERR_StatementLambdaInExpressionTree, node);
				}
				else
				{
					switch (VisualBasicExtensions.Kind(lambdaSymbol.Syntax))
					{
					case SyntaxKind.MultiLineFunctionLambdaExpression:
					case SyntaxKind.MultiLineSubLambdaExpression:
						GenerateDiagnostic(ERRID.ERR_StatementLambdaInExpressionTree, node);
						break;
					case SyntaxKind.SingleLineFunctionLambdaExpression:
					case SyntaxKind.SingleLineSubLambdaExpression:
					{
						bool flag = true;
						BoundBlock body = node.Body;
						if (body.Statements.Length == 1 || (body.Statements.Length == 2 && body.Statements[1].Kind == BoundKind.ReturnStatement && ((BoundReturnStatement)body.Statements[1]).ExpressionOpt == null) || (body.Statements.Length == 3 && body.Statements[1].Kind == BoundKind.LabelStatement && body.Statements[2].Kind == BoundKind.ReturnStatement))
						{
							BoundStatement boundStatement = body.Statements[0];
							while (true)
							{
								BoundBlock boundBlock;
								switch (boundStatement.Kind)
								{
								case BoundKind.ReturnStatement:
									if (((BoundReturnStatement)boundStatement).ExpressionOpt != null)
									{
										flag = false;
									}
									break;
								case BoundKind.ExpressionStatement:
								case BoundKind.AddHandlerStatement:
								case BoundKind.RemoveHandlerStatement:
									flag = false;
									break;
								case BoundKind.Block:
									boundBlock = (BoundBlock)boundStatement;
									if (boundBlock.Locals.IsEmpty && boundBlock.Statements.Length == 1)
									{
										goto IL_01db;
									}
									break;
								}
								break;
								IL_01db:
								boundStatement = boundBlock.Statements[0];
							}
						}
						if (flag)
						{
							GenerateDiagnostic(ERRID.ERR_StatementLambdaInExpressionTree, node);
						}
						break;
					}
					}
				}
			}
			MethodSymbol containingSymbol = _containingSymbol;
			_containingSymbol = node.LambdaSymbol;
			Visit(node.Body);
			_containingSymbol = containingSymbol;
			return null;
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			MethodSymbol method = node.Method;
			if (!method.IsShared)
			{
				Visit(node.ReceiverOpt);
			}
			if (IsInExpressionLambda & method.ReturnsByRef)
			{
				GenerateDiagnostic(ERRID.ERR_RefReturningCallInExpressionTree, node);
			}
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			if (!node.PropertySymbol.IsShared)
			{
				Visit(node.ReceiverOpt);
			}
			CheckRefReturningPropertyAccess(node);
			VisitList(node.Arguments);
			return null;
		}

		private void CheckRefReturningPropertyAccess(BoundPropertyAccess node)
		{
			if (IsInExpressionLambda && node.PropertySymbol.ReturnsByRef)
			{
				GenerateDiagnostic(ERRID.ERR_RefReturningCallInExpressionTree, node);
			}
		}

		public override BoundNode VisitEventAccess(BoundEventAccess node)
		{
			if (!node.EventSymbol.IsShared)
			{
				Visit(node.ReceiverOpt);
			}
			return null;
		}

		private void VisitLambdaConversion(BoundExpression operand, BoundLambda relaxationLambda)
		{
			if (operand.Kind == BoundKind.Lambda && !CheckLambdaForByRefParameters((BoundLambda)operand) && relaxationLambda != null)
			{
				CheckLambdaForByRefParameters(relaxationLambda);
			}
			Visit(operand);
		}

		private bool CheckLambdaForByRefParameters(BoundLambda lambda)
		{
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = lambda.LambdaSymbol.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsByRef)
				{
					GenerateDiagnostic(ERRID.ERR_ByRefParamInExpressionTree, lambda);
					return true;
				}
			}
			return false;
		}

		public override BoundNode VisitConversion(BoundConversion node)
		{
			bool inExpressionLambda = _inExpressionLambda;
			if ((node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0)
			{
				_inExpressionLambda = true;
			}
			if (IsInExpressionLambda && (node.ConversionKind & ConversionKind.Lambda) != 0)
			{
				VisitLambdaConversion(node.Operand, ((BoundRelaxationLambda)node.ExtendedInfoOpt)?.Lambda);
			}
			else
			{
				base.VisitConversion(node);
			}
			_inExpressionLambda = inExpressionLambda;
			return null;
		}

		public override BoundNode VisitTryCast(BoundTryCast node)
		{
			bool inExpressionLambda = _inExpressionLambda;
			if ((node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0)
			{
				_inExpressionLambda = true;
			}
			if (IsInExpressionLambda && (node.ConversionKind & ConversionKind.Lambda) != 0)
			{
				VisitLambdaConversion(node.Operand, node.RelaxationLambdaOpt);
			}
			else
			{
				base.VisitTryCast(node);
			}
			_inExpressionLambda = inExpressionLambda;
			return null;
		}

		public override BoundNode VisitDirectCast(BoundDirectCast node)
		{
			bool inExpressionLambda = _inExpressionLambda;
			if ((node.ConversionKind & ConversionKind.ConvertedToExpressionTree) != 0)
			{
				_inExpressionLambda = true;
			}
			if (IsInExpressionLambda && (node.ConversionKind & ConversionKind.Lambda) != 0)
			{
				VisitLambdaConversion(node.Operand, node.RelaxationLambdaOpt);
			}
			else
			{
				base.VisitDirectCast(node);
			}
			_inExpressionLambda = inExpressionLambda;
			return null;
		}

		public override BoundNode VisitLateInvocation(BoundLateInvocation node)
		{
			if (!IsInExpressionLambda)
			{
				return base.VisitLateInvocation(node);
			}
			GenerateDiagnostic(ERRID.ERR_ExprTreeNoLateBind, node);
			if (node.Member.Kind != BoundKind.LateMemberAccess)
			{
				Visit(node.Member);
			}
			VisitList(node.ArgumentsOpt);
			return null;
		}

		public override BoundNode VisitLateMemberAccess(BoundLateMemberAccess node)
		{
			if (IsInExpressionLambda)
			{
				GenerateDiagnostic(ERRID.ERR_ExprTreeNoLateBind, node);
			}
			return base.VisitLateMemberAccess(node);
		}

		public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
		{
			if (IsInExpressionLambda)
			{
				GenerateDiagnostic(ERRID.ERR_NullPropagatingOpInExpressionTree, node);
			}
			return base.VisitConditionalAccess(node);
		}

		private void GenerateExpressionTreeNotSupportedDiagnostic(BoundNode node)
		{
			GenerateDiagnostic(ERRID.ERR_ExpressionTreeNotSupported, node);
		}

		private void GenerateDiagnostic(ERRID code, BoundNode node)
		{
			_diagnostics.Add(new VBDiagnostic(ErrorFactory.ErrorInfo(code), node.Syntax.GetLocation()));
		}
	}
}
