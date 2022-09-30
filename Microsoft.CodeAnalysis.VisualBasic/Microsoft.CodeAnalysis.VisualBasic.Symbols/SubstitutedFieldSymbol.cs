using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SubstitutedFieldSymbol : FieldSymbol
	{
		private readonly SubstitutedNamedType _containingType;

		private readonly FieldSymbol _originalDefinition;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override string Name => _originalDefinition.Name;

		public override string MetadataName => _originalDefinition.MetadataName;

		internal override bool HasSpecialName => _originalDefinition.HasSpecialName;

		internal override bool HasRuntimeSpecialName => _originalDefinition.HasRuntimeSpecialName;

		internal override bool IsNotSerialized => _originalDefinition.IsNotSerialized;

		internal override MarshalPseudoCustomAttributeData MarshallingInformation => _originalDefinition.MarshallingInformation;

		internal override int? TypeLayoutOffset => _originalDefinition.TypeLayoutOffset;

		internal override ObsoleteAttributeData ObsoleteAttributeData => OriginalDefinition.ObsoleteAttributeData;

		public override FieldSymbol OriginalDefinition => _originalDefinition;

		public override bool IsConst => _originalDefinition.IsConst;

		public override object ConstantValue => _originalDefinition.ConstantValue;

		public override Symbol AssociatedSymbol
		{
			get
			{
				Symbol associatedSymbol = OriginalDefinition.AssociatedSymbol;
				if ((object)associatedSymbol != null)
				{
					return SymbolExtensions.AsMember(associatedSymbol, ContainingType);
				}
				return null;
			}
		}

		public override Accessibility DeclaredAccessibility => _originalDefinition.DeclaredAccessibility;

		public override bool IsReadOnly => _originalDefinition.IsReadOnly;

		public override bool IsShared => _originalDefinition.IsShared;

		public override bool IsImplicitlyDeclared => _originalDefinition.IsImplicitlyDeclared;

		public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

		public override TypeSymbol Type => _originalDefinition.Type.InternalSubstituteTypeParameters(_containingType.TypeSubstitution).Type;

		public override ImmutableArray<CustomModifier> CustomModifiers => _containingType.TypeSubstitution.SubstituteCustomModifiers(_originalDefinition.Type, _originalDefinition.CustomModifiers);

		public SubstitutedFieldSymbol(SubstitutedNamedType container, FieldSymbol originalDefinition)
		{
			_containingType = container;
			_originalDefinition = originalDefinition;
		}

		internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			return _originalDefinition.GetConstantValue(inProgress);
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _originalDefinition.GetAttributes();
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
			if (!(obj is SubstitutedFieldSymbol substitutedFieldSymbol))
			{
				return false;
			}
			if (!_originalDefinition.Equals(substitutedFieldSymbol._originalDefinition))
			{
				return false;
			}
			if (!_containingType.Equals(substitutedFieldSymbol._containingType))
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
