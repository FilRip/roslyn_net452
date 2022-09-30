using System.Collections.Immutable;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedAccessor<T> : SynthesizedMethodBase where T : Symbol
	{
		protected readonly T m_propertyOrEvent;

		private string _lazyMetadataName;

		public sealed override string Name => Binder.GetAccessorName(m_propertyOrEvent.Name, MethodKind, SymbolExtensions.IsCompilationOutputWinMdObj(this));

		public override string MetadataName
		{
			get
			{
				if (_lazyMetadataName == null)
				{
					Interlocked.CompareExchange(ref _lazyMetadataName, GenerateMetadataName(), null);
				}
				return _lazyMetadataName;
			}
		}

		internal sealed override bool HasSpecialName => true;

		public sealed override Symbol AssociatedSymbol => m_propertyOrEvent;

		public T PropertyOrEvent => m_propertyOrEvent;

		public sealed override Accessibility DeclaredAccessibility => m_propertyOrEvent.DeclaredAccessibility;

		public sealed override bool IsMustOverride => m_propertyOrEvent.IsMustOverride;

		public sealed override bool IsNotOverridable => m_propertyOrEvent.IsNotOverridable;

		public sealed override bool IsOverloads => SymbolExtensions.IsOverloads(m_propertyOrEvent);

		internal sealed override bool ShadowsExplicitly => m_propertyOrEvent.ShadowsExplicitly;

		public sealed override bool IsOverridable => m_propertyOrEvent.IsOverridable;

		public sealed override bool IsOverrides => m_propertyOrEvent.IsOverrides;

		public sealed override bool IsShared => m_propertyOrEvent.IsShared;

		public sealed override ImmutableArray<Location> Locations => m_propertyOrEvent.Locations;

		internal sealed override Symbol ImplicitlyDefinedBy => m_propertyOrEvent;

		protected SynthesizedAccessor(NamedTypeSymbol container, T propertyOrEvent)
			: base(container)
		{
			m_propertyOrEvent = propertyOrEvent;
		}

		protected virtual string GenerateMetadataName()
		{
			MethodSymbol overriddenMethod = OverriddenMethod;
			if ((object)overriddenMethod != null)
			{
				return overriddenMethod.MetadataName;
			}
			return Name;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return m_propertyOrEvent.GetLexicalSortKey();
		}
	}
}
