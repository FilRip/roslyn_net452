using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class WrappedEventSymbol : EventSymbol
	{
		protected readonly EventSymbol _underlyingEvent;

		public EventSymbol UnderlyingEvent => _underlyingEvent;

		public override bool IsImplicitlyDeclared => _underlyingEvent.IsImplicitlyDeclared;

		internal override bool HasSpecialName => _underlyingEvent.HasSpecialName;

		public override string Name => _underlyingEvent.Name;

		public override ImmutableArray<Location> Locations => _underlyingEvent.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingEvent.DeclaringSyntaxReferences;

		public override Accessibility DeclaredAccessibility => _underlyingEvent.DeclaredAccessibility;

		public override bool IsShared => _underlyingEvent.IsShared;

		public override bool IsOverridable => _underlyingEvent.IsOverridable;

		public override bool IsOverrides => _underlyingEvent.IsOverrides;

		public override bool IsMustOverride => _underlyingEvent.IsMustOverride;

		public override bool IsNotOverridable => _underlyingEvent.IsNotOverridable;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingEvent.ObsoleteAttributeData;

		public override bool IsWindowsRuntimeEvent => _underlyingEvent.IsWindowsRuntimeEvent;

		internal override bool HasRuntimeSpecialName => _underlyingEvent.HasRuntimeSpecialName;

		public WrappedEventSymbol(EventSymbol underlyingEvent)
		{
			_underlyingEvent = underlyingEvent;
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingEvent.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
