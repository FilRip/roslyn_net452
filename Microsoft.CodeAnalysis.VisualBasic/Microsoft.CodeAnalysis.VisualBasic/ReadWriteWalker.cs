using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ReadWriteWalker : AbstractRegionDataFlowPass
	{
		private readonly HashSet<Symbol> _readInside;

		private readonly HashSet<Symbol> _writtenInside;

		private readonly HashSet<Symbol> _readOutside;

		private readonly HashSet<Symbol> _writtenOutside;

		private readonly HashSet<Symbol> _captured;

		private readonly HashSet<Symbol> _capturedInside;

		private readonly HashSet<Symbol> _capturedOutside;

		private Symbol _currentMethodOrLambda;

		private BoundQueryLambda _currentQueryLambda;

		internal static void Analyze(FlowAnalysisInfo info, FlowAnalysisRegionInfo region, ref IEnumerable<Symbol> readInside, ref IEnumerable<Symbol> writtenInside, ref IEnumerable<Symbol> readOutside, ref IEnumerable<Symbol> writtenOutside, ref IEnumerable<Symbol> captured, ref IEnumerable<Symbol> capturedInside, ref IEnumerable<Symbol> capturedOutside)
		{
			ReadWriteWalker readWriteWalker = new ReadWriteWalker(info, region);
			try
			{
				if (readWriteWalker.Analyze())
				{
					readInside = readWriteWalker._readInside;
					writtenInside = readWriteWalker._writtenInside;
					readOutside = readWriteWalker._readOutside;
					writtenOutside = readWriteWalker._writtenOutside;
					captured = readWriteWalker._captured;
					capturedInside = readWriteWalker._capturedInside;
					capturedOutside = readWriteWalker._capturedOutside;
				}
				else
				{
					readInside = Enumerable.Empty<Symbol>();
					writtenInside = readInside;
					readOutside = readInside;
					writtenOutside = readInside;
					captured = readInside;
					capturedInside = readInside;
					capturedOutside = readInside;
				}
			}
			finally
			{
				readWriteWalker.Free();
			}
		}

		protected override void NoteRead(Symbol variable)
		{
			if (IsCompilerGeneratedTempLocal(variable))
			{
				base.NoteRead(variable);
				return;
			}
			switch (_regionPlace)
			{
			case RegionPlace.Before:
			case RegionPlace.After:
				_readOutside.Add(variable);
				break;
			case RegionPlace.Inside:
				_readInside.Add(variable);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(_regionPlace);
			}
			base.NoteRead(variable);
			CheckCaptured(variable);
		}

		protected override void NoteWrite(Symbol variable, BoundExpression value)
		{
			if (IsCompilerGeneratedTempLocal(variable))
			{
				base.NoteWrite(variable, value);
				return;
			}
			switch (_regionPlace)
			{
			case RegionPlace.Before:
			case RegionPlace.After:
				_writtenOutside.Add(variable);
				break;
			case RegionPlace.Inside:
				_writtenInside.Add(variable);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(_regionPlace);
			}
			base.NoteWrite(variable, value);
			CheckCaptured(variable);
		}

		private void NoteCaptured(Symbol variable)
		{
			if (_regionPlace == RegionPlace.Inside)
			{
				_capturedInside.Add(variable);
				_captured.Add(variable);
			}
			else if (variable.Kind != SymbolKind.RangeVariable)
			{
				_capturedOutside.Add(variable);
				_captured.Add(variable);
			}
		}

		protected override void NoteRead(BoundFieldAccess fieldAccess)
		{
			base.NoteRead(fieldAccess);
			if (_regionPlace != RegionPlace.Inside && fieldAccess.Syntax.Span.Contains(_region))
			{
				NoteReceiverRead(fieldAccess);
			}
		}

		protected override void NoteWrite(BoundExpression node, BoundExpression value)
		{
			base.NoteWrite(node, value);
			if (node.Kind == BoundKind.FieldAccess)
			{
				NoteReceiverWritten((BoundFieldAccess)node);
			}
		}

		private void NoteReceiverRead(BoundFieldAccess fieldAccess)
		{
			NoteReceiverReadOrWritten(fieldAccess, _readInside);
		}

		private void NoteReceiverWritten(BoundFieldAccess fieldAccess)
		{
			NoteReceiverReadOrWritten(fieldAccess, _writtenInside);
		}

		private void NoteReceiverReadOrWritten(BoundFieldAccess fieldAccess, HashSet<Symbol> readOrWritten)
		{
			if (fieldAccess.FieldSymbol.IsShared || fieldAccess.FieldSymbol.ContainingType.IsReferenceType)
			{
				return;
			}
			BoundExpression receiverOpt = fieldAccess.ReceiverOpt;
			if (receiverOpt == null)
			{
				return;
			}
			SyntaxNode syntax = receiverOpt.Syntax;
			if (syntax == null)
			{
				return;
			}
			switch (receiverOpt.Kind)
			{
			case BoundKind.Local:
			{
				TextSpan region = _region;
				if (region.Contains(syntax.Span))
				{
					readOrWritten.Add(((BoundLocal)receiverOpt).LocalSymbol);
				}
				break;
			}
			case BoundKind.MeReference:
			{
				TextSpan region = _region;
				if (region.Contains(syntax.Span))
				{
					readOrWritten.Add(MeParameter);
				}
				break;
			}
			case BoundKind.MyBaseReference:
			{
				TextSpan region = _region;
				if (region.Contains(syntax.Span))
				{
					readOrWritten.Add(MeParameter);
				}
				break;
			}
			case BoundKind.Parameter:
			{
				TextSpan region = _region;
				if (region.Contains(syntax.Span))
				{
					readOrWritten.Add(((BoundParameter)receiverOpt).ParameterSymbol);
				}
				break;
			}
			case BoundKind.RangeVariable:
			{
				TextSpan region = _region;
				if (region.Contains(syntax.Span))
				{
					readOrWritten.Add(((BoundRangeVariable)receiverOpt).RangeVariable);
				}
				break;
			}
			case BoundKind.FieldAccess:
				if (TypeSymbolExtensions.IsStructureType(receiverOpt.Type) && syntax.Span.OverlapsWith(_region))
				{
					NoteReceiverReadOrWritten((BoundFieldAccess)receiverOpt, readOrWritten);
				}
				break;
			}
		}

		private static bool IsCompilerGeneratedTempLocal(Symbol variable)
		{
			return variable is SynthesizedLocal;
		}

		private void CheckCaptured(Symbol variable)
		{
			switch (variable.Kind)
			{
			case SymbolKind.Local:
			{
				LocalSymbol localSymbol = (LocalSymbol)variable;
				if (!localSymbol.IsConst && _currentMethodOrLambda != localSymbol.ContainingSymbol)
				{
					NoteCaptured(localSymbol);
				}
				break;
			}
			case SymbolKind.Parameter:
			{
				ParameterSymbol parameterSymbol = (ParameterSymbol)variable;
				if (_currentMethodOrLambda != parameterSymbol.ContainingSymbol)
				{
					NoteCaptured(parameterSymbol);
				}
				break;
			}
			case SymbolKind.RangeVariable:
			{
				RangeVariableSymbol rangeVariableSymbol = (RangeVariableSymbol)variable;
				if (_currentMethodOrLambda != rangeVariableSymbol.ContainingSymbol && _currentQueryLambda != null && (_currentMethodOrLambda != _currentQueryLambda.LambdaSymbol || !_currentQueryLambda.RangeVariables.Contains(rangeVariableSymbol)))
				{
					NoteCaptured(rangeVariableSymbol);
				}
				break;
			}
			}
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			Symbol currentMethodOrLambda = _currentMethodOrLambda;
			_currentMethodOrLambda = node.LambdaSymbol;
			BoundNode result = base.VisitLambda(node);
			_currentMethodOrLambda = currentMethodOrLambda;
			return result;
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			Symbol currentMethodOrLambda = _currentMethodOrLambda;
			_currentMethodOrLambda = node.LambdaSymbol;
			BoundQueryLambda currentQueryLambda = _currentQueryLambda;
			_currentQueryLambda = node;
			BoundNode result = base.VisitQueryLambda(node);
			_currentMethodOrLambda = currentMethodOrLambda;
			_currentQueryLambda = currentQueryLambda;
			return result;
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			if (!node.WasCompilerGenerated && (object)node.RangeVariableOpt != null)
			{
				NoteWrite(node.RangeVariableOpt, null);
			}
			VisitRvalue(node.Source);
			return null;
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			if (!node.WasCompilerGenerated)
			{
				NoteWrite(node.RangeVariable, null);
			}
			VisitRvalue(node.Value);
			return null;
		}

		private new bool Analyze()
		{
			return Scan();
		}

		private ReadWriteWalker(FlowAnalysisInfo info, FlowAnalysisRegionInfo region)
			: base(info, region)
		{
			_readInside = new HashSet<Symbol>();
			_writtenInside = new HashSet<Symbol>();
			_readOutside = new HashSet<Symbol>();
			_writtenOutside = new HashSet<Symbol>();
			_captured = new HashSet<Symbol>();
			_capturedInside = new HashSet<Symbol>();
			_capturedOutside = new HashSet<Symbol>();
			_currentMethodOrLambda = symbol;
		}
	}
}
