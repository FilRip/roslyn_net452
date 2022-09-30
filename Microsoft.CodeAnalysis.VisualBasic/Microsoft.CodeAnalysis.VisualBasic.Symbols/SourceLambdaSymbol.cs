using System.Collections.Immutable;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceLambdaSymbol : LambdaSymbol
	{
		private readonly UnboundLambda _unboundLambda;

		private NamedTypeSymbol _lazyAnonymousDelegateSymbol;

		public UnboundLambda UnboundLambda => _unboundLambda;

		public override SynthesizedLambdaKind SynthesizedKind => SynthesizedLambdaKind.UserDefined;

		public override bool IsAsync => (_unboundLambda.Flags & SourceMemberFlags.Async) != 0;

		public override bool IsIterator => (_unboundLambda.Flags & SourceMemberFlags.Iterator) != 0;

		public override NamedTypeSymbol AssociatedAnonymousDelegate
		{
			get
			{
				if ((object)_lazyAnonymousDelegateSymbol == ErrorTypeSymbol.UnknownResultType)
				{
					NamedTypeSymbol value = MakeAssociatedAnonymousDelegate();
					Interlocked.CompareExchange(ref _lazyAnonymousDelegateSymbol, value, ErrorTypeSymbol.UnknownResultType);
				}
				return _lazyAnonymousDelegateSymbol;
			}
		}

		public SourceLambdaSymbol(SyntaxNode syntaxNode, UnboundLambda unboundLambda, ImmutableArray<BoundLambdaParameterSymbol> parameters, TypeSymbol returnType, Binder binder)
			: base(syntaxNode, parameters, returnType, binder)
		{
			_lazyAnonymousDelegateSymbol = ErrorTypeSymbol.UnknownResultType;
			_unboundLambda = unboundLambda;
		}

		internal NamedTypeSymbol MakeAssociatedAnonymousDelegate()
		{
			NamedTypeSymbol key = _unboundLambda.InferredAnonymousDelegate.Key;
			UnboundLambda.TargetSignature target = new UnboundLambda.TargetSignature(key.DelegateInvokeMethod);
			if ((object)_unboundLambda.Bind(target).LambdaSymbol != this)
			{
				return null;
			}
			return key;
		}
	}
}
