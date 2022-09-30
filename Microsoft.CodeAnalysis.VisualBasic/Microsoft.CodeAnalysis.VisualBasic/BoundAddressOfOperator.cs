using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAddressOfOperator : BoundExpression
	{
		private readonly ConcurrentDictionary<TypeSymbol, Binder.DelegateResolutionResult> _delegateResolutionResultCache;

		private readonly Binder _Binder;

		private readonly bool _WithDependencies;

		private readonly BoundMethodGroup _MethodGroup;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)MethodGroup);

		public Binder Binder => _Binder;

		public bool WithDependencies => _WithDependencies;

		public BoundMethodGroup MethodGroup => _MethodGroup;

		internal bool GetDelegateResolutionResult(TypeSymbol targetType, ref Binder.DelegateResolutionResult delegateResolutionResult)
		{
			return _delegateResolutionResultCache.TryGetValue(targetType, out delegateResolutionResult);
		}

		internal ConversionKind GetConversionClassification(TypeSymbol targetType)
		{
			Binder.DelegateResolutionResult value = default(Binder.DelegateResolutionResult);
			if (!_delegateResolutionResultCache.TryGetValue(targetType, out value))
			{
				value = Binder.InterpretDelegateBinding(this, targetType, isForHandles: false);
				_delegateResolutionResultCache.TryAdd(targetType, value);
			}
			return value.DelegateConversions;
		}

		public BoundAddressOfOperator(SyntaxNode syntax, Binder binder, bool withDependencies, BoundMethodGroup methodGroup, bool hasErrors = false)
			: base(BoundKind.AddressOfOperator, syntax, null, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(methodGroup))
		{
			_delegateResolutionResultCache = new ConcurrentDictionary<TypeSymbol, Binder.DelegateResolutionResult>();
			_Binder = binder;
			_WithDependencies = withDependencies;
			_MethodGroup = methodGroup;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAddressOfOperator(this);
		}

		public BoundAddressOfOperator Update(Binder binder, bool withDependencies, BoundMethodGroup methodGroup)
		{
			if (binder != Binder || withDependencies != WithDependencies || methodGroup != MethodGroup)
			{
				BoundAddressOfOperator boundAddressOfOperator = new BoundAddressOfOperator(base.Syntax, binder, withDependencies, methodGroup, base.HasErrors);
				boundAddressOfOperator.CopyAttributes(this);
				return boundAddressOfOperator;
			}
			return this;
		}
	}
}
