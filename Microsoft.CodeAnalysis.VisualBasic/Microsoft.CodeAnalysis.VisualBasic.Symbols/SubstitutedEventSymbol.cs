using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SubstitutedEventSymbol : EventSymbol
	{
		private readonly EventSymbol _originalDefinition;

		private readonly SubstitutedNamedType _containingType;

		private readonly SubstitutedMethodSymbol _addMethod;

		private readonly SubstitutedMethodSymbol _removeMethod;

		private readonly SubstitutedMethodSymbol _raiseMethod;

		private readonly SubstitutedFieldSymbol _associatedField;

		private TypeSymbol _lazyType;

		private ImmutableArray<EventSymbol> _lazyExplicitInterfaceImplementations;

		private OverriddenMembersResult<EventSymbol> _lazyOverriddenOrHiddenMembers;

		internal TypeSubstitution TypeSubstitution => _containingType.TypeSubstitution;

		public override TypeSymbol Type
		{
			get
			{
				if ((object)_lazyType == null)
				{
					Interlocked.CompareExchange(ref _lazyType, _originalDefinition.Type.InternalSubstituteTypeParameters(TypeSubstitution).AsTypeSymbolOnly(), null);
				}
				return _lazyType;
			}
		}

		public override string Name => OriginalDefinition.Name;

		internal override bool HasSpecialName => _originalDefinition.HasSpecialName;

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override EventSymbol OriginalDefinition => _originalDefinition;

		public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

		public override bool IsShared => _originalDefinition.IsShared;

		public override bool IsNotOverridable => _originalDefinition.IsNotOverridable;

		public override bool IsMustOverride => _originalDefinition.IsMustOverride;

		public override bool IsOverridable => _originalDefinition.IsOverridable;

		public override bool IsOverrides => OriginalDefinition.IsOverrides;

		public override bool IsImplicitlyDeclared => OriginalDefinition.IsImplicitlyDeclared;

		public override MethodSymbol AddMethod => _addMethod;

		public override MethodSymbol RemoveMethod => _removeMethod;

		public override MethodSymbol RaiseMethod => _raiseMethod;

		internal override FieldSymbol AssociatedField => _associatedField;

		internal override bool IsExplicitInterfaceImplementation => _originalDefinition.IsExplicitInterfaceImplementation;

		public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitInterfaceImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitInterfaceImplementations, ImplementsHelper.SubstituteExplicitInterfaceImplementations(_originalDefinition.ExplicitInterfaceImplementations, TypeSubstitution), default(ImmutableArray<EventSymbol>));
				}
				return _lazyExplicitInterfaceImplementations;
			}
		}

		public override Accessibility DeclaredAccessibility => OriginalDefinition.DeclaredAccessibility;

		internal override OverriddenMembersResult<EventSymbol> OverriddenOrHiddenMembers
		{
			get
			{
				if (_lazyOverriddenOrHiddenMembers == null)
				{
					Interlocked.CompareExchange(ref _lazyOverriddenOrHiddenMembers, OverrideHidingHelper<EventSymbol>.MakeOverriddenMembers(this), null);
				}
				return _lazyOverriddenOrHiddenMembers;
			}
		}

		internal override ObsoleteAttributeData ObsoleteAttributeData => OriginalDefinition.ObsoleteAttributeData;

		public override bool IsWindowsRuntimeEvent => _originalDefinition.IsWindowsRuntimeEvent;

		internal SubstitutedEventSymbol(SubstitutedNamedType containingType, EventSymbol originalDefinition, SubstitutedMethodSymbol addMethod, SubstitutedMethodSymbol removeMethod, SubstitutedMethodSymbol raiseMethod, SubstitutedFieldSymbol associatedField)
		{
			_containingType = containingType;
			_originalDefinition = originalDefinition;
			_associatedField = associatedField;
			if ((object)addMethod != null)
			{
				addMethod.SetAssociatedPropertyOrEvent(this);
				_addMethod = addMethod;
			}
			if ((object)removeMethod != null)
			{
				removeMethod.SetAssociatedPropertyOrEvent(this);
				_removeMethod = removeMethod;
			}
			if ((object)raiseMethod != null)
			{
				raiseMethod.SetAssociatedPropertyOrEvent(this);
				_raiseMethod = raiseMethod;
			}
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _originalDefinition.GetAttributes();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
