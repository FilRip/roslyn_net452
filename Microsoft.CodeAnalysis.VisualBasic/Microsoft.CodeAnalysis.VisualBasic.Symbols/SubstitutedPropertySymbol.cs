using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Microsoft.Cci;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SubstitutedPropertySymbol : PropertySymbol
	{
		private readonly SubstitutedNamedType _containingType;

		private readonly PropertySymbol _originalDefinition;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly SubstitutedMethodSymbol _getMethod;

		private readonly SubstitutedMethodSymbol _setMethod;

		private readonly SubstitutedFieldSymbol _associatedField;

		public override string Name => _originalDefinition.Name;

		public override string MetadataName => _originalDefinition.MetadataName;

		internal override bool HasSpecialName => _originalDefinition.HasSpecialName;

		internal override ObsoleteAttributeData ObsoleteAttributeData => OriginalDefinition.ObsoleteAttributeData;

		public override PropertySymbol OriginalDefinition => _originalDefinition;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override bool IsWithEvents => _originalDefinition.IsWithEvents;

		public override Accessibility DeclaredAccessibility => _originalDefinition.DeclaredAccessibility;

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImplementsHelper.SubstituteExplicitInterfaceImplementations(_originalDefinition.ExplicitInterfaceImplementations, TypeSubstitution);

		public override MethodSymbol GetMethod => _getMethod;

		public override MethodSymbol SetMethod => _setMethod;

		internal override FieldSymbol AssociatedField => _associatedField;

		public override bool IsDefault => _originalDefinition.IsDefault;

		public override bool IsMustOverride => _originalDefinition.IsMustOverride;

		public override bool IsNotOverridable => _originalDefinition.IsNotOverridable;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

		public override bool IsOverridable => _originalDefinition.IsOverridable;

		public override bool IsOverrides => _originalDefinition.IsOverrides;

		public override bool IsShared => _originalDefinition.IsShared;

		public override bool IsOverloads => _originalDefinition.IsOverloads;

		public override bool IsImplicitlyDeclared => _originalDefinition.IsImplicitlyDeclared;

		public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override int ParameterCount => _originalDefinition.ParameterCount;

		public override bool ReturnsByRef => _originalDefinition.ReturnsByRef;

		public override TypeSymbol Type => _originalDefinition.Type.InternalSubstituteTypeParameters(TypeSubstitution).Type;

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => TypeSubstitution.SubstituteCustomModifiers(_originalDefinition.Type, _originalDefinition.TypeCustomModifiers);

		public override ImmutableArray<CustomModifier> RefCustomModifiers => TypeSubstitution.SubstituteCustomModifiers(_originalDefinition.RefCustomModifiers);

		internal override CallingConvention CallingConvention => _originalDefinition.CallingConvention;

		internal TypeSubstitution TypeSubstitution => _containingType.TypeSubstitution;

		internal override bool IsMyGroupCollectionProperty => false;

		public SubstitutedPropertySymbol(SubstitutedNamedType container, PropertySymbol originalDefinition, SubstitutedMethodSymbol getMethod, SubstitutedMethodSymbol setMethod, SubstitutedFieldSymbol associatedField)
		{
			_containingType = container;
			_originalDefinition = originalDefinition;
			_parameters = SubstituteParameters();
			_getMethod = getMethod;
			_setMethod = setMethod;
			_associatedField = associatedField;
			if ((object)_getMethod != null)
			{
				_getMethod.SetAssociatedPropertyOrEvent(this);
			}
			if ((object)_setMethod != null)
			{
				_setMethod.SetAssociatedPropertyOrEvent(this);
			}
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _originalDefinition.GetAttributes();
		}

		private ImmutableArray<ParameterSymbol> SubstituteParameters()
		{
			ImmutableArray<ParameterSymbol> parameters = _originalDefinition.Parameters;
			int length = parameters.Length;
			if (length == 0)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			ParameterSymbol[] array = new ParameterSymbol[length - 1 + 1];
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = SubstitutedParameterSymbol.CreatePropertyParameter(this, parameters[i]);
			}
			return array.AsImmutableOrNull();
		}

		public override int GetHashCode()
		{
			int hashCode = _originalDefinition.GetHashCode();
			return Hash.Combine(_containingType, hashCode);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is SubstitutedPropertySymbol substitutedPropertySymbol))
			{
				return false;
			}
			if (!_originalDefinition.Equals(substitutedPropertySymbol._originalDefinition))
			{
				return false;
			}
			if (!_containingType.Equals(substitutedPropertySymbol._containingType))
			{
				return false;
			}
			return true;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
