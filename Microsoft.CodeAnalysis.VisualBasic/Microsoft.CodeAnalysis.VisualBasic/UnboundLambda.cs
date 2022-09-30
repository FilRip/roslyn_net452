using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class UnboundLambda : BoundExpression
	{
		internal class TargetSignature
		{
			public readonly ImmutableArray<TypeSymbol> ParameterTypes;

			public readonly TypeSymbol ReturnType;

			public readonly bool ReturnsByRef;

			public readonly BitVector ParameterIsByRef;

			public TargetSignature(ImmutableArray<TypeSymbol> parameterTypes, BitVector parameterIsByRef, TypeSymbol returnType, bool returnsByRef)
			{
				ParameterTypes = parameterTypes;
				ParameterIsByRef = parameterIsByRef;
				ReturnType = returnType;
				ReturnsByRef = returnsByRef;
			}

			public TargetSignature(ImmutableArray<ParameterSymbol> @params, TypeSymbol returnType, bool returnsByRef)
			{
				BitVector empty = BitVector.Empty;
				if (@params.Length == 0)
				{
					ParameterTypes = ImmutableArray<TypeSymbol>.Empty;
				}
				else
				{
					TypeSymbol[] array = new TypeSymbol[@params.Length - 1 + 1];
					int num = @params.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = @params[i].Type;
						if (@params[i].IsByRef)
						{
							empty[i] = true;
						}
					}
					ParameterTypes = array.AsImmutableOrNull();
				}
				ParameterIsByRef = empty;
				ReturnType = returnType;
				ReturnsByRef = returnsByRef;
			}

			public TargetSignature(MethodSymbol method)
				: this(method.Parameters, method.ReturnType, method.ReturnsByRef)
			{
			}

			public override int GetHashCode()
			{
				int currentKey = 0;
				ImmutableArray<TypeSymbol>.Enumerator enumerator = ParameterTypes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					currentKey = Hash.Combine(enumerator.Current, currentKey);
				}
				return Hash.Combine(ReturnType, currentKey);
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
				{
					return true;
				}
				if (!(obj is TargetSignature targetSignature) || targetSignature.ParameterTypes.Length != ParameterTypes.Length)
				{
					return false;
				}
				int num = ParameterTypes.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					if (!TypeSymbol.Equals(ParameterTypes[i], targetSignature.ParameterTypes[i], TypeCompareKind.ConsiderEverything) || ParameterIsByRef[i] != targetSignature.ParameterIsByRef[i])
					{
						return false;
					}
				}
				return ReturnsByRef == targetSignature.ReturnsByRef && TypeSymbol.Equals(ReturnType, targetSignature.ReturnType, TypeCompareKind.ConsiderEverything);
			}
		}

		public class UnboundLambdaBindingCache
		{
			public readonly bool WithDependencies;

			public Tuple<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> AnonymousDelegate;

			public readonly ConcurrentDictionary<TargetSignature, KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>>> InferredReturnType;

			public readonly ConcurrentDictionary<TargetSignature, BoundLambda> BoundLambdas;

			public TargetSignature ErrorRecoverySignature;

			public UnboundLambdaBindingCache(bool withDependencies)
			{
				InferredReturnType = new ConcurrentDictionary<TargetSignature, KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>>>();
				BoundLambdas = new ConcurrentDictionary<TargetSignature, BoundLambda>();
				WithDependencies = withDependencies;
			}
		}

		private readonly Binder _Binder;

		private readonly SourceMemberFlags _Flags;

		private readonly ImmutableArray<ParameterSymbol> _Parameters;

		private readonly TypeSymbol _ReturnType;

		private readonly UnboundLambdaBindingCache _BindingCache;

		public bool IsSingleLine
		{
			get
			{
				SyntaxKind syntaxKind = VisualBasicExtensions.Kind(base.Syntax);
				if (syntaxKind != SyntaxKind.SingleLineFunctionLambdaExpression)
				{
					return syntaxKind == SyntaxKind.SingleLineSubLambdaExpression;
				}
				return true;
			}
		}

		public bool IsFunctionLambda
		{
			get
			{
				SyntaxKind syntaxKind = VisualBasicExtensions.Kind(base.Syntax);
				if (syntaxKind != SyntaxKind.SingleLineFunctionLambdaExpression)
				{
					return syntaxKind == SyntaxKind.MultiLineFunctionLambdaExpression;
				}
				return true;
			}
		}

		public KeyValuePair<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> InferredAnonymousDelegate
		{
			get
			{
				Tuple<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> anonymousDelegate = _BindingCache.AnonymousDelegate;
				if (anonymousDelegate == null)
				{
					KeyValuePair<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> keyValuePair = _Binder.InferAnonymousDelegateForLambda(this);
					Interlocked.CompareExchange(ref _BindingCache.AnonymousDelegate, new Tuple<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>>(keyValuePair.Key, keyValuePair.Value), null);
					anonymousDelegate = _BindingCache.AnonymousDelegate;
				}
				return new KeyValuePair<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>>(anonymousDelegate.Item1, anonymousDelegate.Item2);
			}
		}

		public bool WithDependencies => _BindingCache.WithDependencies;

		public Binder Binder => _Binder;

		public SourceMemberFlags Flags => _Flags;

		public ImmutableArray<ParameterSymbol> Parameters => _Parameters;

		public TypeSymbol ReturnType => _ReturnType;

		public UnboundLambdaBindingCache BindingCache => _BindingCache;

		public BoundLambda Bind(TargetSignature target)
		{
			return _BindingCache.BoundLambdas.GetOrAdd(target, DoBind);
		}

		public KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> InferReturnType(TargetSignature target)
		{
			if ((object)ReturnType != null)
			{
				KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> value = new KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>>((IsFunctionLambda && TypeSymbolExtensions.IsVoidType(ReturnType)) ? LambdaSymbol.ReturnTypeVoidReplacement : ReturnType, default(ImmutableBindingDiagnostic<AssemblySymbol>));
				return _BindingCache.InferredReturnType.GetOrAdd(target, value);
			}
			return _BindingCache.InferredReturnType.GetOrAdd(target, DoInferFunctionLambdaReturnType);
		}

		public BoundLambda BindForErrorRecovery()
		{
			return _Binder.BindLambdaForErrorRecovery(this);
		}

		public BoundLambda GetBoundLambda(TargetSignature target)
		{
			BoundLambda value = null;
			if (_BindingCache.BoundLambdas.TryGetValue(target, out value))
			{
				return value;
			}
			return null;
		}

		private BoundLambda GetSingletonBoundLambda()
		{
			BoundLambda result = _BindingCache.BoundLambdas.Values.FirstOrDefault();
			if (_BindingCache.BoundLambdas.Count == 1)
			{
				return result;
			}
			return null;
		}

		private BoundLambda DoBind(TargetSignature target)
		{
			return _Binder.BindUnboundLambda(this, target);
		}

		private KeyValuePair<TypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> DoInferFunctionLambdaReturnType(TargetSignature target)
		{
			return _Binder.InferFunctionLambdaReturnType(this, target);
		}

		public bool IsInferredDelegateForThisLambda(NamedTypeSymbol delegateType)
		{
			Tuple<NamedTypeSymbol, ImmutableBindingDiagnostic<AssemblySymbol>> anonymousDelegate = _BindingCache.AnonymousDelegate;
			if (anonymousDelegate == null)
			{
				return false;
			}
			return (object)delegateType == anonymousDelegate.Item1;
		}

		public UnboundLambda(SyntaxNode syntax, Binder binder, SourceMemberFlags flags, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType, UnboundLambdaBindingCache bindingCache, bool hasErrors)
			: base(BoundKind.UnboundLambda, syntax, null, hasErrors)
		{
			_Binder = binder;
			_Flags = flags;
			_Parameters = parameters;
			_ReturnType = returnType;
			_BindingCache = bindingCache;
		}

		public UnboundLambda(SyntaxNode syntax, Binder binder, SourceMemberFlags flags, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType, UnboundLambdaBindingCache bindingCache)
			: base(BoundKind.UnboundLambda, syntax, null)
		{
			_Binder = binder;
			_Flags = flags;
			_Parameters = parameters;
			_ReturnType = returnType;
			_BindingCache = bindingCache;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUnboundLambda(this);
		}

		public UnboundLambda Update(Binder binder, SourceMemberFlags flags, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType, UnboundLambdaBindingCache bindingCache)
		{
			if (binder != Binder || flags != Flags || parameters != Parameters || (object)returnType != ReturnType || bindingCache != BindingCache)
			{
				UnboundLambda unboundLambda = new UnboundLambda(base.Syntax, binder, flags, parameters, returnType, bindingCache, base.HasErrors);
				unboundLambda.CopyAttributes(this);
				return unboundLambda;
			}
			return this;
		}
	}
}
