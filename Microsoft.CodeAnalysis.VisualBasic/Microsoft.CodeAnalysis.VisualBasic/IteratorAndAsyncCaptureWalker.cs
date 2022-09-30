using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class IteratorAndAsyncCaptureWalker : DataFlowPass
	{
		public struct Result
		{
			public readonly OrderedSet<Symbol> CapturedLocals;

			public readonly Dictionary<LocalSymbol, BoundExpression> ByRefLocalsInitializers;

			internal Result(OrderedSet<Symbol> cl, Dictionary<LocalSymbol, BoundExpression> initializers)
			{
				this = default(Result);
				CapturedLocals = cl;
				ByRefLocalsInitializers = initializers;
			}
		}

		private readonly OrderedSet<Symbol> _variablesToHoist;

		private readonly Dictionary<LocalSymbol, BoundExpression> _byRefLocalsInitializers;

		private MultiDictionary<Symbol, SyntaxNode> _lazyDisallowedCaptures;

		protected override bool IgnoreOutSemantics => false;

		protected override bool EnableBreakingFlowAnalysisFeatures => true;

		protected override bool ProcessCompilerGeneratedLocals => true;

		public IteratorAndAsyncCaptureWalker(FlowAnalysisInfo info)
			: base(info, default(FlowAnalysisRegionInfo), suppressConstExpressionsSupport: false, null, trackUnassignments: true, trackStructsWithIntrinsicTypedFields: true)
		{
			_variablesToHoist = new OrderedSet<Symbol>();
			_byRefLocalsInitializers = new Dictionary<LocalSymbol, BoundExpression>();
		}

		public static Result Analyze(FlowAnalysisInfo info, DiagnosticBag diagnostics)
		{
			IteratorAndAsyncCaptureWalker iteratorAndAsyncCaptureWalker = new IteratorAndAsyncCaptureWalker(info);
			iteratorAndAsyncCaptureWalker._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = true;
			iteratorAndAsyncCaptureWalker.Analyze();
			OrderedSet<Symbol> variablesToHoist = iteratorAndAsyncCaptureWalker._variablesToHoist;
			VariableIdentifier[] array = iteratorAndAsyncCaptureWalker.variableBySlot;
			Dictionary<LocalSymbol, BoundExpression> byRefLocalsInitializers = iteratorAndAsyncCaptureWalker._byRefLocalsInitializers;
			MultiDictionary<Symbol, SyntaxNode> lazyDisallowedCaptures = iteratorAndAsyncCaptureWalker._lazyDisallowedCaptures;
			iteratorAndAsyncCaptureWalker.Free();
			if (lazyDisallowedCaptures != null)
			{
				foreach (Symbol key in lazyDisallowedCaptures.Keys)
				{
					TypeSymbol typeSymbol = ((key.Kind == SymbolKind.Local) ? (key as LocalSymbol).Type : (key as ParameterSymbol).Type);
					foreach (SyntaxNode item in lazyDisallowedCaptures[key])
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_CannotLiftRestrictedTypeResumable1, item.GetLocation(), typeSymbol);
					}
				}
			}
			if (info.Compilation.Options.OptimizationLevel != OptimizationLevel.Release)
			{
				bool isIterator = ((MethodSymbol)info.Symbol).IsIterator;
				VariableIdentifier[] array2 = array;
				for (int i = 0; i < array2.Length; i = checked(i + 1))
				{
					VariableIdentifier variableIdentifier = array2[i];
					if ((object)variableIdentifier.Symbol != null && HoistInDebugBuild(variableIdentifier.Symbol, isIterator))
					{
						variablesToHoist.Add(variableIdentifier.Symbol);
					}
				}
			}
			return new Result(variablesToHoist, byRefLocalsInitializers);
		}

		private static bool HoistInDebugBuild(Symbol symbol, bool skipByRefLocals)
		{
			if (symbol.Kind == SymbolKind.Parameter)
			{
				return !TypeSymbolExtensions.IsRestrictedType((symbol as ParameterSymbol).Type);
			}
			if (symbol.Kind == SymbolKind.Local)
			{
				LocalSymbol localSymbol = symbol as LocalSymbol;
				if (localSymbol.IsConst)
				{
					return false;
				}
				if (skipByRefLocals && localSymbol.IsByRef)
				{
					return false;
				}
				if (localSymbol.SynthesizedKind == SynthesizedLocalKind.UserDefined)
				{
					return !TypeSymbolExtensions.IsRestrictedType(localSymbol.Type);
				}
				return localSymbol.SynthesizedKind != SynthesizedLocalKind.ConditionalBranchDiscriminator;
			}
			return false;
		}

		protected override bool Scan()
		{
			_variablesToHoist.Clear();
			_byRefLocalsInitializers.Clear();
			_lazyDisallowedCaptures?.Clear();
			return base.Scan();
		}

		private void CaptureVariable(Symbol variable, SyntaxNode syntax)
		{
			if (TypeSymbolExtensions.IsRestrictedType((variable.Kind == SymbolKind.Local) ? (variable as LocalSymbol).Type : (variable as ParameterSymbol).Type))
			{
				if (!(variable is SynthesizedLocal))
				{
					if (_lazyDisallowedCaptures == null)
					{
						_lazyDisallowedCaptures = new MultiDictionary<Symbol, SyntaxNode>();
					}
					_lazyDisallowedCaptures.Add(variable, syntax);
				}
			}
			else if (compilation.Options.OptimizationLevel == OptimizationLevel.Release)
			{
				_variablesToHoist.Add(variable);
			}
		}

		protected override void EnterParameter(ParameterSymbol parameter)
		{
			GetOrCreateSlot(parameter);
			CaptureVariable(parameter, null);
		}

		protected override void ReportUnassigned(Symbol symbol, SyntaxNode node, ReadWriteContext rwContext, int slot = -1, BoundFieldAccess boundFieldAccess = null)
		{
			if (symbol.Kind == SymbolKind.Field)
			{
				Symbol nodeSymbol = GetNodeSymbol(boundFieldAccess);
				if ((object)nodeSymbol != null)
				{
					CaptureVariable(nodeSymbol, node);
				}
			}
			else if (symbol.Kind == SymbolKind.Parameter || symbol.Kind == SymbolKind.Local)
			{
				CaptureVariable(symbol, node);
			}
		}

		protected override bool IsEmptyStructType(TypeSymbol type)
		{
			return false;
		}

		private void MarkLocalsUnassigned()
		{
			int num = nextVariableSlot - 1;
			for (int i = 2; i <= num; i++)
			{
				Symbol symbol = variableBySlot[i].Symbol;
				switch (symbol.Kind)
				{
				case SymbolKind.Local:
					if (!((LocalSymbol)symbol).IsConst)
					{
						SetSlotState(i, assigned: false);
					}
					break;
				case SymbolKind.Parameter:
					SetSlotState(i, assigned: false);
					break;
				}
			}
		}

		public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			base.VisitAwaitOperator(node);
			MarkLocalsUnassigned();
			return null;
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			BoundNode boundNode = null;
			ImmutableArray<LocalSymbol>.Enumerator enumerator = node.Locals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				LocalSymbol current = enumerator.Current;
				SetSlotState(GetOrCreateSlot(current), assigned: true);
			}
			boundNode = base.VisitSequence(node);
			ImmutableArray<LocalSymbol>.Enumerator enumerator2 = node.Locals.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				LocalSymbol current2 = enumerator2.Current;
				CheckAssigned(current2, node.Syntax);
			}
			return boundNode;
		}

		public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
		{
			LocalSymbol localSymbol = node.ByRefLocal.LocalSymbol;
			_byRefLocalsInitializers.Add(localSymbol, node.LValue);
			return base.VisitReferenceAssignment(node);
		}

		public override BoundNode VisitYieldStatement(BoundYieldStatement node)
		{
			base.VisitYieldStatement(node);
			MarkLocalsUnassigned();
			return null;
		}

		protected override bool TreatTheLocalAsAssignedWithinTheLambda(LocalSymbol local, BoundExpression right)
		{
			if (right.Kind == BoundKind.ObjectCreationExpression)
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)right;
				if (boundObjectCreationExpression.Type is LambdaFrame && boundObjectCreationExpression.Arguments.Length == 1)
				{
					BoundExpression boundExpression = boundObjectCreationExpression.Arguments[0];
					if (boundExpression.Kind == BoundKind.Local && (object)((BoundLocal)boundExpression).LocalSymbol == local)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
