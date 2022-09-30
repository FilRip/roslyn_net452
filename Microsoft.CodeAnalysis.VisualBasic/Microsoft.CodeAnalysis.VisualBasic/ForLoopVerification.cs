using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ForLoopVerification
	{
		private sealed class ForLoopVerificationWalker : BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		{
			private readonly DiagnosticBag _diagnostics;

			private readonly Stack<BoundExpression> _controlVariables;

			public ForLoopVerificationWalker(DiagnosticBag diagnostics)
			{
				_diagnostics = diagnostics;
				_controlVariables = new Stack<BoundExpression>();
			}

			public override BoundNode VisitForToStatement(BoundForToStatement node)
			{
				PreVisitForAndForEachStatement(node);
				base.VisitForToStatement(node);
				PostVisitForAndForEachStatement(node);
				return null;
			}

			public override BoundNode VisitForEachStatement(BoundForEachStatement node)
			{
				PreVisitForAndForEachStatement(node);
				base.VisitForEachStatement(node);
				PostVisitForAndForEachStatement(node);
				return null;
			}

			private void PreVisitForAndForEachStatement(BoundForStatement boundForStatement)
			{
				BoundExpression controlVariable = boundForStatement.ControlVariable;
				Symbol symbol = ReferencedSymbol(controlVariable);
				if ((object)symbol != null)
				{
					foreach (BoundExpression controlVariable2 in _controlVariables)
					{
						if (ReferencedSymbol(controlVariable2) == symbol)
						{
							DiagnosticBagExtensions.Add(_diagnostics, ERRID.ERR_ForIndexInUse1, controlVariable.Syntax.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(symbol));
							break;
						}
					}
				}
				_controlVariables.Push(controlVariable);
			}

			private void PostVisitForAndForEachStatement(BoundForStatement boundForStatement)
			{
				if (boundForStatement.NextVariablesOpt.IsDefault)
				{
					return;
				}
				if (boundForStatement.NextVariablesOpt.IsEmpty)
				{
					_controlVariables.Pop();
					return;
				}
				ImmutableArray<BoundExpression>.Enumerator enumerator = boundForStatement.NextVariablesOpt.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BoundExpression current = enumerator.Current;
					BoundExpression boundExpression = _controlVariables.Pop();
					if (!boundExpression.HasErrors && !current.HasErrors && ReferencedSymbol(current) != ReferencedSymbol(boundExpression))
					{
						DiagnosticBagExtensions.Add(_diagnostics, ERRID.ERR_NextForMismatch1, current.Syntax.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(ReferencedSymbol(boundExpression)));
					}
				}
			}
		}

		public static void VerifyForLoops(BoundBlock block, DiagnosticBag diagnostics)
		{
			try
			{
				new ForLoopVerificationWalker(diagnostics).Visit(block);
			}
			catch (BoundTreeVisitor.CancelledByStackGuardException ex)
			{
				ProjectData.SetProjectError(ex);
				BoundTreeVisitor.CancelledByStackGuardException ex2 = ex;
				ex2.AddAnError(diagnostics);
				ProjectData.ClearProjectError();
			}
		}

		internal static Symbol ReferencedSymbol(BoundExpression expression)
		{
			return expression.Kind switch
			{
				BoundKind.ArrayAccess => ReferencedSymbol(((BoundArrayAccess)expression).Expression), 
				BoundKind.PropertyAccess => ((BoundPropertyAccess)expression).PropertySymbol, 
				BoundKind.Call => ((BoundCall)expression).Method, 
				BoundKind.Local => ((BoundLocal)expression).LocalSymbol, 
				BoundKind.RangeVariable => ((BoundRangeVariable)expression).RangeVariable, 
				BoundKind.FieldAccess => ((BoundFieldAccess)expression).FieldSymbol, 
				BoundKind.Parameter => ((BoundParameter)expression).ParameterSymbol, 
				BoundKind.Parenthesized => ReferencedSymbol(((BoundParenthesized)expression).Expression), 
				_ => null, 
			};
		}
	}
}
