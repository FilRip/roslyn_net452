using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedInterfaceImplementationStubSymbol : SynthesizedMethodBase
	{
		private readonly string _name;

		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		private readonly TypeSubstitution _typeParametersSubstitution;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly TypeSymbol _returnType;

		private readonly ImmutableArray<CustomModifier> _customModifiers;

		private ArrayBuilder<MethodSymbol> _explicitInterfaceImplementationsBuilder;

		private ImmutableArray<MethodSymbol> _explicitInterfaceImplementations;

		private static readonly Func<Symbol, TypeSubstitution> s_typeParametersSubstitutionFactory = (Symbol container) => ((SynthesizedInterfaceImplementationStubSymbol)container)._typeParametersSubstitution;

		private static readonly Func<TypeParameterSymbol, Symbol, TypeParameterSymbol> s_createTypeParameter = (TypeParameterSymbol typeParameter, Symbol container) => new SynthesizedClonedTypeParameterSymbol(typeParameter, container, typeParameter.Name, s_typeParametersSubstitutionFactory);

		public override string Name => _name;

		public override int Arity => _typeParameters.Length;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

		public override ImmutableArray<TypeSymbol> TypeArguments => StaticCast<TypeSymbol>.From(TypeParameters);

		public override TypeSymbol ReturnType => _returnType;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => _customModifiers;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => _explicitInterfaceImplementations;

		public override Accessibility DeclaredAccessibility => Accessibility.Private;

		public override bool IsMustOverride => false;

		public override bool IsNotOverridable => false;

		public override bool IsOverloads => false;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsShared => false;

		public override bool IsSub => TypeSymbolExtensions.IsVoidType(_returnType);

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override MethodKind MethodKind => MethodKind.Ordinary;

		internal override bool HasSpecialName => false;

		internal override bool GenerateDebugInfoImpl => false;

		internal SynthesizedInterfaceImplementationStubSymbol(MethodSymbol implementingMethod, MethodSymbol implementedMethod)
			: base(implementingMethod.ContainingType)
		{
			_explicitInterfaceImplementationsBuilder = ArrayBuilder<MethodSymbol>.GetInstance();
			_name = "$VB$Stub_" + implementingMethod.MetadataName;
			_typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(implementingMethod.TypeParameters, this, s_createTypeParameter);
			_typeParametersSubstitution = TypeSubstitution.Create(implementingMethod, implementingMethod.TypeParameters, StaticCast<TypeSymbol>.From(_typeParameters));
			if (implementedMethod.IsGenericMethod)
			{
				implementedMethod = implementedMethod.Construct(StaticCast<TypeSymbol>.From(_typeParameters));
			}
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = implementingMethod.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				ParameterSymbol parameterSymbol = implementedMethod.Parameters[current.Ordinal];
				instance.Add(SynthesizedParameterSymbol.Create(this, parameterSymbol.Type, current.Ordinal, current.IsByRef, current.Name, parameterSymbol.CustomModifiers, parameterSymbol.RefCustomModifiers));
			}
			_parameters = instance.ToImmutableAndFree();
			_returnType = implementedMethod.ReturnType;
			_customModifiers = implementedMethod.ReturnTypeCustomModifiers;
		}

		public void AddImplementedMethod(MethodSymbol implemented)
		{
			_explicitInterfaceImplementationsBuilder.Add(implemented);
		}

		public void Seal()
		{
			_explicitInterfaceImplementations = _explicitInterfaceImplementationsBuilder.ToImmutableAndFree();
			_explicitInterfaceImplementationsBuilder = null;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerHiddenAttribute());
		}

		internal override void AddSynthesizedReturnTypeAttributes(ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedReturnTypeAttributes(ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			if (TypeSymbolExtensions.ContainsTupleNames(ReturnType) && declaringCompilation.HasTupleNamesAttributes)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(ReturnType));
			}
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
