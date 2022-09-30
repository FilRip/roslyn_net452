using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LambdaFrame : SynthesizedContainer, ISynthesizedMethodBodyImplementationSymbol
	{
		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		private readonly MethodSymbol _topLevelMethod;

		private readonly MethodSymbol _sharedConstructor;

		private readonly FieldSymbol _singletonCache;

		internal readonly int ClosureOrdinal;

		internal readonly ArrayBuilder<LambdaCapturedVariable> CapturedLocals;

		private readonly SynthesizedLambdaConstructor _constructor;

		internal readonly TypeSubstitution TypeMap;

		private readonly SyntaxNode _scopeSyntaxOpt;

		private static readonly Func<Symbol, TypeSubstitution> s_typeSubstitutionFactory = (Symbol container) => (!(container is LambdaFrame lambdaFrame)) ? ((SynthesizedMethod)container).TypeMap : lambdaFrame.TypeMap;

		internal static readonly Func<TypeParameterSymbol, Symbol, TypeParameterSymbol> CreateTypeParameter = (TypeParameterSymbol typeParameter, Symbol container) => new SynthesizedClonedTypeParameterSymbol(typeParameter, container, GeneratedNames.MakeDisplayClassGenericParameterName(typeParameter.Ordinal), s_typeSubstitutionFactory);

		public SyntaxNode ScopeSyntax => _constructor.Syntax;

		public override Accessibility DeclaredAccessibility => Accessibility.Internal;

		protected internal override MethodSymbol Constructor => _constructor;

		protected internal MethodSymbol SharedConstructor => _sharedConstructor;

		internal FieldSymbol SingletonCache => _singletonCache;

		public override bool IsSerializable => (object)_singletonCache != null;

		public override IEnumerable<string> MemberNames => SpecializedCollections.EmptyEnumerable<string>();

		public override TypeKind TypeKind => TypeKind.Class;

		public override int Arity => _typeParameters.Length;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

		internal override bool IsInterface => false;

		public bool HasMethodBodyDependency => true;

		public IMethodSymbolInternal Method => _topLevelMethod;

		internal LambdaFrame(MethodSymbol topLevelMethod, SyntaxNode scopeSyntaxOpt, DebugId methodId, DebugId closureId, bool copyConstructor, bool isStatic, bool isDelegateRelaxationFrame)
			: base(topLevelMethod, MakeName(scopeSyntaxOpt, methodId, closureId, isStatic, isDelegateRelaxationFrame), topLevelMethod.ContainingType, ImmutableArray<NamedTypeSymbol>.Empty)
		{
			CapturedLocals = new ArrayBuilder<LambdaCapturedVariable>();
			if (copyConstructor)
			{
				_constructor = new SynthesizedLambdaCopyConstructor(scopeSyntaxOpt, this);
			}
			else
			{
				_constructor = new SynthesizedLambdaConstructor(scopeSyntaxOpt, this);
			}
			if (isStatic)
			{
				_sharedConstructor = new SynthesizedConstructorSymbol(null, this, isShared: true, isDebuggable: false, null, null);
				string name = GeneratedNames.MakeCachedFrameInstanceName();
				_singletonCache = new SynthesizedLambdaCacheFieldSymbol(this, this, this, name, topLevelMethod, Accessibility.Public, isReadOnly: true, isShared: true);
				_scopeSyntaxOpt = null;
			}
			else
			{
				_scopeSyntaxOpt = scopeSyntaxOpt;
			}
			_typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(topLevelMethod.TypeParameters, this, CreateTypeParameter);
			TypeMap = TypeSubstitution.Create(topLevelMethod, topLevelMethod.TypeParameters, base.TypeArgumentsNoUseSiteDiagnostics);
			_topLevelMethod = topLevelMethod;
		}

		private static string MakeName(SyntaxNode scopeSyntaxOpt, DebugId methodId, DebugId closureId, bool isStatic, bool isDelegateRelaxation)
		{
			if (isStatic)
			{
				return GeneratedNames.MakeStaticLambdaDisplayClassName(methodId.Ordinal, methodId.Generation);
			}
			return GeneratedNames.MakeLambdaDisplayClassName(methodId.Ordinal, methodId.Generation, closureId.Ordinal, closureId.Generation, isDelegateRelaxation);
		}

		[Conditional("DEBUG")]
		private static void AssertIsClosureScopeSyntax(SyntaxNode syntaxOpt)
		{
			if (syntaxOpt != null && !LambdaUtilities.IsClosureScope(syntaxOpt))
			{
				SyntaxKind syntaxKind = VisualBasicExtensions.Kind(syntaxOpt);
				if (syntaxKind != SyntaxKind.ObjectMemberInitializer)
				{
					ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(syntaxOpt));
				}
			}
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			ImmutableArray<Symbol> immutableArray = StaticCast<Symbol>.From(CapturedLocals.AsImmutable());
			if ((object)_sharedConstructor != null)
			{
				return immutableArray.AddRange(ImmutableArray.Create<Symbol>(_constructor, _sharedConstructor, _singletonCache));
			}
			return immutableArray.Add(_constructor);
		}

		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			if ((object)_singletonCache == null)
			{
				return CapturedLocals;
			}
			return CapturedLocals.Concat(_singletonCache);
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return ContainingAssembly.GetSpecialType(SpecialType.System_Object);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return ContainingAssembly.GetSpecialType(SpecialType.System_Object);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}
	}
}
