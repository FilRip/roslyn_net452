using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class WithExpressionRewriter
	{
		public struct Result
		{
			public readonly BoundExpression Expression;

			public readonly ImmutableArray<LocalSymbol> Locals;

			public readonly ImmutableArray<BoundExpression> Initializers;

			public Result(BoundExpression expression, ImmutableArray<LocalSymbol> locals, ImmutableArray<BoundExpression> initializers)
			{
				this = default(Result);
				Expression = expression;
				Locals = locals;
				Initializers = initializers;
			}
		}

		private class State
		{
			public readonly Symbol ContainingMember;

			public readonly bool DoNotUseByRefLocal;

			public readonly Binder Binder;

			public readonly bool PreserveIdentityOfLValues;

			private ArrayBuilder<LocalSymbol> _locals;

			private ArrayBuilder<BoundExpression> _initializers;

			public State(Symbol containingMember, bool doNotUseByRefLocal, Binder binder, bool preserveIdentityOfLValues)
			{
				_locals = null;
				_initializers = null;
				ContainingMember = containingMember;
				DoNotUseByRefLocal = doNotUseByRefLocal;
				Binder = binder;
				PreserveIdentityOfLValues = preserveIdentityOfLValues;
			}

			public void AddLocal(LocalSymbol local, BoundExpression initializer)
			{
				if (_locals == null)
				{
					_locals = ArrayBuilder<LocalSymbol>.GetInstance();
					_initializers = ArrayBuilder<BoundExpression>.GetInstance();
				}
				_locals.Add(local);
				_initializers.Add(initializer);
			}

			public Result CreateResult(BoundExpression expression)
			{
				return new Result(expression, (_locals == null) ? ImmutableArray<LocalSymbol>.Empty : _locals.ToImmutableAndFree(), (_initializers == null) ? ImmutableArray<BoundExpression>.Empty : _initializers.ToImmutableAndFree());
			}
		}

		private readonly WithStatementSyntax _withSyntax;

		internal WithExpressionRewriter(WithStatementSyntax withSyntax)
		{
			_withSyntax = withSyntax;
		}

		private BoundLocal CaptureInATemp(BoundExpression value, State state)
		{
			TypeSymbol type = value.Type;
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(state.ContainingMember, type, SynthesizedLocalKind.With, _withSyntax);
			BoundLocal boundLocal = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(value.Syntax, synthesizedLocal, isLValue: true, type));
			if (value.IsLValue)
			{
				value = ((!state.PreserveIdentityOfLValues) ? value.MakeRValue() : BoundNodeExtensions.MakeCompilerGenerated(new BoundLValueToRValueWrapper(value.Syntax, value, value.Type)));
			}
			state.AddLocal(synthesizedLocal, BoundNodeExtensions.MakeCompilerGenerated(new BoundAssignmentOperator(value.Syntax, boundLocal, value, suppressObjectClone: true, type)));
			return boundLocal;
		}

		private BoundExpression CaptureInAByRefTemp(BoundExpression value, State state)
		{
			TypeSymbol type = value.Type;
			SynthesizedLocal synthesizedLocal = new SynthesizedLocal(state.ContainingMember, type, SynthesizedLocalKind.With, _withSyntax, isByRef: true);
			BoundLocal boundLocal = BoundNodeExtensions.MakeCompilerGenerated(new BoundLocal(value.Syntax, synthesizedLocal, isLValue: true, type));
			state.AddLocal(synthesizedLocal, BoundNodeExtensions.MakeCompilerGenerated(new BoundReferenceAssignment(value.Syntax, boundLocal, value, isLValue: true, type)));
			return boundLocal;
		}

		private BoundExpression CaptureArrayAccess(BoundArrayAccess value, State state)
		{
			BoundExpression expression = CaptureInATemp(value.Expression, state).MakeRValue();
			int length = value.Indices.Length;
			BoundExpression[] array = new BoundExpression[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = CaptureRValue(value.Indices[i], state);
			}
			return value.Update(expression, array.AsImmutableOrNull(), value.IsLValue, value.Type);
		}

		private BoundExpression CaptureRValue(BoundExpression value, State state)
		{
			BoundKind kind = value.Kind;
			if (kind == BoundKind.BadVariable || kind - 107 <= BoundKind.OmittedArgument || kind - 110 <= BoundKind.OmittedArgument)
			{
				return value;
			}
			if (BoundExpressionExtensions.IsValue(value) && (object)value.Type != null && !TypeSymbolExtensions.IsVoidType(value.Type))
			{
				if ((object)value.ConstantValueOpt != null)
				{
					return value;
				}
				return CaptureInATemp(value, state).MakeRValue();
			}
			throw ExceptionUtilities.Unreachable;
		}

		private BoundExpression CaptureFieldAccess(BoundFieldAccess value, State state)
		{
			FieldSymbol fieldSymbol = value.FieldSymbol;
			if (fieldSymbol.IsShared && value.ReceiverOpt != null)
			{
				return value.Update(null, fieldSymbol, value.IsLValue, value.SuppressVirtualCalls, value.ConstantsInProgressOpt, value.Type);
			}
			if (value.ReceiverOpt == null)
			{
				return value;
			}
			BoundExpression receiverOpt = CaptureReceiver(value.ReceiverOpt, state);
			return value.Update(receiverOpt, fieldSymbol, value.IsLValue, value.SuppressVirtualCalls, value.ConstantsInProgressOpt, value.Type);
		}

		private BoundExpression CaptureReceiver(BoundExpression value, State state)
		{
			if (value.IsLValue && value.Type.IsReferenceType)
			{
				return CaptureInATemp(value, state);
			}
			return CaptureExpression(value, state);
		}

		private BoundExpression CaptureExpression(BoundExpression value, State state)
		{
			if (!value.IsLValue)
			{
				return CaptureRValue(value, state);
			}
			switch (value.Kind)
			{
			case BoundKind.ArrayAccess:
				return CaptureArrayAccess((BoundArrayAccess)value, state);
			case BoundKind.FieldAccess:
				return CaptureFieldAccess((BoundFieldAccess)value, state);
			case BoundKind.Local:
			case BoundKind.Parameter:
				return value;
			case BoundKind.WithLValueExpressionPlaceholder:
			{
				BoundExpression withStatementPlaceholderSubstitute = state.Binder.GetWithStatementPlaceholderSubstitute((BoundValuePlaceholderBase)value);
				return (withStatementPlaceholderSubstitute != null) ? CaptureExpression(withStatementPlaceholderSubstitute, state) : value;
			}
			default:
				return CaptureRValue(value, state);
			}
		}

		public Result AnalyzeWithExpression(Symbol containingMember, BoundExpression value, bool doNotUseByRefLocal, Binder binder, bool preserveIdentityOfLValues = false)
		{
			State state = new State(containingMember, doNotUseByRefLocal, binder, preserveIdentityOfLValues);
			return state.CreateResult(CaptureWithExpression(value, state));
		}

		private BoundExpression CaptureWithExpression(BoundExpression value, State state)
		{
			TypeSymbol type = value.Type;
			BoundKind kind = value.Kind;
			if (kind == BoundKind.MeReference || kind == BoundKind.MyClassReference || kind == BoundKind.MyBaseReference)
			{
				return value;
			}
			if (type.IsReferenceType && !TypeSymbolExtensions.IsTypeParameter(type))
			{
				BoundLocal boundLocal = CaptureInATemp(value, state);
				if (!value.IsLValue)
				{
					boundLocal = boundLocal.MakeRValue();
				}
				return boundLocal;
			}
			if (!value.IsLValue)
			{
				return CaptureInATemp(value, state).MakeRValue();
			}
			if (kind == BoundKind.Local || kind == BoundKind.Parameter)
			{
				return value;
			}
			if (!state.DoNotUseByRefLocal && (value.Kind != BoundKind.ArrayAccess || value.Type.Kind != SymbolKind.TypeParameter))
			{
				return CaptureInAByRefTemp(value, state);
			}
			BoundExpression boundExpression = null;
			return value.Kind switch
			{
				BoundKind.ArrayAccess => CaptureArrayAccess((BoundArrayAccess)value, state), 
				BoundKind.FieldAccess => CaptureFieldAccess((BoundFieldAccess)value, state), 
				_ => throw ExceptionUtilities.UnexpectedValue(value.Kind), 
			};
		}
	}
}
