using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class SynthesizedContainer : InstanceTypeSymbol
	{
		private readonly NamedTypeSymbol _containingType;

		private readonly NamedTypeSymbol _baseType;

		private readonly string _name;

		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

		private readonly TypeSubstitution _typeMap;

		private static readonly Func<Symbol, TypeSubstitution> s_typeSubstitutionFactory = (Symbol container) => ((SynthesizedContainer)container).TypeSubstitution;

		private static readonly Func<TypeParameterSymbol, Symbol, TypeParameterSymbol> s_createTypeParameter = (TypeParameterSymbol typeParameter, Symbol container) => new SynthesizedClonedTypeParameterSymbol(typeParameter, container, "SM$" + typeParameter.Name, s_typeSubstitutionFactory);

		protected internal abstract MethodSymbol Constructor { get; }

		public sealed override string Name => _name;

		internal sealed override bool MangleName => _typeParameters.Length > 0;

		internal sealed override bool HasSpecialName => false;

		public override bool IsSerializable => false;

		internal sealed override TypeLayout Layout => default(TypeLayout);

		internal sealed override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

		public abstract override TypeKind TypeKind { get; }

		public override int Arity => _typeParameters.Length;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

		public sealed override bool IsMustInherit => false;

		public sealed override bool MightContainExtensionMethods => false;

		internal sealed override bool HasCodeAnalysisEmbeddedAttribute => false;

		internal sealed override bool HasVisualBasicEmbeddedAttribute => false;

		internal sealed override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

		internal sealed override bool IsWindowsRuntimeImport => false;

		internal override bool ShouldAddWinRTMembers => false;

		internal sealed override bool IsComImport => false;

		internal sealed override TypeSymbol CoClassType => null;

		internal sealed override bool HasDeclarativeSecurity => false;

		internal sealed override string DefaultPropertyName => null;

		public override IEnumerable<string> MemberNames => SpecializedCollections.SingletonEnumerable(".ctor");

		public sealed override Symbol ContainingSymbol => _containingType;

		public sealed override NamedTypeSymbol ContainingType => _containingType;

		public override Accessibility DeclaredAccessibility => Accessibility.Private;

		public sealed override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public sealed override bool IsImplicitlyDeclared => true;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		public sealed override bool IsNotInheritable => true;

		internal override TypeSubstitution TypeSubstitution => _typeMap;

		protected internal SynthesizedContainer(MethodSymbol topLevelMethod, string typeName, NamedTypeSymbol baseType, ImmutableArray<NamedTypeSymbol> originalInterfaces)
		{
			_containingType = topLevelMethod.ContainingType;
			_name = typeName;
			_baseType = baseType;
			if (!topLevelMethod.IsGenericMethod)
			{
				_typeMap = null;
				_typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
				_interfaces = originalInterfaces;
				return;
			}
			_typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(topLevelMethod.OriginalDefinition.TypeParameters, this, s_createTypeParameter);
			TypeSymbol[] array = new TypeSymbol[_typeParameters.Length - 1 + 1];
			int num = _typeParameters.Length - 1;
			for (int j = 0; j <= num; j++)
			{
				array[j] = _typeParameters[j];
			}
			MethodSymbol methodSymbol = topLevelMethod.Construct(array.AsImmutableOrNull());
			_typeMap = TypeSubstitution.Create(methodSymbol.OriginalDefinition, methodSymbol.OriginalDefinition.TypeParameters, array.AsImmutableOrNull());
			_interfaces = originalInterfaces.SelectAsArray((NamedTypeSymbol i) => (NamedTypeSymbol)i.InternalSubstituteTypeParameters(_typeMap).AsTypeSymbolOnly());
		}

		internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		internal sealed override AttributeUsageInfo GetAttributeUsageInfo()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return _baseType;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return MakeAcyclicBaseType(diagnostics);
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return _interfaces;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return MakeAcyclicInterfaces(diagnostics);
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			return ImmutableArray.Create((Symbol)Constructor);
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			if (!CaseInsensitiveComparison.Equals(name, ".ctor"))
			{
				return ImmutableArray<Symbol>.Empty;
			}
			return ImmutableArray.Create((Symbol)Constructor);
		}

		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			return SpecializedCollections.EmptyEnumerable<FieldSymbol>();
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal sealed override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
		}

		internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}
	}
}
