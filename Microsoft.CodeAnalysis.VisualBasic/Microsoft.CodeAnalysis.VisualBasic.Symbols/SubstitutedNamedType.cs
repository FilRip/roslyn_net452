using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SubstitutedNamedType : NamedTypeSymbol
	{
		internal abstract class SpecializedType : SubstitutedNamedType
		{
			protected readonly NamedTypeSymbol _container;

			public override NamedTypeSymbol ConstructedFrom => this;

			public sealed override Symbol ContainingSymbol => _container;

			public new NamedTypeSymbol ContainingType => _container;

			protected SpecializedType(NamedTypeSymbol container, TypeSubstitution substitution)
				: base(substitution)
			{
				_container = container;
			}

			internal sealed override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
			{
				return null;
			}
		}

		internal class SpecializedGenericType : SpecializedType
		{
			private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

			internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => StaticCast<TypeSymbol>.From(TypeParameters);

			internal sealed override bool HasTypeArgumentsCustomModifiers => false;

			internal override bool CanConstruct
			{
				get
				{
					NamedTypeSymbol namedTypeSymbol = _container;
					do
					{
						if (namedTypeSymbol.Arity > 0)
						{
							if ((object)namedTypeSymbol.ConstructedFrom == namedTypeSymbol)
							{
								return false;
							}
							return true;
						}
						namedTypeSymbol = namedTypeSymbol.ContainingType;
					}
					while ((object)namedTypeSymbol != null && !namedTypeSymbol.IsDefinition);
					return true;
				}
			}

			public static SpecializedGenericType Create(NamedTypeSymbol container, NamedTypeSymbol fullInstanceType)
			{
				ImmutableArray<TypeParameterSymbol> typeParameters = fullInstanceType.TypeParameters;
				SubstitutedTypeParameterSymbol[] array = new SubstitutedTypeParameterSymbol[typeParameters.Length - 1 + 1];
				int num = typeParameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = new SubstitutedTypeParameterSymbol(typeParameters[i]);
				}
				ImmutableArray<SubstitutedTypeParameterSymbol> immutableArray = array.AsImmutableOrNull();
				TypeSubstitution substitution = TypeSubstitution.CreateForAlphaRename(container.TypeSubstitution, immutableArray);
				return new SpecializedGenericType(container, substitution, immutableArray);
			}

			private SpecializedGenericType(NamedTypeSymbol container, TypeSubstitution substitution, ImmutableArray<SubstitutedTypeParameterSymbol> typeParameters)
				: base(container, substitution)
			{
				_typeParameters = StaticCast<TypeParameterSymbol>.From(typeParameters);
				ImmutableArray<SubstitutedTypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.SetContainingSymbol(this);
				}
			}

			public sealed override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
			{
				return GetEmptyTypeArgumentCustomModifiers(ordinal);
			}

			public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				CheckCanConstructAndTypeArguments(typeArguments);
				typeArguments = TypeSymbolExtensions.TransformToCanonicalFormFor(typeArguments, this);
				if (typeArguments.IsDefault)
				{
					return this;
				}
				TypeSubstitution substitution = TypeSubstitution.Create(_substitution.Parent, base.OriginalDefinition, typeArguments, allowAlphaRenamedTypeParametersAsArguments: true);
				return new ConstructedSpecializedGenericType(this, substitution);
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal class SpecializedNonGenericType : SpecializedType
		{
			public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

			internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

			internal sealed override bool HasTypeArgumentsCustomModifiers => false;

			internal override bool CanConstruct => false;

			public static SpecializedType Create(NamedTypeSymbol container, NamedTypeSymbol fullInstanceType, TypeSubstitution substitution)
			{
				TypeSubstitution typeSubstitution = container.TypeSubstitution;
				if ((object)substitution.TargetGenericDefinition != fullInstanceType)
				{
					substitution = TypeSubstitution.Concat(fullInstanceType, typeSubstitution, null);
				}
				else if (substitution.Parent != typeSubstitution)
				{
					substitution = TypeSubstitution.Concat(fullInstanceType, typeSubstitution, null);
				}
				return new SpecializedNonGenericType(container, substitution);
			}

			private SpecializedNonGenericType(NamedTypeSymbol container, TypeSubstitution substitution)
				: base(container, substitution)
			{
			}

			public sealed override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
			{
				return GetEmptyTypeArgumentCustomModifiers(ordinal);
			}

			public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				throw new InvalidOperationException();
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
			{
				return new TypeWithModifiers(InternalSubstituteTypeParametersInSpecializedNonGenericType(additionalSubstitution));
			}

			private NamedTypeSymbol InternalSubstituteTypeParametersInSpecializedNonGenericType(TypeSubstitution additionalSubstitution)
			{
				if (additionalSubstitution == null)
				{
					return this;
				}
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)_container.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly();
				if ((object)namedTypeSymbol != _container)
				{
					NamedTypeSymbol originalDefinition = base.OriginalDefinition;
					if (namedTypeSymbol.IsDefinition)
					{
						return originalDefinition;
					}
					return Create(namedTypeSymbol, originalDefinition, namedTypeSymbol.TypeSubstitution);
				}
				return this;
			}
		}

		internal abstract class ConstructedType : SubstitutedNamedType
		{
			private readonly ImmutableArray<TypeSymbol> _typeArguments;

			private readonly bool _hasTypeArgumentsCustomModifiers;

			public sealed override Symbol ContainingSymbol => ConstructedFrom.ContainingSymbol;

			public override bool IsAnonymousType => ConstructedFrom.IsAnonymousType;

			public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ConstructedFrom.TypeParameters;

			internal sealed override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => _typeArguments;

			internal sealed override bool HasTypeArgumentsCustomModifiers => _hasTypeArgumentsCustomModifiers;

			internal override bool CanConstruct => false;

			protected ConstructedType(TypeSubstitution substitution)
				: base(substitution)
			{
				_typeArguments = substitution.GetTypeArgumentsFor(base.OriginalDefinition, out _hasTypeArgumentsCustomModifiers);
			}

			public sealed override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
			{
				if (_hasTypeArgumentsCustomModifiers)
				{
					return _substitution.GetTypeArgumentsCustomModifiersFor(base.OriginalDefinition.TypeParameters[ordinal]);
				}
				return GetEmptyTypeArgumentCustomModifiers(ordinal);
			}

			public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				throw new InvalidOperationException();
			}

			internal sealed override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
			{
				DiagnosticInfo diagnosticInfo = ConstructedFrom.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(_typeArguments, owner, ref checkedTypes);
				if (diagnosticInfo == null && _hasTypeArgumentsCustomModifiers)
				{
					int num = base.Arity - 1;
					for (int i = 0; i <= num; i++)
					{
						diagnosticInfo = Symbol.GetUnificationUseSiteDiagnosticRecursive(GetTypeArgumentCustomModifiers(i), owner, ref checkedTypes);
						if (diagnosticInfo != null)
						{
							break;
						}
					}
				}
				return diagnosticInfo;
			}
		}

		internal class ConstructedInstanceType : ConstructedType
		{
			public override NamedTypeSymbol ConstructedFrom => base.OriginalDefinition;

			public ConstructedInstanceType(TypeSubstitution substitution)
				: base(substitution)
			{
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
			{
				return new TypeWithModifiers(InternalSubstituteTypeParametersInConstructedInstanceType(additionalSubstitution));
			}

			private NamedTypeSymbol InternalSubstituteTypeParametersInConstructedInstanceType(TypeSubstitution additionalSubstitution)
			{
				if (additionalSubstitution == null)
				{
					return this;
				}
				NamedTypeSymbol originalDefinition = base.OriginalDefinition;
				NamedTypeSymbol containingType = originalDefinition.ContainingType;
				NamedTypeSymbol namedTypeSymbol = (((object)containingType == null) ? null : ((NamedTypeSymbol)containingType.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly()));
				TypeSubstitution substitution;
				if ((object)namedTypeSymbol != containingType)
				{
					SpecializedGenericType constructedFrom = SpecializedGenericType.Create(namedTypeSymbol, originalDefinition);
					substitution = TypeSubstitution.AdjustForConstruct(namedTypeSymbol.TypeSubstitution, _substitution, additionalSubstitution);
					return new ConstructedSpecializedGenericType(constructedFrom, substitution);
				}
				substitution = TypeSubstitution.AdjustForConstruct(null, _substitution, additionalSubstitution);
				if (substitution == null)
				{
					return base.OriginalDefinition;
				}
				if (substitution != _substitution)
				{
					return new ConstructedInstanceType(substitution);
				}
				return this;
			}
		}

		internal class ConstructedSpecializedGenericType : ConstructedType
		{
			private readonly SpecializedGenericType _constructedFrom;

			public override NamedTypeSymbol ConstructedFrom => _constructedFrom;

			public ConstructedSpecializedGenericType(SpecializedGenericType constructedFrom, TypeSubstitution substitution)
				: base(substitution)
			{
				_constructedFrom = constructedFrom;
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
			{
				return new TypeWithModifiers(InternalSubstituteTypeParametersInConstructedSpecializedGenericType(additionalSubstitution));
			}

			private NamedTypeSymbol InternalSubstituteTypeParametersInConstructedSpecializedGenericType(TypeSubstitution additionalSubstitution)
			{
				if (additionalSubstitution == null)
				{
					return this;
				}
				NamedTypeSymbol originalDefinition = _constructedFrom.OriginalDefinition;
				NamedTypeSymbol containingType = _constructedFrom.ContainingType;
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)containingType.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly();
				TypeSubstitution typeSubstitution = TypeSubstitution.AdjustForConstruct(namedTypeSymbol.TypeSubstitution, _substitution, additionalSubstitution);
				if (typeSubstitution == null)
				{
					return originalDefinition;
				}
				if (namedTypeSymbol.IsDefinition)
				{
					return new ConstructedInstanceType(typeSubstitution);
				}
				SpecializedGenericType specializedGenericType = _constructedFrom;
				if ((object)namedTypeSymbol != containingType)
				{
					specializedGenericType = SpecializedGenericType.Create(namedTypeSymbol, originalDefinition);
				}
				if ((object)specializedGenericType != _constructedFrom || typeSubstitution != _substitution)
				{
					return new ConstructedSpecializedGenericType(specializedGenericType, typeSubstitution);
				}
				return this;
			}
		}

		private readonly TypeSubstitution _substitution;

		public sealed override string Name => OriginalDefinition.Name;

		internal sealed override bool MangleName => OriginalDefinition.MangleName;

		public sealed override string MetadataName => OriginalDefinition.MetadataName;

		internal sealed override string DefaultPropertyName => OriginalDefinition.DefaultPropertyName;

		internal sealed override bool HasSpecialName => OriginalDefinition.HasSpecialName;

		public sealed override bool IsSerializable => OriginalDefinition.IsSerializable;

		internal override TypeLayout Layout => OriginalDefinition.Layout;

		internal override CharSet MarshallingCharSet => OriginalDefinition.MarshallingCharSet;

		internal sealed override TypeSubstitution TypeSubstitution => _substitution;

		public sealed override NamedTypeSymbol OriginalDefinition => (NamedTypeSymbol)_substitution.TargetGenericDefinition;

		public sealed override AssemblySymbol ContainingAssembly => OriginalDefinition.ContainingAssembly;

		public sealed override int Arity => OriginalDefinition.Arity;

		public sealed override Accessibility DeclaredAccessibility => OriginalDefinition.DeclaredAccessibility;

		public sealed override bool IsMustInherit => OriginalDefinition.IsMustInherit;

		public sealed override bool IsNotInheritable => OriginalDefinition.IsNotInheritable;

		public sealed override bool IsImplicitlyDeclared => OriginalDefinition.IsImplicitlyDeclared;

		internal sealed override EmbeddedSymbolKind EmbeddedSymbolKind => OriginalDefinition.EmbeddedSymbolKind;

		public sealed override bool MightContainExtensionMethods => OriginalDefinition.MightContainExtensionMethods;

		internal sealed override bool HasCodeAnalysisEmbeddedAttribute => OriginalDefinition.HasCodeAnalysisEmbeddedAttribute;

		internal sealed override bool HasVisualBasicEmbeddedAttribute => OriginalDefinition.HasVisualBasicEmbeddedAttribute;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => OriginalDefinition.IsExtensibleInterfaceNoUseSiteDiagnostics;

		internal sealed override bool IsWindowsRuntimeImport => OriginalDefinition.IsWindowsRuntimeImport;

		internal sealed override bool ShouldAddWinRTMembers => OriginalDefinition.ShouldAddWinRTMembers;

		internal sealed override bool IsComImport => OriginalDefinition.IsComImport;

		internal override TypeSymbol CoClassType => OriginalDefinition.CoClassType;

		internal sealed override bool HasDeclarativeSecurity => OriginalDefinition.HasDeclarativeSecurity;

		public sealed override TypeKind TypeKind => OriginalDefinition.TypeKind;

		internal override bool IsInterface => OriginalDefinition.IsInterface;

		public sealed override ImmutableArray<Location> Locations => OriginalDefinition.Locations;

		public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => OriginalDefinition.DeclaringSyntaxReferences;

		public sealed override NamedTypeSymbol EnumUnderlyingType => OriginalDefinition.EnumUnderlyingType;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => OriginalDefinition.ObsoleteAttributeData;

		public override IEnumerable<string> MemberNames => OriginalDefinition.MemberNames;

		private SubstitutedNamedType(TypeSubstitution substitution)
		{
			_substitution = substitution;
		}

		internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return OriginalDefinition.GetAppliedConditionalSymbols();
		}

		internal sealed override AttributeUsageInfo GetAttributeUsageInfo()
		{
			return OriginalDefinition.GetAttributeUsageInfo();
		}

		internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return OriginalDefinition.GetSecurityInformation();
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return OriginalDefinition.GetAttributes();
		}

		internal sealed override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return (NamedTypeSymbol)OriginalDefinition.GetDeclaredBase(basesBeingResolved).InternalSubstituteTypeParameters(_substitution).AsTypeSymbolOnly();
		}

		internal sealed override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = OriginalDefinition.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved);
			if (declaredInterfacesNoUseSiteDiagnostics.Length == 0)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}
			NamedTypeSymbol[] array = new NamedTypeSymbol[declaredInterfacesNoUseSiteDiagnostics.Length - 1 + 1];
			int num = declaredInterfacesNoUseSiteDiagnostics.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = (NamedTypeSymbol)declaredInterfacesNoUseSiteDiagnostics[i].InternalSubstituteTypeParameters(_substitution).AsTypeSymbolOnly();
			}
			return array.AsImmutableOrNull();
		}

		internal sealed override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = OriginalDefinition.BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics != null)
			{
				return (NamedTypeSymbol)baseTypeNoUseSiteDiagnostics.InternalSubstituteTypeParameters(_substitution).AsTypeSymbolOnly();
			}
			return null;
		}

		internal sealed override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<NamedTypeSymbol> interfacesNoUseSiteDiagnostics = OriginalDefinition.InterfacesNoUseSiteDiagnostics;
			if (interfacesNoUseSiteDiagnostics.Length == 0)
			{
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}
			NamedTypeSymbol[] array = new NamedTypeSymbol[interfacesNoUseSiteDiagnostics.Length - 1 + 1];
			int num = interfacesNoUseSiteDiagnostics.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = (NamedTypeSymbol)interfacesNoUseSiteDiagnostics[i].InternalSubstituteTypeParameters(_substitution).AsTypeSymbolOnly();
			}
			return array.AsImmutableOrNull();
		}

		private NamedTypeSymbol SubstituteTypeParametersForMemberType(NamedTypeSymbol memberType)
		{
			if (memberType.Arity == 0)
			{
				return SpecializedNonGenericType.Create(this, memberType, _substitution);
			}
			return SpecializedGenericType.Create(this, memberType);
		}

		protected virtual SubstitutedMethodSymbol SubstituteTypeParametersForMemberMethod(MethodSymbol memberMethod)
		{
			if (memberMethod.Arity > 0)
			{
				return SubstitutedMethodSymbol.SpecializedGenericMethod.Create(this, memberMethod);
			}
			return new SubstitutedMethodSymbol.SpecializedNonGenericMethod(this, memberMethod);
		}

		protected virtual SubstitutedFieldSymbol SubstituteTypeParametersForMemberField(FieldSymbol memberField)
		{
			return new SubstitutedFieldSymbol(this, memberField);
		}

		private SubstitutedPropertySymbol SubstituteTypeParametersForMemberProperty(PropertySymbol memberProperty)
		{
			SubstitutedMethodSymbol getMethod = (((object)memberProperty.GetMethod == null) ? null : SubstituteTypeParametersForMemberMethod(memberProperty.GetMethod));
			SubstitutedMethodSymbol setMethod = (((object)memberProperty.SetMethod == null) ? null : SubstituteTypeParametersForMemberMethod(memberProperty.SetMethod));
			SubstitutedFieldSymbol associatedField = (((object)memberProperty.AssociatedField == null) ? null : SubstituteTypeParametersForMemberField(memberProperty.AssociatedField));
			return new SubstitutedPropertySymbol(this, memberProperty, getMethod, setMethod, associatedField);
		}

		private SubstitutedEventSymbol SubstituteTypeParametersForMemberEvent(EventSymbol memberEvent)
		{
			SubstitutedMethodSymbol addMethod = (((object)memberEvent.AddMethod == null) ? null : SubstituteTypeParametersForMemberMethod(memberEvent.AddMethod));
			SubstitutedMethodSymbol removeMethod = (((object)memberEvent.RemoveMethod == null) ? null : SubstituteTypeParametersForMemberMethod(memberEvent.RemoveMethod));
			SubstitutedMethodSymbol raiseMethod = (((object)memberEvent.RaiseMethod == null) ? null : SubstituteTypeParametersForMemberMethod(memberEvent.RaiseMethod));
			SubstitutedFieldSymbol associatedField = (((object)memberEvent.AssociatedField == null) ? null : SubstituteTypeParametersForMemberField(memberEvent.AssociatedField));
			return CreateSubstitutedEventSymbol(memberEvent, addMethod, removeMethod, raiseMethod, associatedField);
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			ImmutableArray<Symbol> members = OriginalDefinition.GetMembers();
			return GetMembers_Worker(members);
		}

		internal override ImmutableArray<Symbol> GetMembersUnordered()
		{
			ImmutableArray<Symbol> membersUnordered = OriginalDefinition.GetMembersUnordered();
			return GetMembers_Worker(membersUnordered);
		}

		private ImmutableArray<Symbol> GetMembers_Worker(ImmutableArray<Symbol> members)
		{
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			Dictionary<MethodSymbol, SubstitutedMethodSymbol> dictionary = members.OfType<MethodSymbol>().ToDictionary((MethodSymbol m) => m, (MethodSymbol m) => SubstituteTypeParametersForMemberMethod(m));
			ImmutableArray<Symbol>.Enumerator enumerator = members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.NamedType:
					instance.Add(SubstituteTypeParametersForMemberType((NamedTypeSymbol)current));
					break;
				case SymbolKind.Method:
					instance.Add(dictionary[(MethodSymbol)current]);
					break;
				case SymbolKind.Property:
				{
					PropertySymbol propertySymbol = (PropertySymbol)current;
					SubstitutedMethodSymbol methodSubstitute4 = GetMethodSubstitute(dictionary, propertySymbol.GetMethod);
					SubstitutedMethodSymbol methodSubstitute5 = GetMethodSubstitute(dictionary, propertySymbol.SetMethod);
					SubstitutedFieldSymbol associatedField2 = (((object)propertySymbol.AssociatedField == null) ? null : SubstituteTypeParametersForMemberField(propertySymbol.AssociatedField));
					instance.Add(new SubstitutedPropertySymbol(this, propertySymbol, methodSubstitute4, methodSubstitute5, associatedField2));
					break;
				}
				case SymbolKind.Event:
				{
					EventSymbol eventSymbol = (EventSymbol)current;
					SubstitutedMethodSymbol methodSubstitute = GetMethodSubstitute(dictionary, eventSymbol.AddMethod);
					SubstitutedMethodSymbol methodSubstitute2 = GetMethodSubstitute(dictionary, eventSymbol.RemoveMethod);
					SubstitutedMethodSymbol methodSubstitute3 = GetMethodSubstitute(dictionary, eventSymbol.RaiseMethod);
					SubstitutedFieldSymbol associatedField = (((object)eventSymbol.AssociatedField == null) ? null : SubstituteTypeParametersForMemberField(eventSymbol.AssociatedField));
					instance.Add(CreateSubstitutedEventSymbol(eventSymbol, methodSubstitute, methodSubstitute2, methodSubstitute3, associatedField));
					break;
				}
				case SymbolKind.Field:
					instance.Add(SubstituteTypeParametersForMemberField((FieldSymbol)current));
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(current.Kind);
				}
			}
			return instance.ToImmutableAndFree();
		}

		protected virtual SubstitutedEventSymbol CreateSubstitutedEventSymbol(EventSymbol memberEvent, SubstitutedMethodSymbol addMethod, SubstitutedMethodSymbol removeMethod, SubstitutedMethodSymbol raiseMethod, SubstitutedFieldSymbol associatedField)
		{
			return new SubstitutedEventSymbol(this, memberEvent, addMethod, removeMethod, raiseMethod, associatedField);
		}

		private static SubstitutedMethodSymbol GetMethodSubstitute(Dictionary<MethodSymbol, SubstitutedMethodSymbol> methodSubstitutions, MethodSymbol method)
		{
			if ((object)method != null)
			{
				return methodSubstitutions[method];
			}
			return null;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return OriginalDefinition.GetMembers(name).SelectAsArray((Symbol member, SubstitutedNamedType self) => self.SubstituteTypeParametersInMember(member), this);
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return OriginalDefinition.GetTypeMembersUnordered().SelectAsArray((NamedTypeSymbol nestedType, SubstitutedNamedType self) => self.SubstituteTypeParametersForMemberType(nestedType), this);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return OriginalDefinition.GetTypeMembers().SelectAsArray((NamedTypeSymbol nestedType, SubstitutedNamedType self) => self.SubstituteTypeParametersForMemberType(nestedType), this);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return OriginalDefinition.GetTypeMembers(name).SelectAsArray((NamedTypeSymbol nestedType, SubstitutedNamedType self) => self.SubstituteTypeParametersForMemberType(nestedType), this);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return OriginalDefinition.GetTypeMembers(name, arity).SelectAsArray((NamedTypeSymbol nestedType, SubstitutedNamedType self) => self.SubstituteTypeParametersForMemberType(nestedType), this);
		}

		internal sealed override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal Symbol GetMemberForDefinition(Symbol member)
		{
			return SubstituteTypeParametersInMember(member);
		}

		private Symbol SubstituteTypeParametersInMember(Symbol member)
		{
			switch (member.Kind)
			{
			case SymbolKind.NamedType:
				return SubstituteTypeParametersForMemberType((NamedTypeSymbol)member);
			case SymbolKind.Method:
			{
				MethodSymbol methodSymbol = (MethodSymbol)member;
				switch (methodSymbol.MethodKind)
				{
				case MethodKind.PropertyGet:
				case MethodKind.PropertySet:
				{
					SubstitutedPropertySymbol substitutedPropertySymbol = SubstituteTypeParametersForMemberProperty((PropertySymbol)methodSymbol.AssociatedSymbol);
					return (methodSymbol.MethodKind == MethodKind.PropertyGet) ? substitutedPropertySymbol.GetMethod : substitutedPropertySymbol.SetMethod;
				}
				case MethodKind.EventAdd:
					return SubstituteTypeParametersForMemberEvent((EventSymbol)methodSymbol.AssociatedSymbol).AddMethod;
				case MethodKind.EventRemove:
					return SubstituteTypeParametersForMemberEvent((EventSymbol)methodSymbol.AssociatedSymbol).RemoveMethod;
				case MethodKind.EventRaise:
					return SubstituteTypeParametersForMemberEvent((EventSymbol)methodSymbol.AssociatedSymbol).RaiseMethod;
				default:
					return SubstituteTypeParametersForMemberMethod(methodSymbol);
				}
			}
			case SymbolKind.Property:
				return SubstituteTypeParametersForMemberProperty((PropertySymbol)member);
			case SymbolKind.Event:
				return SubstituteTypeParametersForMemberEvent((EventSymbol)member);
			case SymbolKind.Field:
				return SubstituteTypeParametersForMemberField((FieldSymbol)member);
			default:
				throw ExceptionUtilities.UnexpectedValue(member.Kind);
			}
		}

		public sealed override int GetHashCode()
		{
			int hashCode = OriginalDefinition.GetHashCode();
			if (_substitution.WasConstructedForModifiers())
			{
				return hashCode;
			}
			hashCode = Hash.Combine(ContainingType, hashCode);
			if ((object)this != ConstructedFrom)
			{
				ImmutableArray<TypeSymbol>.Enumerator enumerator = TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					hashCode = Hash.Combine(enumerator.Current, hashCode);
				}
			}
			return hashCode;
		}

		public sealed override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			if ((object)other == this)
			{
				return true;
			}
			if ((object)other == null)
			{
				return false;
			}
			if ((comparison & TypeCompareKind.AllIgnoreOptionsForVB) == 0 && !GetType().Equals(other.GetType()))
			{
				return false;
			}
			if (other is TupleTypeSymbol tupleTypeSymbol)
			{
				return tupleTypeSymbol.Equals(this, comparison);
			}
			if (!OriginalDefinition.Equals(other.OriginalDefinition))
			{
				return false;
			}
			NamedTypeSymbol containingType = ContainingType;
			if ((object)containingType != null && !containingType.Equals(other.ContainingType, comparison))
			{
				return false;
			}
			NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)other;
			if ((object)this == ConstructedFrom && (object)namedTypeSymbol == namedTypeSymbol.ConstructedFrom)
			{
				return true;
			}
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = TypeArgumentsNoUseSiteDiagnostics;
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics2 = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics;
			int num = typeArgumentsNoUseSiteDiagnostics.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!typeArgumentsNoUseSiteDiagnostics[i].Equals(typeArgumentsNoUseSiteDiagnostics2[i], comparison))
				{
					return false;
				}
			}
			if ((comparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && !TypeSymbolExtensions.HasSameTypeArgumentCustomModifiers(this, namedTypeSymbol))
			{
				return false;
			}
			return true;
		}

		internal override NamedTypeSymbol GetDirectBaseTypeNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			NamedTypeSymbol directBaseTypeNoUseSiteDiagnostics = OriginalDefinition.GetDirectBaseTypeNoUseSiteDiagnostics(basesBeingResolved);
			if ((object)directBaseTypeNoUseSiteDiagnostics != null)
			{
				return (NamedTypeSymbol)directBaseTypeNoUseSiteDiagnostics.InternalSubstituteTypeParameters(_substitution).AsTypeSymbolOnly();
			}
			return null;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return OriginalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_98_GetSynthesizedWithEventsOverrides))]
		internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_98_GetSynthesizedWithEventsOverrides(-2)
			{
				_0024VB_0024Me = this
			};
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
