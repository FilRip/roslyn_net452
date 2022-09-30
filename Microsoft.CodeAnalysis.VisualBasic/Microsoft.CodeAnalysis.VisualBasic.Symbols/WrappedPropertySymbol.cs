using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class WrappedPropertySymbol : PropertySymbol
	{
		protected PropertySymbol _underlyingProperty;

		public PropertySymbol UnderlyingProperty => _underlyingProperty;

		public override bool IsImplicitlyDeclared => _underlyingProperty.IsImplicitlyDeclared;

		public override bool ReturnsByRef => _underlyingProperty.ReturnsByRef;

		public override bool IsDefault => _underlyingProperty.IsDefault;

		internal override CallingConvention CallingConvention => _underlyingProperty.CallingConvention;

		public override string Name => _underlyingProperty.Name;

		internal override bool HasSpecialName => _underlyingProperty.HasSpecialName;

		public override ImmutableArray<Location> Locations => _underlyingProperty.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingProperty.DeclaringSyntaxReferences;

		public override Accessibility DeclaredAccessibility => _underlyingProperty.DeclaredAccessibility;

		public override bool IsShared => _underlyingProperty.IsShared;

		public override bool IsOverridable => _underlyingProperty.IsOverridable;

		public override bool IsOverrides => _underlyingProperty.IsOverrides;

		public override bool IsMustOverride => _underlyingProperty.IsMustOverride;

		public override bool IsNotOverridable => _underlyingProperty.IsNotOverridable;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingProperty.ObsoleteAttributeData;

		public override string MetadataName => _underlyingProperty.MetadataName;

		internal override bool HasRuntimeSpecialName => _underlyingProperty.HasRuntimeSpecialName;

		public WrappedPropertySymbol(PropertySymbol underlyingProperty)
		{
			_underlyingProperty = underlyingProperty;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingProperty.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
